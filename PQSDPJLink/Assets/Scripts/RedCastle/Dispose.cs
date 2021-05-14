using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dispose : MonoBehaviour
{
    public static Dispose Instance;

    public GameObject scene_prefab;
    public Transform parent_scene;
    public Mode one_mode=Mode.None;
    private void Awake()
    {
        Instance = this;
    }
    public void CreatScene(List<SceneState1> sceneState1s1,Mode mode)
    {
        if (mode!=one_mode)
        {
            if (parent_scene.childCount != 0)
            {
                for (int i = 0; i < parent_scene.childCount; i++)
                {
                    Destroy(parent_scene.GetChild(i).gameObject);
                }
            }
            for (int i = 0; i < sceneState1s1.Count; i++)
            {
                GameObject go = Instantiate(scene_prefab, parent_scene);
                string str = sceneState1s1[i].scene;
                SceneState1 sceneState1 = sceneState1s1[i];
                //scene_dic.Add(go, sceneState1);
                go.GetComponent<SceneDeduce>().sceneState1 = sceneState1;
                go.GetComponent<SceneDeduce>().resetState1 = RedCastleControl.Instance.resetState1s[i];
                go.GetComponent<SceneDeduce>().next_btn.GetComponent<Item>().name = sceneState1s1[i].phases[0].introduce;

                go.GetComponent<SceneDeduce>().Init();
            }
            one_mode = mode;
        }
    }
    private void Update()
    {
        int a = parent_scene.childCount % 4;
        int b = parent_scene.childCount / 4;
        if (a != 0)
        {
            b++;
        }
        parent_scene.GetComponent<RectTransform>().sizeDelta = new Vector2(1700, 320 * b);
    }
    /// <summary>
    /// 重置
    /// </summary>
    public void Reset(GameObject go, SceneState1 sceneState1)
    {
        if (go.GetComponent<SceneDeduce>().num!=0)
        {
            if (sceneState1.scene=="飞跃")
            {
                UDPServer.Instance.SocketSendIP3("192.168.2.85", 6581, Dispose.strToToHexByte("EB BE 00 01 00 00 00 00 00 00 00"));
                UDPServer.Instance.SocketSendIP3("192.168.2.85", 6581, Dispose.strToToHexByte("EB BE 00 04 01 00 09 00 00 00 00"));
            }
            go.GetComponent<SceneDeduce>().num = 0;

            OneJudge(go, sceneState1);
        }
    }
    /// <summary>
    /// 单场景演绎
    /// </summary>
    /// <param name="go"></param>
    /// <param name="sceneState1"></param>
    public void OneJudge(GameObject go, SceneState1 sceneState1)
    {
        int num = go.GetComponent<SceneDeduce>().num;
        if (num > (sceneState1.phases.Count - 1))
        {
            return;
        }

        for (int k = 0; k < sceneState1.phases[num].LightState.Count; k++)
        {
            LightStates lightStates = sceneState1.phases[num].LightState[k];
            StartCoroutine(Play0nClick(lightStates));
        }
        ///需要控制的播控盒列表
        for (int k = 0; k < sceneState1.phases[num].MovieStates.Count; k++)
        {
            MovieStates movieStates = sceneState1.phases[num].MovieStates[k];
            
            if (movieStates.movie_file == "0000")
            {
                StartCoroutine(BoxVolumeSub(movieStates, movieStates.isCycle));
            }
            else
            {
                StartCoroutine(BoxVolumeAdd(movieStates, movieStates.isCycle));
                if (movieStates.movie_ip == "192.168.2.234")
                {
                    StartCoroutine(Baozha(24));
                }
            }

        }
        float yizi = 0;

        ///需要控制的投影仪列表
        for (int k = 0; k < sceneState1.phases[num].projectStates.Count; k++)
        {
            ProjectStates projectStates = sceneState1.phases[num].projectStates[k];
            if (projectStates.project_name.Length > 3)
            {
                UDPServer.Instance.SocketSendIP3(projectStates.project_ip, 6581, strToToHexByte(projectStates.project_name));
                yizi = 4.5f;
            }
            else
            {
                if (projectStates.project_name == "000")
                {
                    UDPServer.Instance.SocketSendIP2(projectStates.project_ip, 3012, "PauseProject");
                }
                else
                {
                    UDPServer.Instance.SocketSendIP2(projectStates.project_ip, 3012, "PlayProject " + projectStates.project_name);
                }
            }
        }

        if (num < sceneState1.phases.Count - 1) 
        {
            go.transform.Find("Next").GetComponent<Item>().name = sceneState1.phases[num + 1].introduce;
        }

        if (!sceneState1.phases[num].isStop)
        {
            go.GetComponent<SceneDeduce>().times = 0;
            if (yizi != 0)
                go.GetComponent<SceneDeduce>().timer = yizi;
            else
                go.GetComponent<SceneDeduce>().timer = sceneState1.phases[num].time;
            go.GetComponent<SceneDeduce>().isStop = true;
        }
        else
        {
            go.GetComponent<SceneDeduce>().times = 0;
            go.GetComponent<SceneDeduce>().isStop = false;
        }
        num++;
        go.GetComponent<SceneDeduce>().num = num;
    }
    /// <summary>
    /// 全场景演绎浏览
    /// </summary>
    /// <param name="go"></param>
    /// <param name="sceneState1s"></param>
    public void Judge(GameObject go, List<SceneState1> sceneState1s)
    {
        bool isFinnish = true;
        int a = 0;
        if (go.GetComponent<ModuleControl>().step_inf.text=="")
            a = go.GetComponent<ModuleControl>().num;
        else
            a = int.Parse(go.GetComponent<ModuleControl>().step_inf.text);
        if (a==0)
        {
            UDPServer.Instance.SocketSendIP3("192.168.2.85", 6581, Dispose.strToToHexByte("EB BE 00 01 00 00 00 00 00 00 00"));
            UDPServer.Instance.SocketSendIP3("192.168.2.85", 6581, Dispose.strToToHexByte("EB BE 00 04 01 00 09 00 00 00 00"));
        }
        #region

        int b = 0;
        int scene_num = 0;
        int step_num = 0;

        for (int i = 0; i < sceneState1s.Count; i++)
        {
            for (int j = 0; j < sceneState1s[i].phases.Count; j++)
            {
                if (b == a)
                {
                    if (j == sceneState1s[i].phases.Count - 1 && i != sceneState1s.Count - 1) 
                    {
                        scene_num = i + 1;
                        step_num = 0;
                    }
                    else if (j == sceneState1s[i].phases.Count - 1 && i == sceneState1s.Count - 1)
                    {
                        scene_num = i;
                        step_num = j;
                    }
                    else
                    {
                        scene_num = i;
                        step_num = j + 1;
                    }

                    isFinnish = false;
                    for (int k = 0; k < sceneState1s[i].phases[j].LightState.Count; k++)
                    {
                        LightStates lightStates = sceneState1s[i].phases[j].LightState[k];
                        StartCoroutine(Play0nClick(lightStates));
                    }
                    ///需要控制的播控盒列表
                    for (int k = 0; k < sceneState1s[i].phases[j].MovieStates.Count; k++)
                    {
                        MovieStates movieStates = sceneState1s[i].phases[j].MovieStates[k];

                        if (movieStates.movie_file == "0000")
                        {
                            StartCoroutine(BoxVolumeSub(movieStates, movieStates.isCycle));
                        }
                        else
                        {

                            StartCoroutine(BoxVolumeAdd(movieStates, movieStates.isCycle));
                            if (movieStates.movie_ip == "192.168.2.234")
                            {
                                StartCoroutine(Baozha(24));
                            }
                        }

                    }
                    float yizi = 0;
                    ///需要控制的投影仪列表
                    for (int k = 0; k < sceneState1s[i].phases[j].projectStates.Count; k++)
                    {
                        ProjectStates projectStates = sceneState1s[i].phases[j].projectStates[k];
                        if (projectStates.project_name.Length > 3)
                        {
                            UDPServer.Instance.SocketSendIP3(projectStates.project_ip, 6581, strToToHexByte(projectStates.project_name));
                            yizi = 4.5f;
                        }
                        else
                        {
                            if (projectStates.project_name == "000")
                            {
                                UDPServer.Instance.SocketSendIP2(projectStates.project_ip, 3012, "PauseProject");
                            }
                            else
                            {
                                UDPServer.Instance.SocketSendIP2(projectStates.project_ip, 3012, "PlayProject " + projectStates.project_name);
                            }
                        }
                    }

                    if (!sceneState1s[i].phases[j].isStop)
                    {
                        go.GetComponent<ModuleControl>().times = 0;
                        if (yizi != 0)
                            go.GetComponent<ModuleControl>().timer = yizi;
                        else
                            go.GetComponent<ModuleControl>().timer = sceneState1s[i].phases[j].time;
                        go.GetComponent<ModuleControl>().isStop = true;
                    }
                    else
                    {
                        go.GetComponent<ModuleControl>().times = 0;
                        go.GetComponent<ModuleControl>().isStop = false;
                    }

                    go.GetComponent<ModuleControl>().scene_name.text = sceneState1s[i].scene;
                }
                b++;
            }

        }
        #endregion
        go.GetComponent<ModuleControl>().next_btn.GetComponent<Item>().name = sceneState1s[scene_num].phases[step_num].introduce;
        a++;
        if (isFinnish)
        {
            go.GetComponent<ModuleControl>().num = 0;
            go.GetComponent<ModuleControl>().scene_name.text = "已经结束";
        }
        else
        {
            go.GetComponent<ModuleControl>().step_inf.text = a.ToString();
            go.GetComponent<ModuleControl>().num = a;

        }
    }

    #region 灯控盒

    /// <summary>
    /// 灯播放
    /// </summary>
    /// <param name="lightStates"></param>
    /// <returns></returns>
    public IEnumerator Play0nClick(LightStates lightStates)
    {
        CycleTypeOnClick(lightStates);
        yield return new WaitForSeconds(0.2f);
        if (lightStates.light_file == "000")
            LightControl.Instance.Send(lightStates.light_port, lightStates.light_file, LigntType.Stop);
        else
            LightControl.Instance.Send(lightStates.light_port, lightStates.light_file, LigntType.Play);
    }


    /// <summary>
    /// 灯循环方式
    /// </summary>
    /// <param name="lightStates"></param>
    public void CycleTypeOnClick(LightStates lightStates)
    {
        switch (lightStates.cycleType)
        {
            case CycleType.stop:
                LightControl.Instance.Send(lightStates.light_port, lightStates.light_file, LigntType.OneStop);
                break;
            case CycleType.one:
                LightControl.Instance.Send(lightStates.light_port, lightStates.light_file, LigntType.OneCycle);
                break;
        }
    }
    #endregion


    public static byte[] strToToHexByte(string hexString)
    {
        hexString = hexString.Replace(" ", "");
        if ((hexString.Length % 2) != 0)
            hexString += " ";
        byte[] returnBytes = new byte[hexString.Length / 2];
        for (int i = 0; i < returnBytes.Length; i++)
            returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
        return returnBytes;
    }

    #region 播控盒
    /// <summary>
    /// 音量渐渐消失
    /// </summary>
    /// <param name="movieStates"></param>
    /// <returns></returns>
    public IEnumerator BoxVolumeSub(MovieStates movieStates, bool isCycle)
    {
        if (movieStates.movie_ip == "192.168.2.69")
        {
            if (isCycle)
                UDPServer.Instance.SocketSendIP(movieStates.movie_ip, movieStates.movie_ip + "B" + movieStates.movie_file + "END");
            else
                UDPServer.Instance.SocketSendIP(movieStates.movie_ip, movieStates.movie_ip + "K" + movieStates.movie_file + "END");
            
        }
        else
        {
            int num = 15;
            while (num >= 0)
            {
                string str = "";
                if (num >= 10)
                    str = num.ToString();
                else
                    str = "0" + num.ToString();

                UDPServer.Instance.SocketSendIP(movieStates.movie_ip, movieStates.movie_ip + "VOL" + str + "END");
                yield return new WaitForSeconds(0.2f);
                num--;
            }
            if (isCycle)
                UDPServer.Instance.SocketSendIP(movieStates.movie_ip, movieStates.movie_ip + "B" + movieStates.movie_file + "END");
            else
                UDPServer.Instance.SocketSendIP(movieStates.movie_ip, movieStates.movie_ip + "K" + movieStates.movie_file + "END");
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(0.1f);

    }
    /// <summary>
    /// 音量渐渐变大
    /// </summary>
    /// <param name="movieStates"></param>
    /// <param name="isCycle"></param>
    /// <returns></returns>
    public IEnumerator BoxVolumeAdd(MovieStates movieStates, bool isCycle)
    {
        if (movieStates.movie_ip=="192.168.2.69")
        {
            UDPServer.Instance.SocketSendIP(movieStates.movie_ip, movieStates.movie_ip + "VOL15END");
            yield return new WaitForSeconds(0.1f);
            if (isCycle)
                UDPServer.Instance.SocketSendIP(movieStates.movie_ip, movieStates.movie_ip + "B" + movieStates.movie_file + "END");
            else
                UDPServer.Instance.SocketSendIP(movieStates.movie_ip, movieStates.movie_ip + "K" + movieStates.movie_file + "END");
            
        }
        else
        {
            UDPServer.Instance.SocketSendIP(movieStates.movie_ip, movieStates.movie_ip + "VOL00END");
            int num = 1;

            yield return new WaitForSeconds(0.5f);
            if (isCycle)
                UDPServer.Instance.SocketSendIP(movieStates.movie_ip, movieStates.movie_ip + "B" + movieStates.movie_file + "END");
            else
                UDPServer.Instance.SocketSendIP(movieStates.movie_ip, movieStates.movie_ip + "K" + movieStates.movie_file + "END");
            

            while (num <= 15)
            {
                string str = "";
                if (num >= 10) str = num.ToString();
                else
                    str = "0" + num.ToString();
                yield return new WaitForSeconds(0.15f);
                UDPServer.Instance.SocketSendIP(movieStates.movie_ip, movieStates.movie_ip + "VOL" + str + "END");
                num++;
            }
        }
    }

    public IEnumerator Baozha(float time)
    {
        yield return new WaitForSeconds(time);
        UDPServer.Instance.SocketSendIP("192.168.2.234", "192.168.2.234B0102END");
    }
    #endregion
}
