using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class FacilityControl : MonoBehaviour
{
    public static FacilityControl Instance;


    /// <summary>
    /// 场景模块父物体
    /// </summary>
    public Transform parent_scene;
    /// <summary>
    /// 场景模块
    /// </summary>
    public GameObject scene_prefab;


    /// <summary>
    /// 生成的小模块的父物体
    /// </summary>
    public Transform parent;
    /// <summary>
    /// 投影仪模块
    /// </summary>
    public GameObject projection_prefab;
    /// <summary>
    /// 播控盒模块
    /// </summary>
    public GameObject box_prefab;
    /// <summary>
    /// 灯控盒模块
    /// </summary>
    public GameObject light_prefab;
    /// <summary>
    /// 融合主机模块
    /// </summary>
    public GameObject fuse_prefab;


    public List<SceneData> sceneDatas;
    /// <summary>
    /// 播控盒集合
    /// </summary>
    Dictionary<string, List<BoxData>> box_dic;
    /// <summary>
    /// 灯控盒集合
    /// </summary>
    Dictionary<string, LightData> light_dic;
    /// <summary>
    /// 投影仪集合
    /// </summary>
    Dictionary<string, List<Projection>> projection_dic;
    /// <summary>
    /// 融合主机集合
    /// </summary>
    Dictionary<string, List<FuseData>> fuse_dic;

    SceneType sceneType = SceneType.None;

    /// <summary>
    /// 场景状态字典
    /// </summary>
    public Dictionary<string, SceneState> sceneState_dic;

    public string current_scene_name;
    private void Awake()
    {
        Instance = this;
    }
    public float projection_sum;
    private void Start()
    {
        sceneDatas = new List<SceneData>();
        light_dic = new Dictionary<string, LightData>();
        box_dic = new Dictionary<string, List<BoxData>>();
        projection_dic = new Dictionary<string, List<Projection>>();
        fuse_dic = new Dictionary<string, List<FuseData>>();
        sceneState_dic = new Dictionary<string, SceneState>();

        JsonData jsonData = JsonControl.Instance.ReadFromFile(Application.streamingAssetsPath + "/SceneData.json");
        if (jsonData != null)
            sceneDatas = JsonMapper.ToObject<List<SceneData>>(jsonData.ToJson());
        for (int i = 0; i < sceneDatas.Count; i++)
        {
            box_dic.Add(sceneDatas[i].name, sceneDatas[i].boxs);
            light_dic.Add(sceneDatas[i].name, sceneDatas[i].light);
            projection_dic.Add(sceneDatas[i].name, sceneDatas[i].projections);
            fuse_dic.Add(sceneDatas[i].name, sceneDatas[i].fuses);
            projection_sum += sceneDatas[i].projections.Count;
        }
    }
    private bool isOpen=false;
    private void Update()
    {
        if (sceneType == SceneType.particulars)
        {

        }
    }
    // Start is called before the first frame update
    public void Init()
    {
        CreatScene();
    }

    public void CreatScene()
    {
        if (parent_scene.childCount != 0)
        {
            for (int i = 0; i < parent_scene.childCount; i++)
            {
                Destroy(parent_scene.GetChild(i).gameObject);
            }
        }
        for (int i = 0; i < sceneDatas.Count; i++)
        {
            GameObject go = Instantiate(scene_prefab, parent_scene);
            string str = sceneDatas[i].name;
            go.transform.Find("name").GetComponent<Text>().text = str;
            go.transform.Find("name").GetComponent<Button>().onClick.AddListener(() => {
                SceneInit(str);
                
                RedCastleControl.Instance.return_btn.onClick.RemoveAllListeners();
                RedCastleControl.Instance.return_btn.onClick.AddListener(Return);
            });
        }
    }
    /// <summary>
    /// 场景初始化
    /// </summary>
    /// <param name="name"></param>
    public void SceneInit(string name)
    {
        current_scene_name = name;
        if (parent.childCount != 0)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Destroy(parent.GetChild(i).gameObject);
            }
        }
        if (!sceneState_dic.ContainsKey(name))
        {
            SceneState ss = new SceneState();
            ss.boxState = new List<BoxState>();
            ss.fuseState = new List<FuseState>();
            ss.lightState = new LightState();

            List<BoxData> boxDatas = new List<BoxData>();
            box_dic.TryGetValue(name, out boxDatas);
            for (int i = 0; i < boxDatas.Count; i++)
            {
                BoxState boxState = new BoxState();
                ss.boxState.Add(boxState);
            }

            List<FuseData> fuseDatas = new List<FuseData>();
            fuse_dic.TryGetValue(name, out fuseDatas);
            for (int i = 0; i < fuseDatas.Count; i++)
            {
                FuseState fuseState = new FuseState();
                ss.fuseState.Add(fuseState);
            }
            sceneState_dic.Add(name, ss);
        }
        CreateBox(name);
        CreateLight(name);
        CreateFuse(name);
        CreateProjection(name);
        int a = parent.childCount % 4;
        int b = parent.childCount / 4;
        if (a != 0)
        {
            b++;
        }
        parent.GetComponent<RectTransform>().sizeDelta = new Vector2(1700, 320 * b);
        sceneType = SceneType.particulars;
    }

    /// <summary>
    /// 生成播控盒
    /// </summary>
    /// <param name="name"></param>
    public void CreateBox(string name)
    {
        List<BoxData> boxDatas = new List<BoxData>();
        box_dic.TryGetValue(name, out boxDatas);
        SceneState ss = new SceneState();
        sceneState_dic.TryGetValue(name, out ss);
        for (int i = 0; i < boxDatas.Count; i++)
        {
            GameObject go = Instantiate(box_prefab, parent);
            go.GetComponent<Box>().ip = boxDatas[i].ip;
            go.GetComponent<Box>().program = boxDatas[i].program;
            go.GetComponent<Box>().id = boxDatas[i].id;
            go.GetComponent<Box>().scene = name;

            go.GetComponent<Box>().num = ss.boxState[i].num;
            go.GetComponent<Box>().volume_num = ss.boxState[i].volume_num;
            go.GetComponent<Box>().isStart = ss.boxState[i].isStart;
            go.GetComponent<Box>().isVolume = ss.boxState[i].isVolume;
            go.GetComponent<Box>().isPlay = ss.boxState[i].isPlay;
            go.name = i.ToString();
            go.GetComponent<Box>().Init();
        }
    }

    /// <summary>
    /// 生成灯控盒
    /// </summary>
    /// <param name="name"></param>
    public void CreateLight(string name)
    {
        LightData lightData = new LightData();
        light_dic.TryGetValue(name, out lightData);
        GameObject go = Instantiate(light_prefab, parent);
        go.GetComponent<Light>().program = lightData.files;
        go.GetComponent<Light>().addr = lightData.addr;
        go.GetComponent<Light>().id = lightData.id;
        go.GetComponent<Light>().scene = name;

        SceneState ss = new SceneState();
        sceneState_dic.TryGetValue(name, out ss);
        go.GetComponent<Light>().curremt_num = ss.lightState.num;
        go.GetComponent<Light>().isPlay = ss.lightState.isPlay;
        go.GetComponent<Light>().Init();
    }

    /// <summary>
    /// 生成投影仪
    /// </summary>
    /// <param name="name"></param>
    public void CreateProjection(string name)
    {

        int num = 0;

        for (int i = 0; i < sceneDatas.Count; i++)
        {
            if (sceneDatas[i].name == name)
            {
                num = i;
            }
        }

        GameObject go = Instantiate(projection_prefab, parent);
        go.transform.Find("Open").GetComponent<Button>().onClick.AddListener(() => {
            isOpen = true;
            RedCastleControl.Instance.OpenSceneProject(num);
        });

        go.transform.Find("Close").GetComponent<Button>().onClick.AddListener(() => {
            RedCastleControl.Instance.CloseSceneProject(num);

        });

        go.transform.Find("OpenALL").GetComponent<Button>().onClick.AddListener(() => {
            RedCastleControl.Instance.OpenProject();
        });

        go.transform.Find("CloseALL").GetComponent<Button>().onClick.AddListener(() => {
            RedCastleControl.Instance.CloseProject();
        });
    }
    /// <summary>
    /// 生成融合模块
    /// </summary>
    /// <param name="name"></param>
    public void CreateFuse(string name)
    {
        List<FuseData> fuseDatas = new List<FuseData>();
        fuse_dic.TryGetValue(name, out fuseDatas);

        SceneState ss = new SceneState();
        sceneState_dic.TryGetValue(name, out ss);


        for (int i = 0; i < fuseDatas.Count; i++)
        {
            GameObject go = Instantiate(fuse_prefab, parent);
            go.GetComponent<Fuse>().ip = fuseDatas[i].ip;
            go.GetComponent<Fuse>().program = fuseDatas[i].program;
            go.GetComponent<Fuse>().id = fuseDatas[i].id;
            go.GetComponent<Fuse>().scene = name;


            go.GetComponent<Fuse>().num = ss.fuseState[i].num;
            go.GetComponent<Fuse>().volume_num = ss.fuseState[i].volume_num;
            go.GetComponent<Fuse>().isVolume = ss.fuseState[i].isVolume;
            go.GetComponent<Fuse>().isPlay = ss.fuseState[i].isPlay;
            go.name = i.ToString();
            go.GetComponent<Fuse>().FuseDatas = fuseDatas;

            go.GetComponent<Fuse>().Init();

        }
    }

    /// <summary>
    /// 返回上一级
    /// </summary>
    public void Return()
    {
        SetState();
        switch (sceneType)
        {
            case SceneType.None:
                ReturnMain();
                break;
            case SceneType.particulars:
                sceneType = SceneType.None;
                CreatScene();
                RedCastleControl.Instance.return_btn.onClick.RemoveAllListeners();
                RedCastleControl.Instance.return_btn.onClick.AddListener(RedCastleControl.Instance.ReturnOnClick);

                break;
        }
    }
    public void SetState()
    {
        SceneState ss = new SceneState();
        if (sceneState_dic.ContainsKey(current_scene_name))
        {
            sceneState_dic.TryGetValue(current_scene_name, out ss);

        }

        for (int i = 0; i < parent.childCount; i++)
        {
            Fuse fuse = parent.GetChild(i).GetComponent<Fuse>();
            Box box = parent.GetChild(i).GetComponent<Box>();
            Light light = parent.GetChild(i).GetComponent<Light>();
            if (fuse != null)
            {
                int a = int.Parse(parent.GetChild(i).name);
                ss.fuseState[a].num = fuse.num;
                ss.fuseState[a].volume_num = fuse.volume_num;
                ss.fuseState[a].isPlay = fuse.isPlay;
                ss.fuseState[a].isVolume = fuse.isVolume;

            }
            if (box != null)
            {
                int a = int.Parse(parent.GetChild(i).name);
                ss.boxState[a].num = box.num;
                ss.boxState[a].volume_num = box.volume_num;
                ss.boxState[a].isPlay = box.isPlay;
                ss.boxState[a].isVolume = box.isVolume;
                ss.boxState[a].isStart = box.isStart;

            }
            if (light != null)
            {
                ss.lightState.num = light.curremt_num;
                ss.lightState.isPlay = light.isPlay;
            }
        }


    }
    /// <summary>
    /// 返回主界面
    /// </summary>
    public void ReturnMain()
    {
        SetState();
        for (int i = 0; i < parent_scene.childCount; i++)
        {
            Destroy(parent_scene.GetChild(i).gameObject);
        }
    }

    public void UpdateData()
    {
        UDPServer.Instance.SocketSendIP2("192.168.2.17", 2701, "update");
    }
}
