using System.Collections;
using UnityEngine;
using System.IO.Ports;
using System.Threading;

public class CarData : MonoBehaviour
{
    private SerialPort serialPort;
    private Thread obdThread;
    private bool isRunning = true; // Controls the OBD-II reading thread
    private string latestRPM = "0"; // Stores RPM value safely
    private string obdRawResponse = ""; // Stores raw response for debugging
    private object dataLock = new object(); // Ensures thread safety

    void Start()
    {
        serialPort = new SerialPort("COM3", 115200, Parity.None, 8, StopBits.One);
        serialPort.ReadTimeout = 3000;  // ✅ Increase timeout to prevent missing data
        serialPort.WriteTimeout = 3000;

        try
        {
            if (!serialPort.IsOpen)
            {
                serialPort.Open();
                Debug.Log("✅ Serial Port Opened!");

                // ✅ Reset OBD-II Adapter
                serialPort.Write("ATZ\r");
                Thread.Sleep(1000);

                // ✅ Force a Specific Protocol (Try ATSP6 first, then ATSP1)
                serialPort.Write("ATSP6\r"); // 🚨 If ATSP6 doesn't work, try ATSP1
                Thread.Sleep(500);

                // ✅ Enable Adaptive Timing (Faster ECU Responses)
                serialPort.Write("ATAT1\r");
                Thread.Sleep(500);

                // ✅ Turn off Headers (Removes extra text from responses)
                serialPort.Write("ATH0\r");
                Thread.Sleep(500);

                Debug.Log("✅ OBD-II Adapter Initialized!");

                // ✅ Manual test to check if ECU responds
                serialPort.Write("01 0C\r"); // Request RPM manually
                Thread.Sleep(1000);
                string response = serialPort.ReadLine().Trim();
                Debug.Log("✅ Manual OBD Response: " + response);
            }

            if (obdThread == null || !obdThread.IsAlive)
            {
                obdThread = new Thread(ReadOBDData);
                obdThread.IsBackground = true;
                obdThread.Start();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("❌ Error opening serial port: " + e.Message);
        }
    }

    void Update()
    {
        // ✅ Safely retrieve and display latest OBD-II data
        lock (dataLock)
        {
            Debug.Log($"RPM: {latestRPM} | Raw: {obdRawResponse}");
        }
    }

    void ReadOBDData()
    {
        Debug.Log("✅ OBD-II Thread Started!");

        while (isRunning)
        {
            if (serialPort.IsOpen)
            {
                try
                {
                    serialPort.DiscardInBuffer(); // ✅ Clears old data before sending new command
                    serialPort.Write("01 0C\r"); // ✅ Ensure proper command formatting
                    Thread.Sleep(1000); // ✅ Give ECU time to respond

                    // ✅ Read the full response
                    string response = serialPort.ReadLine().Trim();
                    Debug.Log("🔍 Raw OBD Response: " + response);

                    if (string.IsNullOrEmpty(response) || response.Contains("SEARCHING") || response.Contains("STOPPED"))
                    {
                        Debug.LogError("❌ ECU is not responding properly. Trying another protocol.");
                        
                        // ✅ Switch to ISO 9141-2 if CAN fails
                        serialPort.Write("ATSP1\r"); 
                        Thread.Sleep(500);
                    }
                    else
                    {
                        lock (dataLock) 
                        {
                            obdRawResponse = response;
                            latestRPM = ParseRPM(response);
                        }
                    }
                }
                catch (System.TimeoutException)
                {
                    lock (dataLock) { latestRPM = "No Data"; }
                }
                catch (System.Exception e)
                {
                    Debug.LogError("❌ Serial Read Error: " + e.Message);
                }
            }
            else
            {
                Debug.LogError("❌ Serial Port Closed Unexpectedly!");
                break;
            }

            Thread.Sleep(100); // ✅ Prevents excessive CPU usage
        }
    }

    string ParseRPM(string rawResponse)
    {
        if (string.IsNullOrEmpty(rawResponse))
            return "Invalid Data";

        // ✅ Extract correct part of the response
        string[] parts = rawResponse.Split(' ');

        if (parts.Length >= 4 && parts[0] == "41" && parts[1] == "0C") // ✅ Ensure valid response
        {
            int rpm = (int.Parse(parts[2], System.Globalization.NumberStyles.HexNumber) * 256) +
                      int.Parse(parts[3], System.Globalization.NumberStyles.HexNumber);

            if (rpm == 0)
            {
                Debug.LogWarning("⚠️ ECU is reporting 0 RPM. Is the engine running?");
            }

            return (rpm / 4).ToString(); // ✅ Convert to actual RPM
        }

        return "Invalid Data";
    }

    void OnDestroy()
    {
        isRunning = false; // ✅ Stop the thread loop

        if (obdThread != null && obdThread.IsAlive)
        {
            obdThread.Abort(); // ✅ Force close thread to prevent crashes
        }

        if (serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}
