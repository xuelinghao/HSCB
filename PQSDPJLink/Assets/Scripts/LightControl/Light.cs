using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Light : MonoBehaviour
{
    private Text _id;
    /// <summary>
    /// 设备命名ID
    /// </summary>
    public string id;

    /// <summary>
    /// 场景名显示文本框
    /// </summary>
    private Text scene_name;
    /// <summary>
    /// 场景名
    /// </summary>
    public string scene;

    /// <summary>
    /// 设备地址
    /// </summary>
    public string addr;

    /// <summary>
    /// 节目单
    /// </summary>
    public List<FileCycle> program;
    /// <summary>
    /// 当前播放序号
    /// </summary>
    public int curremt_num;
    /// <summary>
    /// 当前播放
    /// </summary>
    private Text current_text;

    /// <summary>
    /// 切换按钮
    /// </summary>
    private Button cut_btn;
    /// <summary>
    /// 切换输入框
    /// </summary>
    private InputField cut_inputField;

    /// <summary>
    /// 下一个按钮
    /// </summary>
    private Button reset_btn;
    /// <summary>
    /// 下一个按钮
    /// </summary>
    private Button stop_btn;


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
    /// SD卡模式
    /// </summary>
    private Button sd;
    /// <summary>
    /// drd文件模式
    /// </summary>
    private Button drd;


    // Start is called before the first frame update
    public void Init()
    {
        _id = transform.Find("ID").GetComponent<Text>();
        _id.text = id;

        scene_name = transform.Find("Scene/name").GetComponent<Text>();
        scene_name.text = scene;

        ///当前播放
        current_text = transform.Find("Catalogue/name").GetComponent<Text>();
        current_text.text = program[curremt_num].name;

        ///切换按钮
        cut_btn = transform.Find("Cut").GetComponent<Button>();
        cut_inputField = transform.Find("program/InputField").GetComponent<InputField>();
        cut_btn.onClick.AddListener(CutOnClick);

        ///重置按钮
        reset_btn = transform.Find("Control/Reset").GetComponent<Button>();
        reset_btn.onClick.AddListener(ResetOnClick);

        ///停止按钮
        stop_btn = transform.Find("Control/Stop").GetComponent<Button>();
        stop_btn.onClick.AddListener(StopOnClick);

        ///播放暂停按钮
        play = transform.Find("Control/play").GetComponent<Image>();
        play.GetComponent<Button>().onClick.AddListener(()=> { StartCoroutine(Play0nClick()); });
        if (isPlay)
            play.sprite = play_spr;
        else
            play.sprite = stop_spr;



        ///下一曲，单曲循环无效
        ctlnx = transform.Find("Control/ctlnx").GetComponent<Button>();
        ctlnx.onClick.AddListener(() => { StartCoroutine(NextOnClick()); });

        ///上一曲，单曲循环无效
        ctlpr = transform.Find("Control/ctlpr").GetComponent<Button>();
        ctlpr.onClick.AddListener(() => { StartCoroutine(PrevOnClick()); });

        ///SD模式
        sd = transform.Find("Control/SD").GetComponent<Button>();
        sd.onClick.AddListener(SDOnClick);

        ///drd模式
        drd = transform.Find("Control/drd").GetComponent<Button>();
        drd.onClick.AddListener(DrdOnClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// 播放暂停
    /// </summary>
    public IEnumerator Play0nClick()
    {
        CycleTypeOnClick(program[curremt_num].cycleType);
        yield return new WaitForSeconds(0.1f);
        if (isPlay)
        {
            LightControl.Instance.Send(addr, program[curremt_num].name, LigntType.Pause);
            play.sprite = stop_spr;
        }
        else
        {
            LightControl.Instance.Send(addr, program[curremt_num].name, LigntType.Play);
            play.sprite = play_spr;
        }
       
        isPlay = !isPlay;
    }
    /// <summary>
    /// 切换
    /// </summary>
    public void CutOnClick()
    {
        if (cut_inputField.text!="")
        {
            LightControl.Instance.Send(addr, cut_inputField.text, LigntType.Appoint);
        }

    }
    /// <summary>
    /// 重置
    /// </summary>
    public void ResetOnClick()
    {
        curremt_num = 0;
        current_text.text = program[curremt_num].name;

        cut_inputField.text = "";

        isPlay = false;
        play.sprite = stop_spr;

        LightControl.Instance.Send(addr, program[0].name, LigntType.Stop);
    }
    /// <summary>
    /// 停止
    /// </summary>
    public void StopOnClick()
    {
        LightControl.Instance.Send(addr, program[curremt_num].name, LigntType.Stop);

    }

    /// <summary>
    /// 下一个
    /// </summary>
    public IEnumerator NextOnClick()
    {
        
        if (curremt_num < program.Count-1) 
        {
            curremt_num++;
            CycleTypeOnClick(program[curremt_num].cycleType);
            yield return new WaitForSeconds(0.1f);
            LightControl.Instance.Send(addr, program[curremt_num].name, LigntType.Appoint);
            CycleTypeOnClick(program[curremt_num].cycleType);
        }
    }
    /// <summary>
    /// 上一个
    /// </summary>
    public IEnumerator PrevOnClick()
    {
        if (curremt_num > 0)
        {
            curremt_num--;
            CycleTypeOnClick(program[curremt_num].cycleType);
            yield return new WaitForSeconds(0.1f);
            LightControl.Instance.Send(addr, program[curremt_num].name, LigntType.Appoint);
            CycleTypeOnClick(program[curremt_num].cycleType);

        }
    }

    /// <summary>
    /// SD卡模式
    /// </summary>
    public void SDOnClick()
    {

        LightControl.Instance.Send(addr, program[curremt_num].name, LigntType.SD);

    }

    /// <summary>
    /// drd文件模式
    /// </summary>
    public void DrdOnClick()
    {
        LightControl.Instance.Send(addr, program[curremt_num].name, LigntType.Drd);

    }
    public void CycleTypeOnClick(CycleType cycleType)
    {
        switch (cycleType)
        {
            case CycleType.stop:
                LightControl.Instance.Send(addr, program[curremt_num].name, LigntType.OneStop);
                break;
            case CycleType.one:
                LightControl.Instance.Send(addr, program[curremt_num].name, LigntType.OneCycle);
                break;
        }
    }
}
