using System;
using System.IO.Ports;
using UnityEngine;

public class CarData : MonoBehaviour
{
    private SerialPort serialPort;
    public string portName = "COM3"; // Change this to match your OBD-II adapter
    public int baudRate = 115200; // Common baud rate for OBD-II adapters

    void Start()
    {
        try
        {
            serialPort = new SerialPort(portName, baudRate);
            serialPort.ReadTimeout = 1000;
            serialPort.WriteTimeout = 1000;
            serialPort.Open();
            Debug.Log("Connected to OBD-II on " + portName);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to connect to OBD-II: " + e.Message);
        }
    }

    void Update()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            try
            {
                string rpmData = RequestRPM();
                if (!string.IsNullOrEmpty(rpmData))
                {
                    int rpm = ParseRPM(rpmData);
                    Debug.Log("RPM: " + rpm);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error reading from OBD-II: " + e.Message);
            }
        }
    }


    private string RequestRPM()
    {
        serialPort.Write("010C"); // OBD-II request for RPM
        System.Threading.Thread.Sleep(100); // Small delay for response
        return serialPort.ReadLine();
    }

    private int ParseRPM(string response)
    {
        // OBD-II responses typically look like: "41 0C 1A F8"
        string[] bytes = response.Split(' ');

        if (bytes.Length >= 4 && bytes[0] == "41" && bytes[1] == "0C")
        {
            int A = Convert.ToInt32(bytes[2], 16);
            int B = Convert.ToInt32(bytes[3], 16);
            return ((A * 256) + B) / 4; // OBD-II RPM formula
        }

        Debug.LogWarning("Invalid RPM response: " + response);
        return 0;
    }

    void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            Debug.Log("Disconnected from OBD-II.");
        }
    }
}
