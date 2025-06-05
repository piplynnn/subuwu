using System;
using System.IO.Ports;
using System.Threading;
using UnityEngine;
using System.Collections.Generic;

public class CarData : MonoBehaviour
{
    private SerialPort serialPort;
    private Thread serialThread;
    private bool keepReading = true;
    
    public bool timecheck = false;
    public static bool BothActive  = false;
    
    public bool ranonce = false;
    public bool ranonce2 = false;
    public bool ranonceloop = false;
    public bool ranonceloop2 = false;
    public static bool ObdChecked;
    public static bool ObdFound;
    public static bool EcuData;
    public static bool EcuCheck;
    public static int rpm;
    public static int mph;

    public static float throttleper;
    
    public string portName = "COM3"; // Change this based on your OBD-II adapter
    public int baudRate = 115200; // Try 9600, 38400, or 115200 if needed

    private Queue<string> dataQueue = new Queue<string>();
    private readonly object queueLock = new object();

    void Start()
    {
        try
        {
            // Initialize Serial Port
            serialPort = new SerialPort(portName, baudRate)
            {
                ReadTimeout = 5000, // Increased timeout for stability
                WriteTimeout = 1000,
                DtrEnable = true, // Enable if adapter needs it
                RtsEnable = true, // Some adapters may require this
                NewLine = "\r" // Required for ELM327 commands
            };

            serialPort.Open();
            Debug.Log("Connected to OBD-II on " + portName);

            // Start the background thread for reading data
            serialThread = new Thread(ReadSerial);
            serialThread.IsBackground = true;
            serialThread.Start();
            ObdChecked = true;
            ObdFound = true;
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to connect to OBD-II: " + e.Message);
            ObdChecked = true;
            ObdFound = false;
        }
    }



    private void SendCommand(string command)
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Write(command + "\r"); // Ensure proper termination
            //Thread.Sleep(10); // Allow ECU time to process request
        }
    }

    private void ReadSerial()
    {
        while (keepReading && serialPort != null && serialPort.IsOpen)
        {
            try
            {
                string response = serialPort.ReadLine().Trim();
                response = response.Replace(">", "").Trim(); // Remove prompt if present

                lock (queueLock)
                {
                    dataQueue.Enqueue(response);
                }
            }
            catch (TimeoutException)
            {
                Debug.LogWarning("⚠ OBD-II response timeout. No data received.");
            }
        }
    }

void Update()
{
    lock (queueLock)
    {
        while (dataQueue.Count > 0)
        {
            string data = dataQueue.Dequeue();
            ProcessOBDData(data);
        }
    }

    // Initial single request for RPM + Speed
    if (Time.frameCount % 600 == 0 && !ranonce)
    {
        SendCommand("010C0D"); // Request RPM and Speed at the same time
        ranonce = true;
    }

    // Once RPM is verified, send again for confirmation
    else if (Time.frameCount % 600 == 0 && !ranonce2 && timecheck)
    {
        SendCommand("010C0D"); // Repeat RPM and Speed once car is confirmed on
        ranonce2 = true;
    }

    // Active polling for throttle and rpm/speed
    if (Time.frameCount % 6 == 0 && BothActive)
    {
        SendCommand("010C0D"); // Get RPM and Speed
    }

    if (Time.frameCount % 10 == 0 && BothActive)
    {
        SendCommand("014A"); // Get Throttle Position separately
    }
}

private void ProcessOBDData(string response)
{
    // Ignore bad responses
    if (response.Contains("SEARCHING") || response.Contains("?"))
    {
        Debug.LogWarning(response);
        return;
    }

    string[] bytes = response.Split(' ');

    for (int i = 0; i < bytes.Length - 1; i++)
    {
        if (bytes[i] == "41")
        {
            string pid = bytes[i + 1];

            if (pid == "0C" && i + 3 < bytes.Length) // RPM
            {
                int A = Convert.ToInt32(bytes[i + 2], 16);
                int B = Convert.ToInt32(bytes[i + 3], 16);
                rpm = ((A * 256) + B) / 4;
                CarMath.totalrpm += rpm;
                CarMath.rpmcount++;
                Debug.Log("Engine RPM: " + rpm);

                if (!ranonceloop)
                {
                    EcuData = rpm > 0;
                    EcuCheck = true;
                    timecheck = true;
                    ranonceloop = true;
                }
            }
            else if (pid == "0D" && i + 2 < bytes.Length) // Speed
            {
                int kmh = Convert.ToInt32(bytes[i + 2], 16);
                mph = Mathf.RoundToInt(kmh * 0.621371f);
                Debug.Log("🏎 Speed: " + mph + " mph");

                if (!ranonceloop2)
                {
                    BothActive = true;
                    ranonceloop2 = true;
                }
            }
            else if (pid == "4A" && i + 2 < bytes.Length) // Throttle
            {
                int rawPedal = Convert.ToInt32(bytes[i + 2], 16);
                int minRaw = 30;
                int maxRaw = 153;
                rawPedal = Mathf.Clamp(rawPedal, minRaw, maxRaw);
                float pedalPercent = ((rawPedal - minRaw) / (float)(maxRaw - minRaw)) * 100f;
                throttleper = pedalPercent;
                Debug.Log("Throttle = " + throttleper);
            }
        }
    }
}

    void OnApplicationQuit()
    {
        keepReading = false;

        if (serialThread != null && serialThread.IsAlive)
        {
            serialThread.Join(); // Ensure thread ends cleanly
        }

        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            Debug.Log(" Disconnected from OBD-II.");
        }
    }
}
