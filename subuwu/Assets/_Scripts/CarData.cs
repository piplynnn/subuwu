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
       

        try
        {
            if (!serialPort.IsOpen)
            {
                serialPort.Open();
                Debug.Log("✅ Serial Port Opened!");

                // ✅ Reset OBD-II Adapter
                serialPort.WriteLine("ATZ\r");
                Thread.Sleep(1000); // Give the adapter time to reset

                // ✅ Set Protocol (Autodetect)
                serialPort.WriteLine("ATSP0\r");
                Thread.Sleep(500);

                // ✅ Enable Adaptive Timing (Faster ECU Responses)
                serialPort.WriteLine("ATAT1\r");
                Thread.Sleep(500);

                // ✅ Turn off Headers (Removes extra text from responses)
                serialPort.WriteLine("ATH0\r");
                Thread.Sleep(500);

                Debug.Log("✅ OBD-II Adapter Initialized!");
            }

            if (obdThread == null || !obdThread.IsAlive)
            {
                obdThread = new Thread(ReadOBDData);
                obdThread.IsBackground = true; // ✅ Allows Unity to close without issues
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
        Debug.Log("OBD-II Thread Started!");

        while (isRunning)
        {
            if (serialPort.IsOpen)
            {
                try
                {
                    serialPort.WriteLine("01 0C\r"); // ✅ Request RPM
                    Thread.Sleep(500); // ✅ Wait for ECU response

                    // ✅ Check if there is data before reading
                    if (serialPort.BytesToRead > 0)
                    {
                        string response = serialPort.ReadExisting().Trim(); // ✅ Read all available data
                        Debug.Log("Raw OBD Response: " + response);

                        lock (dataLock) // ✅ Ensure thread safety
                        {
                            obdRawResponse = response;
                            latestRPM = ParseRPM(response);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("No data available to read from OBD-II.");
                    }
                }
                catch (System.TimeoutException)
                {
                    lock (dataLock) { latestRPM = "No Data"; }
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Serial Read Error: " + e.Message);
                }
            }
            else
            {
                Debug.LogError("Serial Port Closed Unexpectedly!");
                break;
            }

            Thread.Sleep(100); // ✅ Prevents excessive CPU usage
        }
    }

    string ParseRPM(string rawResponse)
    {
        if (string.IsNullOrEmpty(rawResponse))
            return "Invalid Data";

        string[] parts = rawResponse.Split(' ');

        if (parts.Length >= 4 && parts[0] == "41" && parts[1] == "0C") // ✅ Ensure valid response
        {
            int rpm = (int.Parse(parts[2], System.Globalization.NumberStyles.HexNumber) * 256) +
                      int.Parse(parts[3], System.Globalization.NumberStyles.HexNumber);

            if (rpm == 0)
            {
                Debug.LogWarning("ECU is reporting 0 RPM. Is the engine running?");
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
            obdThread.Abort(); // ✅ Wait for the thread to finish instead of force-closing it
        }

        if (serialPort.IsOpen)
        {
            serialPort.Close();
            Debug.Log("Serial Port Closed.");
        }
    }
}
