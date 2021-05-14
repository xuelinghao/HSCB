using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Box : MonoBehaviour
{
    /// <summary>
    /// 路径
    /// </summary>
    private string path;
    /// <summary>
    /// 文件名
    /// </summary>
    private string filename;
    /// <summary>
    /// IP地址
    /// </summary>
    public string ip;
    /// <summary>
    /// 文件路径/文件名集合
    /// </summary>
    public List<string> program;
    /// <summary>
    /// 当前播放的序号
    /// </summary>
    public int num;

    /// <summary>
    /// 场景名显示文本框
    /// </summary>
    private Text scene_name;
    /// <summary>
    /// 场景名
    /// </summary>
    public string scene;

    /// <summary>
    /// 文件所在文件夹显示文本框
    /// </summary>
    private Text catalogue_name;
    /// <summary>
    /// 文件名显示文本框
    /// </summary>
    private Text program_name;


    private Text _id;
    /// <summary>
    /// 设备命名ID
    /// </summary>
    public string id;
    /// <summary>
    /// 开机显示图
    /// </summary>
    private Image start;
    /// <summary>
    /// 是否开机
    /// </summary>
    public bool isStart;

    /// <summary>
    /// 播放暂停显示图
    /// </summary>
    private Image play;
    /// <summary>
    /// 播放图片
    /// </summary>
    public Sprite play_spr;
    /// <summary>
    /// 暂停图片
    /// </summary>
    public Sprite stop_spr;
    /// <summary>
    /// 是否播放
    /// </summary>
    public bool isPlay;

    /// <summary>
    /// 下一个按钮
    /// </summary>
    private Button ctlnx;
    /// <summary>
    /// 上一曲按钮
    /// </summary>
    private Button ctlpr;
    /// <summary>
    /// 单循环列表循环显示图
    /// </summary>
    private Image stop;
    /// <summary>
    /// 音效按钮
    /// </summary>
    private Button volume;
    /// <summary>
    /// 是否静音
    /// </summary>
    public bool isVolume;
    /// <summary>
    /// 音量控制器
    /// </summary>
    private Slider volume_slider;
    /// <summary>
    /// 重启按钮
    /// </summary>
    private Button restart_btn;
    /// <summary>
    /// 音量值
    /// </summary>
    public int volume_num;

    public void Init()
    {
        _id = transform.Find("ID").GetComponent<Text>();
        _id.text = id;
        //string[] str = program[num].Split('/');
        //path = str[0];
        //filename = str[1];
        scene_name = transform.Find("Scene/name").GetComponent<Text>();
        scene_name.text = scene;

        catalogue_name= transform.Find("Catalogue/name").GetComponent<Text>();
        catalogue_name.text = path;

        program_name = transform.Find("program/name").GetComponent<Text>();
        program_name.text = filename;

        ///开机待机按钮
        start = transform.Find("Control/start").GetComponent<Image>();
        start.GetComponent<Button>().onClick.AddListener(StartOnClick);
        if (isStart)
            start.color = new Color(0, 1, 0, 1);
        else
            start.color = new Color(1, 0, 0, 1);

        ///播放暂停按钮
        play = transform.Find("Control/play").GetComponent<Image>();
        play.GetComponent<Button>().onClick.AddListener(Play_Stop);
        if (isPlay)
            play.sprite = play_spr;
        else
            play.sprite = stop_spr;


        ///下一曲，单曲循环无效
        ctlnx = transform.Find("Control/ctlnx").GetComponent<Button>();
        ctlnx.onClick.AddListener(Next);

        ///上一曲，单曲循环无效
        ctlpr = transform.Find("Control/ctlpr").GetComponent<Button>();
        ctlpr.onClick.AddListener(Prev);

        ///单曲循环，目录循环
        stop = transform.Find("Control/stop").GetComponent<Image>();
        stop.GetComponent<Button>().onClick.AddListener(Stop);

        ///静音按钮
        volume = transform.Find("Control/volume").GetComponent<Button>();
        volume.onClick.AddListener(Ctlmu);

        ///音量控制条
        volume_slider = transform.Find("Control/volume_slider").GetComponent<Slider>();
        volume_slider.onValueChanged.AddListener(Volume);
        volume_slider.value = volume_num;

        restart_btn = transform.Find("Restart").GetComponent<Button>();
        restart_btn.onClick.AddListener(Restart);

    }

    /// <summary>
    /// 播放暂停方法
    /// </summary>
    public void Play_Stop()
    {
        if (isPlay)
        {
            UDPServer.Instance.SocketSendIP(ip, ip + "CTLPS" + "END");
            isPlay = false;
            play.sprite = stop_spr;
        }
        else
        {
            UDPServer.Instance.SocketSendIP(ip, ip + "B"+program[num] + "END");
            isPlay = true;
            play.sprite = play_spr;
        }
    }
    /// <summary>
    /// 下一曲方法
    /// </summary>
    public void Next()
    {
        UDPServer.Instance.SocketSendIP(ip, ip + "CTLNX" + "END");
    }
    /// <summary>
    /// 上一曲方法
    /// </summary>
    public void Prev()
    {
        UDPServer.Instance.SocketSendIP(ip, ip + "CTLPR" + "END");
    }
    /// <summary>
    /// 循环方式
    /// </summary>
    public void Stop()
    {
        UDPServer.Instance.SocketSendIP(ip, ip + "STOPP" + "END");
    }
    /// <summary>
    /// 音量值
    /// </summary>

    /// <summary>
    /// 音量设置方法
    /// </summary>
    /// <param name="num"></param>
    public void Volume(float num)
    {
        volume_slider.value = (int)num;
        string str = "00";
        if ((int)num<10)
             str= "0" + (int)num;
        else
            str = ((int)num).ToString();
        UDPServer.Instance.SocketSendIP(ip, ip + "VOL" + str + "END");
        volume_num = (int)num;
    }

    /// <summary>
    /// 静音，再次发送取消静音
    /// </summary>
    public void Ctlmu()
    {
        if (isVolume)
        {
            volume_slider.value = volume_num;
            UDPServer.Instance.SocketSendIP(ip, ip + "VOL" + volume_num.ToString()+ "END");
            isVolume = false;
        }
        else
        {
            volume_num = (int)volume_slider.value;
            volume_slider.value = 0;
            isVolume = true;
        }
        UDPServer.Instance.SocketSendIP(ip, ip + "CTLMU" + "END");

    }

    /// <summary>
    /// 开机关机方法
    /// </summary>
    public void StartOnClick()
    {
        if (isStart)
        {
            UDPServer.Instance.SocketSendIP(ip, ip + "STDBY" + "END");
            isStart = false;
            start.color = new Color(1, 0, 0, 1);
        }
        else
        {
            UDPServer.Instance.SocketSendIP(ip, ip + "PWRON" + "END");
            isStart = true;
            start.color = new Color(0, 1, 0, 1);

        }
    }

    /// <summary>
    /// 查询文件校验
    /// </summary>
    public void ECHKF()
    {

    }

    public void Restart()
    {
        UDPServer.Instance.SocketSendIP(ip, ip + "MRWET" + "END");

    }
}
