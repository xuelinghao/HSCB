using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using UnityEngine.UI;
using rv;
using static rv.Command;
using static rv.PowerCommand;

public class ProjectionControl : MonoBehaviour
{
    public static ProjectionControl Instance;

    int current_scene;
    int scene_num;
    int projections_num;
    ProjectionType type = ProjectionType.ALL;
    Power power = Power.QUERY;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
    }
    public void Projection(int _scene_num, ProjectionType _type,Power _power)
    {
        current_scene = _scene_num;
        scene_num = _scene_num;
        type = _type;
        power = _power;
        Open();
    }
    public float number = 0;
    public void Open()
    {
        while (FacilityControl.Instance.sceneDatas[scene_num].projections.Count==0)
        {
            scene_num++;
        }
        if ((type == ProjectionType.ONE && current_scene == scene_num) || type == ProjectionType.ALL)
        {
            
            PJLinkConnection c = new PJLinkConnection(FacilityControl.Instance.sceneDatas[scene_num].projections[projections_num].ip);
            PowerCommand pc = new PowerCommand(power);
            c.sendCommandAsync(pc, OpenProjection);
        }
        else
        {
            
        }
    }
    int error=0;
    public void OpenProjection(Command sender, Response response)
    {
        bool isSuccess = false;
        SceneData sceneData = FacilityControl.Instance.sceneDatas[scene_num];
        switch (response)
        {
            case Response.SUCCESS:
                isSuccess = true;
                string str = "";
                if (power == Power.OFF)
                    str = "关闭";
                else if (power == Power.ON)
                    str = "开启";
                Debug.Log(sceneData.name + "场景的：" + sceneData.projections[projections_num].ip + "已" + str);
                if (RedCastleControl.Instance.fail_project.ContainsKey(sceneData.projections[projections_num].ip))
                {
                    RedCastleControl.Instance.fail_project.Remove(sceneData.projections[projections_num].ip);
                }
                error = 0;
                break;
            case Response.UNDEFINED_CMD:
                Debug.Log(sceneData.name + "场景的：" + sceneData.projections[projections_num].ip + "已" + response);
                if (!RedCastleControl.Instance.fail_project.ContainsKey(sceneData.projections[projections_num].ip))
                {
                    RedCastleControl.Instance.fail_project.Add(sceneData.projections[projections_num].ip, sceneData.name);
                }
                break;
            case Response.OUT_OF_PARAMETER:
                Debug.Log(sceneData.name + "场景的：" + sceneData.projections[projections_num].ip + "已" + response);
                if (!RedCastleControl.Instance.fail_project.ContainsKey(sceneData.projections[projections_num].ip))
                {
                    RedCastleControl.Instance.fail_project.Add(sceneData.projections[projections_num].ip, sceneData.name);
                }
                break;
            case Response.UNVAILABLE_TIME:
                ///已经开或者已经关，直接跳过
                isSuccess = true;
                Debug.Log(sceneData.name + "场景的：" + sceneData.projections[projections_num].ip + "已" + response);
                if (RedCastleControl.Instance.fail_project.ContainsKey(sceneData.projections[projections_num].ip))
                {
                    RedCastleControl.Instance.fail_project.Remove(sceneData.projections[projections_num].ip);
                }
                error = 0;
                break;
            case Response.PROJECTOR_FAILURE:
                Debug.Log(sceneData.name + "场景的：" + sceneData.projections[projections_num].ip + "已" + response);
                if (!RedCastleControl.Instance.fail_project.ContainsKey(sceneData.projections[projections_num].ip))
                {
                    RedCastleControl.Instance.fail_project.Add(sceneData.projections[projections_num].ip, sceneData.name);
                }
                break;
            case Response.AUTH_FAILURE:
                Debug.Log(sceneData.name + "场景的：" + sceneData.projections[projections_num].ip + "已" + response);
                if (!RedCastleControl.Instance.fail_project.ContainsKey(sceneData.projections[projections_num].ip))
                {
                    RedCastleControl.Instance.fail_project.Add(sceneData.projections[projections_num].ip, sceneData.name);
                }
                break;
            case Response.COMMUNICATION_ERROR:
                Debug.Log(sceneData.name + "场景的：" + sceneData.projections[projections_num].ip + "已" + response);
                if (!RedCastleControl.Instance.fail_project.ContainsKey(sceneData.projections[projections_num].ip))
                {
                    RedCastleControl.Instance.fail_project.Add(sceneData.projections[projections_num].ip, sceneData.name);
                }
                break;
        }
        if (!isSuccess)
            error++;

        if (error == 0 || error >= 3)
        {
            if (!(projections_num == FacilityControl.Instance.sceneDatas[scene_num].projections.Count - 1))
            {
                projections_num++;
            }
            else
            {
                projections_num = 0;
                scene_num++;
            }
            error = 0;
            number++;
        }
        if (scene_num < FacilityControl.Instance.sceneDatas.Count)
        {
            Open();
        }

    }
}
public class ProjectionData
{
    public string scene_name;
    public List<Projection> ip_list;

}

