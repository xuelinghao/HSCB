using LitJson;
using rv;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;
using static rv.Command;
using static rv.PowerCommand;

public class TestControl : MonoBehaviour
{
    public InputField ty_ip;
    public InputField ty_port;
    public InputField ty_order;
    Power power = Power.QUERY;


    public InputField rh_ip;
    public InputField rh_port;
    public InputField rh_order;
    List<string> yes;

    public InputField pianzi;
    public InputField shijian;

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);


    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();


    const int SW_SHOWMINIMIZED = 2; //{最小化, 激活}   
    const int SW_SHOWMAXIMIZED = 3;//最大化   
    const int SW_SHOWRESTORE = 1;//还原   
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowWindow(GetForegroundWindow(), SW_SHOWMINIMIZED);
        }
    }

    #region 视频播放盒
    public void PlayOnClick()
    {
        //视频播放盒命令
        UDPServer.Instance.SocketSendIP("192.168.1.104", "192.168.1.104A0301END");
    }
    public void PauseOnClick()
    {
        UDPServer.Instance.SocketSendIP("192.168.1.104", "192.168.1.104CTLPPEND");
    }
    #endregion

    public void RHSendOnClick()
    {
        UDPServer.Instance.SocketSendIP2(rh_ip.text, int.Parse(rh_port.text), rh_order.text);
    }


    public void OpenSendOnClick()
    {

        if (ty_ip.text != "" && ty_port.text == "" && ty_order.text == "") 
        {
            PJLinkConnection c = new PJLinkConnection(ty_ip.text);
            PowerCommand pc = new PowerCommand(Power.ON);
            c.sendCommandAsync(pc, OpenProjection);
        }
        else if(ty_ip.text != "" && ty_port.text != "" && ty_order.text == "")
        {
            PJLinkConnection c = new PJLinkConnection(ty_ip.text, int.Parse(ty_port.text));
            PowerCommand pc = new PowerCommand(Power.ON);
            c.sendCommandAsync(pc, OpenProjection);
        }
        else if (ty_ip.text != "" && ty_port.text == "" && ty_order.text != "")
        {
            PJLinkConnection c = new PJLinkConnection(ty_ip.text, ty_order.text);
            PowerCommand pc = new PowerCommand(Power.ON);
            c.sendCommandAsync(pc, OpenProjection);
        }
        else if (ty_ip.text != "" && ty_port.text == "" && ty_order.text == "")
        {
            PJLinkConnection c = new PJLinkConnection(ty_ip.text, int.Parse(ty_port.text), ty_order.text);
            PowerCommand pc = new PowerCommand(Power.ON);
            c.sendCommandAsync(pc, OpenProjection);
        }
        
    }
    public void CloseSendOnClick()
    {
        if (ty_ip.text != "" && ty_port.text == "" && ty_order.text == "")
        {
            PJLinkConnection c = new PJLinkConnection(ty_ip.text);
            PowerCommand pc = new PowerCommand(Power.OFF);
            c.sendCommandAsync(pc, OpenProjection);
        }
        else if (ty_ip.text != "" && ty_port.text != "" && ty_order.text == "")
        {
            PJLinkConnection c = new PJLinkConnection(ty_ip.text, int.Parse(ty_port.text));
            PowerCommand pc = new PowerCommand(Power.OFF);
            c.sendCommandAsync(pc, OpenProjection);
        }
        else if (ty_ip.text != "" && ty_port.text == "" && ty_order.text != "")
        {
            PJLinkConnection c = new PJLinkConnection(ty_ip.text, ty_order.text);
            PowerCommand pc = new PowerCommand(Power.OFF);
            c.sendCommandAsync(pc, OpenProjection);
        }
        else if (ty_ip.text != "" && ty_port.text == "" && ty_order.text == "")
        {
            PJLinkConnection c = new PJLinkConnection(ty_ip.text, int.Parse(ty_port.text), ty_order.text);
            PowerCommand pc = new PowerCommand(Power.OFF);
            c.sendCommandAsync(pc, OpenProjection);
        }
    }
    /// <summary>
    /// 发送后的返回值
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="response"></param>
    public void OpenProjection(Command sender, Response response)
    {
        switch (response)
        {
            case Response.SUCCESS:
                break;
            case Response.UNDEFINED_CMD:
                break;
            case Response.OUT_OF_PARAMETER:
                break;
            case Response.UNVAILABLE_TIME:
                break;
            case Response.PROJECTOR_FAILURE:
                break;
            case Response.AUTH_FAILURE:
                break;
            case Response.COMMUNICATION_ERROR:
                break;
        }

    }
    public void Playyizi()
    {
        StartCoroutine(Play());
    }
    public IEnumerator Play( )
    {
        UDPServer.Instance.SocketSendIP3("192.168.2.85", 6581, strToToHexByte("EB BE 00 05 00 00 00 00 00 00 00"));
        if (shijian.text!="")
        {
            yield return new WaitForSeconds(float.Parse(shijian.text));
        }
        else
        {
            yield return new WaitForSeconds(1);
        }
        UDPServer.Instance.SocketSendIP2("192.168.2.85", 3012, "PlayProject" + " P1");

    }
    public void Stop()
    {
        UDPServer.Instance.SocketSendIP3("192.168.2.85", 6581, strToToHexByte("EB BE 00 07 00 00 00 00 00 00 00"));
        UDPServer.Instance.SocketSendIP2("192.168.2.85", 3012, "PlayProject" + " P0");

    }
    public void Restoration()
    {
        UDPServer.Instance.SocketSendIP3("192.168.2.85", 6581, strToToHexByte("EB BE 00 04 "+pianzi.text+" 00 09 00 00 00 00"));

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