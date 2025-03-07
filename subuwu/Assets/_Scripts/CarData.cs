using System.Collections;
using UnityEngine;
using System.IO.Ports;
using System.Threading;

public class CarData : MonoBehaviour
{
    private SerialPort serialPort;
    private Thread obdThread;
    private bool isRunning = true;  // Controls OBD-II thread
    private string latestRPM = "0"; // Stores RPM value safely
    private string obdRawResponse = ""; // Stores raw response for debugging
    private object dataLock = new object(); // Ensures thread safety

    void Start()
    {
        serialPort = new SerialPort("COM3", 115200);

        try
        {
            if (!serialPort.IsOpen)
            {
                serialPort.Open();
                Debug.Log("Serial Port Opened!");
                serialPort.WriteLine("ATI\r"); // ✅ Check adapter version
                serialPort.WriteLine("ATSP0\r"); // ✅ Auto-detect OBD-II protocol
                serialPort.WriteLine("ATAT1\r"); // ✅ Enable Adaptive Timing
                serialPort.WriteLine("ATH0\r");  // ✅ Turn off headers
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
            Debug.LogError("Error opening serial port: " + e.Message);
        }
    }

    void Update()
    {
        // Retrieve data safely from the thread
        lock (dataLock)
        {
            Debug.Log($"RPM: {latestRPM} | Raw: {obdRawResponse}"); // ✅ Show latest values
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
                    serialPort.WriteLine("01 0C\r"); // Request RPM
                    string response = serialPort.ReadLine().Trim();

                    lock (dataLock) // ✅ Make it thread-safe
                    {
                        obdRawResponse = response; // ✅ Store raw response
                        latestRPM = ParseRPM(response); // ✅ Store parsed RPM
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
