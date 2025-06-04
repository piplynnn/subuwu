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

            // Run OBD-II initialization
            //InitializeOBD();

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
            Thread.Sleep(10); // Allow ECU time to process request
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

        
        if (Time.frameCount % 600 == 0 && !ranonce) {
            
            SendCommand("010C"); // Request RPM
            ranonce = true;
            
        }

        else if (Time.frameCount % 600 == 0 && !ranonce2 && timecheck)
        {
            SendCommand("010D");
            ranonce2 = true;
        }
		
 		if (Time.frameCount % 10 == 0 && BothActive)
		{
    		SendCommand("010D");
		}
		

        if (Time.frameCount % 8 == 0 && BothActive)
        {
            SendCommand("014A");
			
        }
		
		else if (Time.frameCount % 6 == 0 && BothActive)
		{
    		SendCommand("010C");
		}
	

    
    }

    private void ProcessOBDData(string response)
    {
        // Ignore "SEARCHING..." responses
        if (response.Contains("SEARCHING") || response.Contains("?"))
        {
            Debug.LogWarning(response);
            return;
        }

        string[] bytes = response.Split(' ');
        

        if (bytes.Length >= 2)
        {
            if (bytes[0] == "41" && bytes[1] == "0C") // RPM
            {
                int A = Convert.ToInt32(bytes[2], 16);
                int B = Convert.ToInt32(bytes[3], 16);
                rpm = ((A * 256) + B) / 4;
                CarMath.totalrpm += rpm;
                CarMath.rpmcount++;
                
                Debug.Log("Engine RPM: " + rpm);
                if (!ranonceloop)
                {
                    int initrpm = rpm;
                    if (initrpm > 0)
                    {
                        EcuData = true;
                        EcuCheck = true;

                    }

                    else
                    {
                        EcuData = false;
                        EcuCheck = true;
                    }
                   
                    timecheck = true;
                    ranonceloop = true;
                }
                
                
            }
            else if (bytes[0] == "41" && bytes[1] == "0D") // Speed
            {
                int kmh = Convert.ToInt32(bytes[2], 16);
                mph = Convert.ToInt32(kmh * 0.621371);
                
                Debug.Log("🏎 Speed: " + mph + " mph");
                if (!ranonceloop2)
                {
                    BothActive = true;
                    ranonceloop2 = true;
                    
                }
                
                
            }
            else if (bytes[0] == "41" && bytes[1] == "4A")
            {
				
        		int rawPedal = Convert.ToInt32(bytes[2], 16);
        		int minRaw = 30;   // value at 0% pedal (idle)
        		int maxRaw = 153;  // value at 100% pedal (WOT)
        		rawPedal = Mathf.Clamp(rawPedal, minRaw, maxRaw);
        		float pedalPercent = ((rawPedal - minRaw) / (float)(maxRaw - minRaw)) * 100f;
				throttleper = pedalPercent;
				Debug.Log("Throttle =" + throttleper);
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
