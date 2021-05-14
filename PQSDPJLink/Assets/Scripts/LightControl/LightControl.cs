using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading;
using UnityEngine;

public class LightControl : MonoBehaviour
{
    public static LightControl Instance;
    // Update is called once per frame
    
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

    private void Awake()
    {
        Instance = this;
    }
    //byte[] message=new byte[8];
    void Start()
    {
        OpenPort();//打开串口
        dataReceiveThread = new Thread(new ThreadStart(DataReceiveFunction));//数据接收线程
        dataReceiveThread.Start();
    }

    /// <summary>
    /// 发送
    /// </summary>
    /// <param name="scene">场景盒子ID</param>
    /// <param name="file_name">文件名</param>
    /// <param name="lightType">文件类型</param>
    public void Send(string scene,string file_name, LigntType lightType)
    {
        string str = "55 AA";//设置指令头
        str = str + " " + scene;//设置地址

        DMX d = new DMX();
        switch (lightType)
        {
            case LigntType.Play:
                str = str + " " + d.appoint + " " + FileToHex(file_name);
                break;
            case LigntType.Pause:
                str = str + " " + d.pause+" 00";
                break;
            case LigntType.Stop:
                str = str + " " + d.stop+" 00";
                break;
            case LigntType.Next:
                str = str + " " + d.appoint + " " + FileToHex(file_name);
                break;
            case LigntType.Last:
                str = str + " " + d.appoint + " " + FileToHex(file_name);
                break;
            case LigntType.Appoint:
                str = str + " " + d.appoint + " " + FileToHex(file_name);
                break;
            case LigntType.SD:
                str = str + " " + d.sd;
                break;
            case LigntType.Drd:
                str = str + " " + d.drd;
                break;
            case LigntType.OneCycle:
                str = str + " " + d.oneCycle;
                break;
            case LigntType.OneStop:
                str = str + " " + d.oneStop;
                break;
        }//加入指令


        byte[] by = strToToHexByte(str);
        byte bb = 00;
        for (int i = 2; i < by.Length; i++)
        {
            bb += by[i];
        }
        string strs = Convert.ToString((0 - bb), 16).ToString();//计算数据包校验

        str = str + " " + strs.Substring(strs.Length - 2, 2).ToUpper();

        Debug.Log(str);

        if (sp.IsOpen)
        {
            sp.Write(strToToHexByte(str), 0, strToToHexByte(str).Length);
        }

    }

    /// <summary>
    /// 文件名转换
    /// </summary>
    /// <param name="file_name"></param>
    /// <returns></returns>
    public string FileToHex(string file_name)
    {
        string str = "";
        byte[] strAscByte = Encoding.ASCII.GetBytes(file_name);

        if (strAscByte.Length < 10)
            str = str + " 0" + strAscByte.Length;
        else
            str = str + " " + strAscByte.Length;//加入文件长度

        for (int i = 0; i < strAscByte.Length; i++)
        {
            str = str + " " + Convert.ToString((strAscByte[i]), 16).ToUpper();
        }//加入指定文件
        return str;
    }

    #region 串口模块

    /// <summary>
    /// 打开串口
    /// </summary>
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

    /// <summary>
    /// 关闭串口
    /// </summary>
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

    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="bys"></param>
    public void WriteData(byte[] bys)
    {

        if (sp.IsOpen)
        {
            sp.Write(bys, 0, bys.Length);
        }

    }

    /// <summary>
    /// 退出程序
    /// </summary>
    void OnApplicationQuit()
    {
        ClosePort();
    }

    /// <summary>
    /// 接收数据
    /// </summary>
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

    /// <summary>
    /// 字符串转byte信息
    /// </summary>
    /// <param name="hexString"></param>
    /// <returns></returns>
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
    #endregion
}