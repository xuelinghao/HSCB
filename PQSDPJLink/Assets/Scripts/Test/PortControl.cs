using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using UnityEngine.UI;
using LitJson;

public class PortControl : MonoBehaviour
{
    //public GUIText gui;
    //定义基本信息
    public string portName = "COM3";
    public int baudRate = 115200;
    public Parity parity = Parity.None;
    public int dataBits = 8;
    public StopBits stopBits = StopBits.One;


    int[] data = new int[6];//用于存储6位数据
    SerialPort sp = null;//串口控制
    Thread dataReceiveThread;
    //发送
    string message = "";
    public Transform parent;
    public GameObject obj;
    public InputField portnumber;
    public InputField baudrate;

    string filename;
    string filePath;
    List<Instruct> ifs;
    char sum = (char)0;
    //byte[] message=new byte[8];
    void Start()
    {
        filePath = Application.dataPath + @"/StreamingAssets/Instruct.json";
        JsonData jsonData = JsonControl.Instance.ReadFromFile(filePath);
        ifs = new List<Instruct>();
        if (jsonData != null)
            ifs = JsonMapper.ToObject<List<Instruct>>(jsonData.ToJson());

        for (int i = 0; i < ifs.Count; i++)
        {
            GameObject go = Instantiate(obj, parent);
            go.transform.GetChild(0).GetComponent<Text>().text = ifs[i].name;
            string str = ifs[i].instruct;
            go.GetComponent<Button>().onClick.AddListener(() => { OnClick(str); });
        }

        OpenPort();//打开串口
        dataReceiveThread = new Thread(new ThreadStart(DataReceiveFunction));//数据接收线程
        dataReceiveThread.Start();

        WriteData(strToToHexByte("FE 0F 00 00 00 08 01 FF F1 D1"));

    }
    public void OnClick(string hexString)
    {
        if (portnumber.text!="")
        {
            portName = portnumber.text;

        }
        if (baudrate.text!="")
        {
            baudRate = int.Parse(baudrate.text);

        }
        WriteData(strToToHexByte(hexString));
    }
    void Update()
    {
    }

    public void OpenPort()
    {
        sp = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
        sp.ReadTimeout = 400;
        try
        {
            sp.Open();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public void ClosePort()
    {
        try
        {
            sp.Close();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public void WriteData(byte[] bys)
    {

        if (sp.IsOpen)
        {
            sp.Write(bys, 0, bys.Length);
        }

    }


    void OnApplicationQuit()
    {
        ClosePort();
    }


    void DataReceiveFunction()//数据接收功能
    {
        byte[] buffer = new byte[128];
        int bytes = 0;
        //定义协议
        int flag0 = 0xFF;
        int flag1 = 0xAA;
        int index = 0;//用于记录此时的数据次序
        while (true)
        {
            if (sp != null && sp.IsOpen)
            {
                try
                {
                    bytes = sp.Read(buffer, 0, buffer.Length);
                    for (int i = 0; i < bytes; i++)
                    {

                        if (buffer[i] == flag0 || buffer[i] == flag1)
                        {
                            index = 0;//次序归0 
                            continue;
                        }
                        else
                        {
                            if (index >= data.Length) index = data.Length - 1;//理论上不应该会进入此判断，但是由于传输的误码，导致数据的丢失，使得标志位与数据个数出错
                            data[index] = buffer[i];//将数据存入data中
                            index++;
                        }
                        Debug.Log(buffer[i]);

                    }
                }
                catch (Exception ex)
                {
                    if (ex.GetType() != typeof(ThreadAbortException))
                    {

                    }
                }
            }
            Thread.Sleep(10);
        }
    }
    private static byte[] strToToHexByte(string hexString)
    {
        hexString = hexString.Replace(" ", "");
        if ((hexString.Length % 2) != 0)
            hexString += " ";
        byte[] returnBytes = new byte[hexString.Length / 2];
        for (int i = 0; i < returnBytes.Length; i++)
            returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
        return returnBytes;
    }
}
public class SheZhi
{
    public string portName;
    public int baudRate;
    public List<Instruct> instructs;
}
public class Instruct
{
    public string name;
    public string instruct;
}