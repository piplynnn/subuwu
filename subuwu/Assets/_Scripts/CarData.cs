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
    
    private bool timecheck = false;
    
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
            Debug.Log("✅ Connected to OBD-II on " + portName);

            // Run OBD-II initialization
            InitializeOBD();

            // Start the background thread for reading data
            serialThread = new Thread(ReadSerial);
            serialThread.IsBackground = true;
            serialThread.Start();
        }
        catch (Exception e)
        {
            Debug.LogError("❌ Failed to connect to OBD-II: " + e.Message);
        }
    }

    private void InitializeOBD()
    {
        SendCommand("ATZ");   // Reset ELM327
        Thread.Sleep(1000);   // Wait for reset
        SendCommand("ATE0");  // Turn off echo (prevents duplicate input)
        Thread.Sleep(500);
        SendCommand("ATSP0"); // Auto-detect OBD-II protocol
        Thread.Sleep(500);
        SendCommand("ATST 64"); // Increase Timeout (Prevents "SEARCHING..." from cutting off)
        Thread.Sleep(500);
        Debug.Log("✅ OBD-II Adapter Initialized.");
    }

    private void SendCommand(string command)
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Write(command + "\r"); // Ensure proper termination
            Debug.Log("📤 Sent: " + command);
            Thread.Sleep(500); // Allow ECU time to process request
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

        // Example: Continuously request RPM and Speed in Unity
        if (Time.frameCount % 600 == 0 && !timecheck) // Request every 3 seconds
        {
            SendCommand("010C"); // Request RPM
            //SendCommand("010D"); // Request Speed
        }

        if (Time.frameCount % 6 == 0 && timecheck)
        {
            SendCommand("010C");
        }
    }

    private void ProcessOBDData(string response)
    {
        // Ignore "SEARCHING..." responses
        if (string.IsNullOrWhiteSpace(response) || response.Contains("SEARCHING") || response.Contains("?"))
        {
            Debug.LogWarning("⚠ Ignored invalid response: " + response);
            return;
        }

        Debug.Log("📥 Raw Response: " + response);

        string[] bytes = response.Split(' ');

        if (bytes.Length >= 4)
        {
            if (bytes[0] == "41" && bytes[1] == "0C") // RPM
            {
                int A = Convert.ToInt32(bytes[2], 16);
                int B = Convert.ToInt32(bytes[3], 16);
                int rpm = ((A * 256) + B) / 4;
                Debug.Log("🔥 Engine RPM: " + rpm);
                timecheck = true;
            }
            else if (bytes[0] == "41" && bytes[1] == "0D") // Speed
            {
                int speed = Convert.ToInt32(bytes[2], 16);
                Debug.Log("🏎 Speed: " + speed + " km/h");
            }
        }
        else
        {
            Debug.LogWarning("⚠ Invalid OBD-II response format: " + response);
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
            Debug.Log("🔌 Disconnected from OBD-II.");
        }
    }
}
