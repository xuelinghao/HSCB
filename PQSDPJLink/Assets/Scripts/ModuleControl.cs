using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModuleControl : MonoBehaviour
{
    public static ModuleControl Instance;
    /// <summary>
    /// 执行到了第几步
    /// </summary>
    public int num;
    /// <summary>
    /// 场景状态集合
    /// </summary>
    public List<SceneState1> sceneState1s;
    /// <summary>
    /// 队伍
    /// </summary>
    public int troops;
    /// <summary>
    /// 下一步按钮
    /// </summary>
    public Button next_btn;
    /// <summary>
    /// 执行时间
    /// </summary>
    public float timer;
    /// <summary>
    /// 是否等待时间到
    /// </summary>
    public bool isStop;
    /// <summary>
    /// 时间记录
    /// </summary>
    public float times;
    /// <summary>
    /// 场景名显示文本框
    /// </summary>
    public Text scene_name;
    /// <summary>
    /// 队伍显示文本框
    /// </summary>
    public Text troops_name;
    /// <summary>
    /// 是否按了下一步
    /// </summary>
    public bool isOnClick = false;
    /// <summary>
    /// 防误触时间
    /// </summary>
    public float btn_times;
    /// <summary>
    /// 是否按了键盘
    /// </summary>
    public bool isKeyCode;
    public Mode module_mode;
    public InputField step_inf;

    public int scene_num;
    public int step_num;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        next_btn.GetComponent<Image>().color = new Color(0.3f, 1, 0.3f);
        next_btn.onClick.AddListener(()=> {
            Dispose.Instance.Judge(gameObject, sceneState1s);
            isOnClick = true; });
        isStop = false;
        isKeyCode = true;
        step_inf.text = num.ToString();
    }

    void Update()
    {
        troops_name.text = (troops) + "队";
        if (isStop)
        {
            times += Time.deltaTime;
            next_btn.enabled = false;
            next_btn.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
            step_inf.enabled = false;
            if (times >= timer)
            {
                times = 0;
                isStop = false;
                isOnClick = false;
                Dispose.Instance.Judge(gameObject, sceneState1s);
                isKeyCode = true;
                next_btn.GetComponent<Image>().color = new Color(0.3f, 1, 0.3f);
                next_btn.enabled = true;
                step_inf.enabled = true;

            }
        }
        else if (isOnClick)
        {
            next_btn.enabled = false;
            step_inf.enabled = false;
            next_btn.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
            btn_times += Time.deltaTime;
            if (btn_times > 5)
            {
                btn_times = 0;
                next_btn.GetComponent<Image>().color = new Color(0.3f, 1, 0.3f);
                next_btn.enabled = true;
                step_inf.enabled = true;
                isOnClick = false;
                isKeyCode = true;
            }
        }
        else if ((Input.GetKeyDown((KeyCode)(48 + troops)) || Input.GetKeyDown((KeyCode)(256 + troops))) //判断是否按下了相应的数字键
            && isKeyCode //按键是否监听
            && !RedCastleControl.Instance._isSelect//判断输入框是否被按下 
            && RedCastleControl.Instance.mode != Mode.None//是否在主界面
            && module_mode== RedCastleControl.Instance.mode)  //当前显示界面是否与自身对应
        {
            isKeyCode = false;
            //NextState();
            Dispose.Instance.Judge(gameObject, sceneState1s);
            isOnClick = true;
        }
    }
    public void InputStep(string str)
    {
        num = int.Parse(str);
    }
}
