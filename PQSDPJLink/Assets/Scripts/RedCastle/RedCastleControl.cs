using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class RedCastleControl : MonoBehaviour
{
    public static RedCastleControl Instance;

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    const int SW_SHOWMINIMIZED = 2; //{最小化, 激活}
    const int SW_SHOWMAXIMIZED = 3;//最大化
    const int SW_SHOWRESTORE = 1;//还原

    public Mode mode = Mode.None;
    public Mode one_mode = Mode.None;

    /// <summary>
    /// 主界面
    /// </summary>
    public GameObject main;
    /// <summary>
    /// 演绎界面
    /// </summary>
    public GameObject deduce;
    /// <summary>
    /// 单场景演绎界面
    /// </summary>
    public GameObject scenededuce;//
    /// <summary>
    /// 浏览界面
    /// </summary>
    public GameObject visit;//
    /// <summary>
    /// 单场景浏览界面
    /// </summary>
    public GameObject facility;//
    
    /// <summary>
    /// 返回图标烟雾图标父物体
    /// </summary>
    public GameObject return_obj;
    /// <summary>
    /// 返回按钮
    /// </summary>
    public Button return_btn;
    /// <summary>
    /// 返回主场景按钮
    /// </summary>
    public Button user_btn;
    /// <summary>
    /// 浏览模式状态集合
    /// </summary>
    public List<SceneState1> sceneState1s1;
    /// <summary>
    /// 演绎模式状态集合
    /// </summary>
    public List<SceneState1> sceneState1s2;
    /// <summary>
    /// 还原状态集合
    /// </summary>
    public List<SceneState1> resetState1s;
   
    public bool _isSelect = false;
    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        DownloadControl.Instance.Init();
        LoadJosn();
        //HideInterface();
        fail_project = new Dictionary<string,string>();
        fail_scene = new List<string>();
        return_btn.onClick.AddListener(ReturnOnClick);
        user_btn.onClick.AddListener(LogoutOnClick);
        isOpen = false;
        isClose = false;
        isOneOpen = false;
        isOneClose = false;

        DownLoad();
    }
    public GameObject dialog;
    /// <summary>
    /// 账号密码保存路径
    /// </summary>
    public string userPath = @"D:\HSCB\User";

    /// <summary>
    /// 账号密码保存名字
    /// </summary>
    public string userFile = "User.json";

    /// <summary>
    /// 下载文件保存路径
    /// </summary>
    public string mediaPath = @"D:\HSCB\Media";

    /// <summary>
    /// 文件名字
    /// </summary>
    public string meidaFile = "FileData.json";
    /// <summary>
    /// 媒体文件集合
    /// </summary>
    public List<FileData> fileDatas;
    public void DownLoadOnClick()
    {
        mode = Mode.None;
        dialog.SetActive(true);
        dialog.transform.SetAsLastSibling();
    }
    public void YesOnClick()
    {
        DeleteAllFile(mediaPath);

        DownloadControl.Instance.UpdateMeida();
        dialog.SetActive(false);

    }
    public void NoOnClick()
    {
        dialog.SetActive(false);
    }
    /// <summary>
    /// 下载账号密码文件
    /// 查看版本
    /// </summary>
    public void DownLoad()
    {
        //判断文件夹是否存在
        if (!Directory.Exists(userPath))
            //文件夹不存在则创建该文件夹
            Directory.CreateDirectory(userPath);
        
        if (!File.Exists(userPath + @"\" + userFile))
        {
            DownloadControl.Instance.DownLoadUser(userPath,userFile);
        }
        else
        {
            DownloadControl.Instance.ReadUser(userPath + @"\" + userFile);
            DownloadControl.Instance.UpdateUser(Application.dataPath,userFile);
        }

        if (!Directory.Exists(mediaPath))
            //文件夹不存在则创建该文件夹
            Directory.CreateDirectory(mediaPath);
        if (!File.Exists(mediaPath + @"\" + meidaFile))
            DownloadControl.Instance.DownLoadMedia(mediaPath, meidaFile);
        else
        {
            string strt = File.ReadAllText(mediaPath + @"\" + meidaFile);
            JsonData jsonData1 = JsonMapper.ToObject(strt);
            if (jsonData1 != null)
                fileDatas = JsonMapper.ToObject<List<FileData>>(jsonData1.ToJson());
            Debug.Log(jsonData1);
        }

    }

    /// <summary>
    /// 更新媒体文件
    /// </summary>
    public void UpdateMedia()
    {
        DeleteAllFile(mediaPath);
        DownloadControl.Instance.DownLoadMedia(mediaPath, meidaFile);
    }

    /// <summary>
    /// 删除指定文件目录下的所有文件
    /// </summary>
    /// <param name="fullPath">文件路径</param>
    public bool DeleteAllFile(string fullPath)
    {
        //获取指定路径下面的所有资源文件  然后进行删除
        if (Directory.Exists(fullPath))
        {
            DirectoryInfo direction = new DirectoryInfo(fullPath);
            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

            Debug.Log(files.Length);

            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.EndsWith(".meta"))
                {
                    continue;
                }
                string FilePath = fullPath + "/" + files[i].Name;
                print(FilePath);
                File.Delete(FilePath);
            }
            return true;
        }
        return false;
    }


    /// <summary>
    /// 加载Json文件
    /// </summary>
    public void LoadJosn()
    {
        JsonData jsonData1 = JsonControl.Instance.ReadFromFile(Application.streamingAssetsPath + "/SceneState1.json");
        JsonData jsonData2 = JsonControl.Instance.ReadFromFile(Application.streamingAssetsPath + "/SceneState2.json");
        JsonData jsonData4 = JsonControl.Instance.ReadFromFile(Application.streamingAssetsPath + "/Reset.json");
        if (jsonData1 != null)
            sceneState1s1 = JsonMapper.ToObject<List<SceneState1>>(jsonData1.ToJson());
        if (jsonData2 != null)
            sceneState1s2 = JsonMapper.ToObject<List<SceneState1>>(jsonData2.ToJson());
        if (jsonData4 != null)
            resetState1s = JsonMapper.ToObject<List<SceneState1>>(jsonData4.ToJson());
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))///esc小化程序
        {
            ShowWindow(GetForegroundWindow(), SW_SHOWMINIMIZED);
        }

        if (mode!=Mode.None)
        {
            return_obj.transform.SetAsLastSibling();
        }

        #region 投影仪模块
        if (isOpen|| isClose||isOneClose||isOneOpen)
        {
            fail_dialog.transform.Find("Dialog").GetComponent<Text>().text = "";
            if (ProjectionControl.Instance.number != sum)
            {
                progress_bar.value = ProjectionControl.Instance.number / sum;
                progress_text.text = (int)(progress_bar.value * 100) + "%";
            }
            else
            {
                ProjectionControl.Instance.number = 0;
                progress_text.text =  "100%";
                fail_dialog.SetActive(true);
                progress.transform.Find("Start").gameObject.SetActive(true);
                progress_bar.value = 1;
                foreach (var item in fail_project)
                {
                    if (!fail_scene.Contains(item.Value)) 
                    {
                        fail_scene.Add(item.Value);
                    }
                }
                for (int i = 0; i < fail_scene.Count; i++)
                {
                    if (isOpen || isOneOpen) 
                        fail_dialog.transform.Find("Dialog").GetComponent<Text>().text += fail_scene[i] + "场景未全部开启";
                    else if (isClose || isOneClose)
                        fail_dialog.transform.Find("Dialog").GetComponent<Text>().text += fail_scene[i] + "场景未全部关闭";
                    fail_dialog.transform.Find("Dialog").GetComponent<Text>().text += "\n";
                }
                if (fail_scene.Count==0)
                {
                    if (isOpen) 
                        fail_dialog.transform.Find("Dialog").GetComponent<Text>().text = "所有场景已全部开启";
                    else if(isClose)
                        fail_dialog.transform.Find("Dialog").GetComponent<Text>().text = "所有场景已全部关闭";
                    else if(isOneOpen)
                        fail_dialog.transform.Find("Dialog").GetComponent<Text>().text = FacilityControl.Instance.sceneDatas[scene_num].name + "场景已全部开启";
                    else if (isOneClose)
                        fail_dialog.transform.Find("Dialog").GetComponent<Text>().text = FacilityControl.Instance.sceneDatas[scene_num].name + "场景已全部关闭";
                }
                isOpen = false;
                isClose = false;
                isOneClose = false;
                isOneOpen = false;

            }
        }
        #endregion

        
    }

    #region 投影仪模块

    /// <summary>
    /// 投影仪进度条界面
    /// </summary>
    public GameObject progress;

    /// <summary>
    /// 错误投影仪IP、场景字典
    /// </summary>
    public Dictionary<string, string> fail_project;

    /// <summary>
    /// 错误场景集合
    /// </summary>
    private List<string> fail_scene;

    /// <summary>
    /// 投影仪打开状态提示框
    /// </summary>
    public GameObject fail_dialog;

    /// <summary>
    /// 进度条
    /// </summary>
    public Slider progress_bar;

    /// <summary>
    /// 进度百分比
    /// </summary>
    public Text progress_text;

    /// <summary>
    /// 是否全开投影仪
    /// </summary>
    public bool isOpen;

    /// <summary>
    /// 是否全关投影仪
    /// </summary>
    public bool isClose;

    /// <summary>
    /// 是否开单个场景投影仪
    /// </summary>
    public bool isOneOpen;

    /// <summary>
    /// 是否关单个投影仪
    /// </summary>
    public bool isOneClose;

    /// <summary>
    /// 需要打开或关闭单个场景投影仪的场景编号
    /// </summary>
    int scene_num;

    /// <summary>
    /// 要开的投影仪数量
    /// </summary>
    float sum = 0;

    /// <summary>
    /// 打开全部投影仪
    /// </summary>
    public void OpenProject()
    {
        ProjectionControl.Instance.number = 0;
        sum = FacilityControl.Instance.projection_sum;
        Progress();
        progress.transform.Find("Text").GetComponent<Text>().text= "投影仪正在开启......";
        isOpen = true;
        ProjectionControl.Instance.Projection(0, ProjectionType.ALL, rv.PowerCommand.Power.ON);
    }


    /// <summary>
    /// 关闭全部投影仪
    /// </summary>
    public void CloseProject()
    {
        ProjectionControl.Instance.number = 0;
        sum = FacilityControl.Instance.projection_sum;
        Progress();
        isClose = true;
        progress.transform.Find("Text").GetComponent<Text>().text= "投影仪正在关闭......";
        ProjectionControl.Instance.Projection(0, ProjectionType.ALL, rv.PowerCommand.Power.OFF);
    }

    
    /// <summary>
    /// 打开单场景投影仪
    /// </summary>
    public void OpenSceneProject(int num)
    {
        Progress();
        ProjectionControl.Instance.number = 0;
        scene_num = num;
        sum = FacilityControl.Instance.sceneDatas[num].projections.Count;
        isOneOpen = true;
        progress.transform.Find("Text").GetComponent<Text>().text = "投影仪正在开启......";
        ProjectionControl.Instance.Projection(num, ProjectionType.ONE, rv.PowerCommand.Power.ON);
    }


    /// <summary>
    /// 关闭单场景投影仪
    /// </summary>
    public void CloseSceneProject(int num)
    {
        Progress();
        ProjectionControl.Instance.number = 0;
        sum = FacilityControl.Instance.sceneDatas[num].projections.Count;
        scene_num = num;
        isOneClose = true;
        progress.transform.Find("Text").GetComponent<Text>().text = "投影仪正在关闭......";
        ProjectionControl.Instance.Projection(num, ProjectionType.ONE, rv.PowerCommand.Power.OFF);
    }


    /// <summary>
    /// 投影仪进度条界面
    /// </summary>
    public void Progress()
    {
        fail_scene.Clear();
        fail_project.Clear();
        progress.SetActive(true);
        progress.transform.SetAsLastSibling();
        progress.transform.Find("Start").gameObject.SetActive(false);
    }
    #endregion


    /// <summary>
    /// 演绎模式界面
    /// </summary>
    public void DeduceOnClick()
    {
        mode = Mode.Deduce;
        deduce.transform.SetSiblingIndex(6);
    }
    /// <summary>
    /// 单场景演绎模式界面
    /// </summary>
    public void SceneDeduceOnClick()
    {
        scenededuce.transform.SetSiblingIndex(6);
        Dispose.Instance.CreatScene(sceneState1s2, Mode.SceneDeduce);
    }
    /// <summary>
    /// 浏览模式界面
    /// </summary>
    public void VisitOnClick()
    {
        mode = Mode.Visit;
        visit.transform.SetSiblingIndex(6);
    }
    /// <summary>
    /// 单场景浏览模式界面
    /// </summary>
    public void SceneVisitOnClick()
    {
        scenededuce.transform.SetSiblingIndex(6);
        Dispose.Instance.CreatScene(sceneState1s1, Mode.SceneVisit);
    }
    /// <summary>
    /// 关闭界面
    /// </summary>
    public void MainInterface()
    {
        progress.SetActive(false);
        main.SetActive(true);
        main.transform.SetSiblingIndex(6);

    }
    
    /// <summary>
    /// 设备模式界面
    /// </summary>
    public void FacilityOnClick()
    {
        mode = Mode.Facility;
        facility.SetActive(true);
        facility.transform.SetSiblingIndex(6);
        FacilityControl.Instance.Init();
    }

    /// <summary>
    /// 返回按钮
    /// </summary>
    public void ReturnOnClick()
    {
        mode = Mode.None;
        main.transform.SetSiblingIndex(6);
    }
    /// <summary>
    /// 注销按钮
    /// </summary>
    public void LogoutOnClick()
    {
        mode = Mode.None;
        transform.GetComponent<RegisterControl>().register.transform.SetAsLastSibling();
        transform.GetComponent<RegisterControl>().register.SetActive(true);

    }
    /// <summary>
    /// 隐藏二级界面
    /// </summary>
    public void HideInterface()
    {
        deduce.SetActive(false);
        scenededuce.SetActive(false);
        visit.SetActive(false);
        facility.SetActive(false);
    }
    public bool isJueDu = false;
    public void JueDu()
    {
        if (!isJueDu)
        {
            LightControl.Instance.Send("04", "001", LigntType.Play);
            return_obj.transform.Find("Smoke/JueDu/state").GetComponent<Image>().color = new Color(0, 1, 0);
            isJueDu = true;
        }
        else
        {
            LightControl.Instance.Send("04", "001", LigntType.Stop);
            return_obj.transform.Find("Smoke/JueDu/state").GetComponent<Image>().color = new Color(1, 0, 0);
            isJueDu = false;
        }
    }
    public bool isWeiYa = false;
    public void WeiYa()
    {
       
        if (!isWeiYa)
        {
            LightControl.Instance.Send("06", "001", LigntType.Play);
            return_obj.transform.Find("Smoke/WeiYa/state").GetComponent<Image>().color = new Color(0, 1, 0);
            isWeiYa = true;
        }
        else
        {
            LightControl.Instance.Send("06", "001", LigntType.Stop);
            return_obj.transform.Find("Smoke/WeiYa/state").GetComponent<Image>().color = new Color(1, 0, 0);
            isWeiYa = false;
        }
    }
    public bool isHeiAn = false;
    public void HeiAn()
    {
        if (!isHeiAn)
        {
            LightControl.Instance.Send("07", "001", LigntType.Play);
            return_obj.transform.Find("Smoke/HeiAn/state").GetComponent<Image>().color = new Color(0, 1, 0);
            isHeiAn = true;
        }
        else
        {
            LightControl.Instance.Send("07", "001", LigntType.Stop);
            return_obj.transform.Find("Smoke/HeiAn/state").GetComponent<Image>().color = new Color(1, 0, 0);
            isHeiAn = false;
        }
        
    }
}
public enum Mode
{
    None,
    Deduce,
    Visit,
    SceneDeduce,
    SceneVisit,
    Facility
}
public class AAA
{
    public string name;
    public float aaa;
}