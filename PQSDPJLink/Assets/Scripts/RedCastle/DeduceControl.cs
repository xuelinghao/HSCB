using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeduceControl : MonoBehaviour
{
    public static DeduceControl Instance;

    public int troops_sum = 0;

    public GameObject obj;
    public Transform parent;
    public GameObject hint;
    public int current_num;
    public List<GameObject> gos;
    int nums = 0;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        gos = new List<GameObject>();
    }
    public void AddOnClick()
    {
        hint.SetActive(true);

        
    }
    public void Yes()
    {
        hint.SetActive(false);
        GameObject go = Instantiate(obj, parent);
        int count = parent.childCount;
        go.transform.SetSiblingIndex(count - 2);
        //创建模块
        //添加队伍
        troops_sum++;

        go.GetComponent<ModuleControl>().troops = troops_sum;
        go.GetComponent<ModuleControl>().scene_name.text = RedCastleControl.Instance.sceneState1s2[0].scene;
        go.GetComponent<ModuleControl>().num = current_num;
        go.GetComponent<ModuleControl>().sceneState1s = RedCastleControl.Instance.sceneState1s2;
        go.GetComponent<ModuleControl>().next_btn.GetComponent<Item>().name = RedCastleControl.Instance.sceneState1s2[0].phases[0].introduce;
        go.GetComponent<ModuleControl>().module_mode = Mode.Deduce;

        gos.Add(go);
    }
    public void No()
    {
        hint.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (RedCastleControl.Instance.mode != Mode.Deduce)
            return;

        for (int i = 0; i < gos.Count; i++)
        {
            if (gos[i].transform.Find("Step/step_input").GetComponent<StepInputFiled>()._isSelect)
            {
                RedCastleControl.Instance._isSelect = true;
            }
            else
            {
                nums++;
            }
        }
        if (nums == gos.Count)
        {
            RedCastleControl.Instance._isSelect = false;

        }

    }

    //public void Judge(int num)
    //{
    //    List<SceneState1> sceneState1s = new List<SceneState1>();
    //    record_dic.TryGetValue(num, out sceneState1s);
    //    int a = 0;
    //    step.TryGetValue(num, out a);
    //    int b = 0;
    //    ModuleControl moduleControl = new ModuleControl();
    //    module_dic.TryGetValue(num, out moduleControl);
    //    for (int i = 0; i < sceneState1s.Count; i++)
    //    {
    //        for (int j = 0; j < sceneState1s[i].phases.Count; j++)
    //        {
    //            if (b == a)
    //            {
    //                Debug.Log(sceneState1s[i].scene);
    //                for (int k = 0; k < sceneState1s[i].phases[j].LightState.Count; k++)
    //                {
    //                    LightStates lightStates = sceneState1s[i].phases[j].LightState[k];
    //                    StartCoroutine(Play0nClick(lightStates));
    //                }
    //                ///需要控制的播控盒列表
    //                for (int k = 0; k < sceneState1s[i].phases[j].MovieStates.Count; k++)
    //                {
    //                    MovieStates movieStates = sceneState1s[i].phases[j].MovieStates[k];
    //                    if (movieStates.movie_ip == "192.168.2.234")
    //                    {
    //                        StartCoroutine(Baozha(24));
    //                    }
    //                    if (movieStates.movie_file == "0000")
    //                    {
    //                        StartCoroutine(BoxVolumeSub(movieStates, movieStates.isCycle));
    //                    }
    //                    else
    //                    {
    //                        StartCoroutine(BoxVolumeAdd(movieStates, movieStates.isCycle));
    //                    }

    //                }
    //                ///需要控制的投影仪列表
    //                for (int k = 0; k < sceneState1s[i].phases[j].projectStates.Count; k++)
    //                {
    //                    ProjectStates projectStates = sceneState1s[i].phases[j].projectStates[k];
    //                    if (projectStates.project_name.Length > 3)
    //                    {
    //                        UDPServer.Instance.SocketSendIP3(projectStates.project_ip, 6581, strToToHexByte(projectStates.project_name));
    //                    }
    //                    else
    //                    {
    //                        if (projectStates.project_name == "000")
    //                        {
    //                            UDPServer.Instance.SocketSendIP2(projectStates.project_ip, 3012, "PauseProject");
    //                        }
    //                        else
    //                        {
    //                            UDPServer.Instance.SocketSendIP2(projectStates.project_ip, 3012, "PlayProject " + projectStates.project_name);
    //                        }
    //                    }
    //                }

    //                if (!sceneState1s[i].phases[j].isStop)
    //                {
    //                    moduleControl.times = 0;
    //                    moduleControl.timer = sceneState1s[i].phases[j].time;
    //                    moduleControl.isStop = true;
    //                }
    //                else
    //                {
    //                    moduleControl.times = 0;
    //                    moduleControl.isStop = false;
    //                }

    //                moduleControl.scene_name.text = sceneState1s[i].scene;
    //            }
    //            b++;
    //        }
    //    }
    //    a++;

    //    step.Remove(num);
    //    step.Add(num, a);
    //}

    ///// <summary>
    ///// 灯播放
    ///// </summary>
    ///// <param name="lightStates"></param>
    ///// <returns></returns>
    //public IEnumerator Play0nClick(LightStates lightStates)
    //{
    //    CycleTypeOnClick(lightStates);
    //    yield return new WaitForSeconds(0.2f);
    //    if (lightStates.light_file == "000")
    //        LightControl.Instance.Send(lightStates.light_port, lightStates.light_file, LigntType.Stop);
    //    else
    //        LightControl.Instance.Send(lightStates.light_port, lightStates.light_file, LigntType.Play);
    //}


    ///// <summary>
    ///// 灯循环方式
    ///// </summary>
    ///// <param name="lightStates"></param>
    //public void CycleTypeOnClick(LightStates lightStates)
    //{
    //    switch (lightStates.cycleType)
    //    {
    //        case CycleType.stop:
    //            LightControl.Instance.Send(lightStates.light_port, lightStates.light_file, LigntType.OneStop);
    //            break;
    //        case CycleType.one:
    //            LightControl.Instance.Send(lightStates.light_port, lightStates.light_file, LigntType.OneCycle);
    //            break;
    //    }
    //}

    ///// <summary>
    ///// 音量渐渐消失
    ///// </summary>
    ///// <param name="movieStates"></param>
    ///// <returns></returns>
    //public IEnumerator BoxVolumeSub(MovieStates movieStates, bool isCycle)
    //{
    //    int num = 15;
    //    while (num >= 0)
    //    {
    //        string str = "";
    //        if (num >= 10)
    //            str = num.ToString();

    //        else
    //            str = "0" + num.ToString();

    //        UDPServer.Instance.SocketSendIP(movieStates.movie_ip, movieStates.movie_ip + "VOL" + str + "END");
    //        yield return new WaitForSeconds(0.2f);
    //        num--;
    //    }

    //    UDPServer.Instance.SocketSendIP(movieStates.movie_ip, movieStates.movie_ip + "B" + movieStates.movie_file + "END");
    //    yield return new WaitForSeconds(0.5f);
    //}
    ///// <summary>
    ///// 音量渐渐变大
    ///// </summary>
    ///// <param name="movieStates"></param>
    ///// <param name="isCycle"></param>
    ///// <returns></returns>
    //public IEnumerator BoxVolumeAdd(MovieStates movieStates, bool isCycle)
    //{
    //    UDPServer.Instance.SocketSendIP(movieStates.movie_ip, movieStates.movie_ip + "VOL00END");
    //    int num = 1;

    //    yield return new WaitForSeconds(0.5f);
    //    UDPServer.Instance.SocketSendIP(movieStates.movie_ip, movieStates.movie_ip + "B" + movieStates.movie_file + "END");

    //    while (num <= 15)
    //    {
    //        string str = "";
    //        if (num >= 10) str = num.ToString();
    //        else
    //            str = "0" + num.ToString();
    //        yield return new WaitForSeconds(0.15f);
    //        UDPServer.Instance.SocketSendIP(movieStates.movie_ip, movieStates.movie_ip + "VOL" + str + "END");
    //        num++;
    //    }

    //}
    //private static byte[] strToToHexByte(string hexString)
    //{
    //    hexString = hexString.Replace(" ", "");
    //    if ((hexString.Length % 2) != 0)
    //        hexString += " ";
    //    byte[] returnBytes = new byte[hexString.Length / 2];
    //    for (int i = 0; i < returnBytes.Length; i++)
    //        returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
    //    return returnBytes;
    //}
    //public IEnumerator Baozha(float time)
    //{
    //    yield return new WaitForSeconds(time);
    //    UDPServer.Instance.SocketSendIP("192.168.2.234", "192.168.2.234B0102END");
    //}
}
