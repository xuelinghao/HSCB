using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml;
using UnityEngine;

public class UDPServer : MonoBehaviour
{
    public static UDPServer Instance;

    public string ipAddress;
    public int ConnectPort = 2701;

    byte[] sendData = new byte[1024];

    private List<IPEndPoint> client;

    public Dictionary<string, IPEndPoint> ipList;
    #region  UDP通讯

    private Thread RecviveThread;


    //初始化
    void InitSocket()
    {
        ipList = new Dictionary<string, IPEndPoint>();
        client = new List<IPEndPoint>();
        //LoadXMLPassword();
        RecviveThread = new Thread(() =>
        {
            //实例化一个IPEndPoint，任意IP和对应端口 端口自行修改
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, ConnectPort);
            UdpClient udpReceive = new UdpClient(endPoint);
            UDPData data = new UDPData(endPoint, udpReceive);
            //开启异步接收
            udpReceive.BeginReceive(CallBackRecvive, data);
        })
        {
            //设置为后台线程
            IsBackground = true
        };
        //开启线程
        RecviveThread.Start();
    }
    /// <summary>
    /// 总控主机发送给某台主机
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="sendStr"></param>
    public void SocketSendIP(string ip,string sendStr)
    {
        Debug.Log("IP:" + ip + "sendStr：" + sendStr);
        sendData = new byte[1024];
        sendData = Encoding.UTF8.GetBytes(sendStr);
        UdpClient udpSend = new UdpClient();
        IPEndPoint ipend = new IPEndPoint(IPAddress.Parse(ip), 50505);
        udpSend.Send(sendData, sendData.Length, ipend);
        udpSend.Close();
    }

    /// <summary>
    /// 逻辑主机单发给某个渲染主机
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="sendStr"></param>
    public void SocketSendIP2(string ip,int port, string sendStr)
    {
        Debug.Log("IP:" + ip+ "port"+ port + "sendStr：" + sendStr);

        sendData = new byte[1024];
        sendData = Encoding.UTF8.GetBytes(sendStr);
        UdpClient udpSend = new UdpClient();
        IPEndPoint ipend = new IPEndPoint(IPAddress.Parse(ip), port);
        udpSend.Send(sendData, sendData.Length, ipend);
        udpSend.Close();
    }
    /// <summary>
    /// 逻辑主机单发给某个渲染主机
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="sendStr"></param>
    public void SocketSendIP3(string ip, int port, byte[] sendStr)
    {
        Debug.Log("IP:" + ip + "port" + port + "sendStr：" + sendStr);

        UdpClient udpSend = new UdpClient();
        IPEndPoint ipend = new IPEndPoint(IPAddress.Parse(ip), port);
        udpSend.Send(sendStr, sendStr.Length, ipend);
        udpSend.Close();
    }

    string receiveData = string.Empty;
    IPEndPoint ipEnd = null;
    private Action<string, IPEndPoint> ReceiveCallBack = null;

    class UDPData
    {
        private readonly UdpClient udpClient;
        public UdpClient UDPClient
        {
            get { return udpClient; }
        }
        private readonly IPEndPoint endPoint;
        public IPEndPoint EndPoint
        {
            get { return endPoint; }
        }
        //构造函数
        public UDPData(IPEndPoint endPoint, UdpClient udpClient)
        {
            this.endPoint = endPoint;
            this.udpClient = udpClient;
        }
    }

    /// <summary>
    /// 异步接收回调
    /// </summary>
    /// <param name="ar"></param>
    private void CallBackRecvive(IAsyncResult ar)
    {
        try
        {
            //将传过来的异步结果转为我们需要解析的类型
            UDPData state = ar.AsyncState as UDPData;
            IPEndPoint ipEndPoint = state.EndPoint;
            //结束异步接受 不结束会导致重复挂起线程卡死
            byte[] data = state.UDPClient.EndReceive(ar, ref ipEndPoint);
            //解析数据 编码自己调整暂定为默认 依客户端传过来的编码而定
            receiveData = Encoding.UTF8.GetString(data);
            ipEnd = ipEndPoint;
            //Debug.Log(receiveData);
            if (!client.Contains(ipEndPoint))
            {
                client.Add(ipEndPoint);
            }
            //数据的解析再Update里执行 Unity中Thread无法调用主线程的方法
            //再次开启异步接收数据
            state.UDPClient.BeginReceive(CallBackRecvive, state);

        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            throw;
        }
    }

    public void SetReceiveCallBack(Action<string, IPEndPoint> action)
    {
        ReceiveCallBack = action;
    }
    void ReceiveUDPMessage(string receiveData, IPEndPoint ipep)
    {
        Debug.Log(receiveData);
    }
    #endregion


    private void Awake()
    {
        Instance = this;
    }
    // Use this for initialization
    void Start()
    {
        InitSocket(); //在这里初始化server
        SetReceiveCallBack(ReceiveUDPMessage);
    }
    private void Update()
    {
        //Dispatch();
        if (ReceiveCallBack != null &&
            !string.IsNullOrEmpty(receiveData))
        {
            //调用处理函数去数据进行处理
            ReceiveCallBack(receiveData,ipEnd);
            //使用之后清空接受的数据
            receiveData = string.Empty;
        }
    }
    public string StringOutInt(string str)
    {
        int a = 0;
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] == '1')
                a++;
        }
        string s = "";
        if (a >= 10)
        {
            s = a.ToString();
        }
        else
        {
            s = "0" + a.ToString();
        }
        return s;
    }
}