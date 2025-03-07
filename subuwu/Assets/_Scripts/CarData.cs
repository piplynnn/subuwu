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
    
    public string portName = "COM3"; // Change this based on your OBD-II adapter
    public int baudRate = 115200; // Try 9600, 38400, or 115200 if needed

    private Queue<string> dataQueue = new Queue<string>();
    private readonly object queueLock = new object();
    private float lastRequestTime = 0f;
    private float requestInterval = 2.0f; // Wait 2 seconds between requests

    void Start()
    {
        try
        {
            // Initialize Serial Port
            serialPort = new SerialPort(portName, baudRate)
            {
                ReadTimeout = 3000, // Increased timeout to handle slow responses
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
        Debug.Log("✅ OBD-II Adapter Initialized.");
    }

    private void SendCommand(string command)
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Write(command + "\r"); // Ensure proper termination
            Debug.Log("📤 Sent: " + command);
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
        if (Time.time - lastRequestTime > requestInterval)
        {
            SendCommand("010C"); // Request RPM
            SendCommand("010D"); // Request Speed
            lastRequestTime = Time.time; // Update timestamp
        }
    }


    private void ProcessOBDData(string response)
    {
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
            }
            else if (bytes[0] == "41" && bytes[1] == "0D") // Speed
            {
                int speed = Convert.ToInt32(bytes[2], 16);
                Debug.Log("🏎 Speed: " + speed + " km/h");
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
            Debug.Log("🔌 Disconnected from OBD-II.");
        }
    }
}
