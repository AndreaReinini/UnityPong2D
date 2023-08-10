using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MyMessageListener : MonoBehaviour
{
    public string[] serialMessageReceived;
    public int[] serialMessageConverted;

    private void Awake()
    {
        serialMessageReceived = new string[7];
        serialMessageConverted = new int[7];
    }

    // Invoked when a line of data is received from the serial device.
    void OnMessageArrived(string msg)
    {
        SplitComPortMessage(msg);
    }
    // Invoked when a connect/disconnect event occurs. The parameter 'success'
    // will be 'true' upon connection, and 'false' upon disconnection or
    // failure to connect.
    void OnConnectionEvent(bool success)
    {
        Debug.Log(success ? "Device connected" : "Device disconnected");
    }


    //-------------------------------------------------------------------------

    private void SplitComPortMessage(string msg)
    {
        serialMessageReceived = msg.Split(";");
    }

    public int[] GetMessageById(int id)
    {
        if (id == 1)
        {
            if (serialMessageReceived[0] != null && serialMessageReceived[2] != null && serialMessageReceived[4] != null)
            {
                serialMessageConverted[0] = int.Parse(serialMessageReceived[0]);
                serialMessageConverted[2] = int.Parse(serialMessageReceived[2]);
                serialMessageConverted[4] = int.Parse(serialMessageReceived[4]);
            }
            else
            {
                serialMessageConverted[0] = -1000; ;
                serialMessageConverted[2] = -1000;
                serialMessageConverted[4] = -1000; ;
            }

        }
        else if (id == 2)
        {
            if (serialMessageReceived[1] != null && serialMessageReceived[3] != null && serialMessageReceived[5] != null)
            {
                serialMessageConverted[1] = int.Parse(serialMessageReceived[1]);
                serialMessageConverted[3] = int.Parse(serialMessageReceived[3]);
                serialMessageConverted[5] = int.Parse(serialMessageReceived[5]);
            }
            else
            {
                serialMessageConverted[1] = -1000; ;
                serialMessageConverted[3] = -1000;
                serialMessageConverted[5] = -1000;
            }
        }
        else if (id == 0)
        {
            if (serialMessageReceived[6] != null)
                serialMessageConverted[6] = int.Parse(serialMessageReceived[6]);
            else
                serialMessageConverted[6] = -1000;
        }
        return serialMessageConverted;
    }
}