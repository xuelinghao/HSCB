using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fuse : MonoBehaviour
{
    /// <summary>
    /// 节目名
    /// </summary>
    private string filename;

    /// <summary>
    /// IP地址
    /// </summary>
    public string ip;

    /// <summary>
    /// 节目单集合
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
    /// 文件名显示文本框
    /// </summary>
    private Text program_name;


    private Text _id;
    /// <summary>
    /// 设备命名ID
    /// </summary>
    public string id;

    /// <summary>
    /// 重启显示图
    /// </summary>
    private Image restart;

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
    /// 停止
    /// </summary>
    private Image stop;

    /// <summary>
    /// 下一个按钮
    /// </summary>
    private Button ctlnx;
    /// <summary>
    /// 上一曲按钮
    /// </summary>
    private Button ctlpr;

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
    /// 音量值
    /// </summary>
    public int volume_num;
    public List<FuseData> FuseDatas;
    public void Init()
    {
        _id = transform.Find("ID").GetComponent<Text>();
        _id.text = id;

        scene_name = transform.Find("Scene/name").GetComponent<Text>();
        scene_name.text = scene;

        //节目单文本框
        program_name = transform.Find("Catalogue/name").GetComponent<Text>();
        program_name.text = program[num];

        ///重启按钮
        restart = transform.Find("Control/restart").GetComponent<Image>();
        restart.GetComponent<Button>().onClick.AddListener(Restart);

        ///播放暂停按钮
        play = transform.Find("Control/play").GetComponent<Image>();
        play.GetComponent<Button>().onClick.AddListener(Play_Pause);
        if (isPlay)
            play.sprite = play_spr;
        else
            play.sprite = stop_spr;


        //停止按钮
        stop = transform.Find("Control/stop").GetComponent<Image>();
        stop.GetComponent<Button>().onClick.AddListener(Stop);

        ///下一曲，单曲循环无效
        ctlnx = transform.Find("Control/ctlnx").GetComponent<Button>();
        ctlnx.onClick.AddListener(Next);

        ///上一曲，单曲循环无效
        ctlpr = transform.Find("Control/ctlpr").GetComponent<Button>();
        ctlpr.onClick.AddListener(Prev);

        ///静音按钮
        volume = transform.Find("Control/volume").GetComponent<Button>();
        volume.onClick.AddListener(Ctlmu);

        ///音量控制条
        volume_slider = transform.Find("Control/volume_slider").GetComponent<Slider>();
        volume_slider.onValueChanged.AddListener(Volume);

    }

    /// <summary>
    /// 播放暂停方法
    /// </summary>
    public void Play_Pause()
    {
        if (isPlay)
        {
            if (program[num].Length > 5)
            {
                for (int i = 0; i < FuseDatas.Count; i++)
                {
                    UDPServer.Instance.SocketSendIP2(FuseDatas[i].ip, 3012, "Pause");
                }
            }
            else
            {
                for (int i = 0; i < FuseDatas.Count; i++)
                {
                    UDPServer.Instance.SocketSendIP2(FuseDatas[i].ip, 3012, "PauseProject");
                }

            }
            isPlay = false;
            play.sprite = stop_spr;
        }
        else
        {
            if (program[num].Length > 5)
            {
                for (int i = 0; i < FuseDatas.Count; i++)
                {
                    UDPServer.Instance.SocketSendIP2(FuseDatas[i].ip, 3012, "Update" + " " + program[num]);
                }
            }
            else
            {
                for (int i = 0; i < FuseDatas.Count; i++)
                {
                    UDPServer.Instance.SocketSendIP2(FuseDatas[i].ip, 3012, "PlayProject" + " " + program[num]);
                }

            }
            isPlay = true;
            play.sprite = play_spr;
        }
    }

    public void Stop()
    {
        if (program[num].Length > 5)
            UDPServer.Instance.SocketSendIP2(ip, 3012, "Stop");
        else
            UDPServer.Instance.SocketSendIP2(ip, 3012, "StopProject");

    }
    /// <summary>
    /// 下一曲方法
    /// </summary>
    public void Next()
    {
        if (num < program.Count - 1) 
        {
            num++;
            if (program[num].Length > 5)
            {
                for (int i = 0; i < FuseDatas.Count; i++)
                {
                    UDPServer.Instance.SocketSendIP2(FuseDatas[i].ip, 3012, "Update" + " " + program[num]);
                }
            }
            else
            {
                for (int i = 0; i < FuseDatas.Count; i++)
                {
                    UDPServer.Instance.SocketSendIP2(FuseDatas[i].ip, 3012, "PlayProject" + " " + program[num]);
                }
            }
            //UDPServer.Instance.SocketSendIP2(ip, 3012, "MinWnd");

        }
    }
    /// <summary>
    /// 上一曲方法
    /// </summary>
    public void Prev()
    {
        if (num > 0)
        {
            num--; 
            if (program[num].Length > 5)
            {
                for (int i = 0; i < FuseDatas.Count; i++)
                {
                    UDPServer.Instance.SocketSendIP2(FuseDatas[i].ip, 3012, "Update" + " " + program[num]);
                }
            }
            else
            {
                for (int i = 0; i < FuseDatas.Count; i++)
                {
                    UDPServer.Instance.SocketSendIP2(FuseDatas[i].ip, 3012, "PlayProject" + " " + program[num]);
                }
            }
            //UDPServer.Instance.SocketSendIP2(ip, 3012, "RestoreWnd");

        }
    }
    

    /// <summary>
    /// 音量设置方法
    /// </summary>
    /// <param name="num"></param>
    public void Volume(float num)
    {
        volume_slider.value = (int)num;
        if ((int)num > volume_num)
            UDPServer.Instance.SocketSendIP2(ip, 3012, "InsSystemVol" + (int)num);
        else
            UDPServer.Instance.SocketSendIP2(ip, 3012, "DesSystemVol" + (int)num);
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
            UDPServer.Instance.SocketSendIP2(ip, 3012, "InsSystemVol" + 0);
            isVolume = false;
        }
        else
        {
            UDPServer.Instance.SocketSendIP2(ip, 3012, "InsSystemVol" + 1);
            volume_num = (int)volume_slider.value;
            volume_slider.value = 0;
            isVolume = true;
        }
    }
    /// <summary>
    /// 重启
    /// </summary>
    public void Restart()
    {
        UDPServer.Instance.SocketSendIP2(ip, 3012, "RestartServer");
    }
}
