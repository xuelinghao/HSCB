using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// json文件
/// </summary>
public class SceneData
{
    public string name;
    public List<BoxData> boxs;
    public List<Projection> projections;
    public LightData light;
    public List<FuseData> fuses;
    public List<string> pcHost;
}
/// <summary>
/// 播控盒数据
/// </summary>
public class BoxData
{
    /// <summary>
    /// 播控盒IP地址
    /// </summary>
    public string ip;
    /// <summary>
    /// 播控盒id
    /// </summary>
    public string id;
    /// <summary>
    /// 播控盒播放文件集合
    /// </summary>
    public List<string> program;
}
public class FuseData
{
    /// <summary>
    /// 融合主机IP地址
    /// </summary>
    public string ip;
    /// <summary>
    /// 融合主机id
    /// </summary>
    public string id;
    /// <summary>
    /// 融合主机节目单
    /// </summary>
    public List<string> program;
}
/// <summary>
/// 投影仪数据
/// </summary>
public class Projection
{
    /// <summary>
    /// 投影仪ip
    /// </summary>
    public string ip;
    /// <summary>
    /// 投影仪id
    /// </summary>
    public string id;
}

/// <summary>
/// 灯控盒数据
/// </summary>
public class LightData
{
    /// <summary>
    /// 灯控盒id
    /// </summary>
    public string id;
    /// <summary>
    /// 灯控盒地址
    /// </summary>
    public string addr;
    /// <summary>
    /// 灯控盒文件集合
    /// </summary>
    public List<FileCycle> files;
}

/// <summary>
/// 灯控盒文件数据
/// </summary>
public class FileCycle
{
    /// <summary>
    /// 灯控盒文件名
    /// </summary>
    public string name;
    /// <summary>
    /// 灯控盒文件播放类型
    /// </summary>
    public CycleType cycleType; 
}

/// <summary>
/// 循环类型
/// </summary>
public enum CycleType
{
    /// <summary>
    /// 单文件播放完停止
    /// </summary>
    stop,
    /// <summary>
    /// 单文件循环
    /// </summary>
    one
}

/// <summary>
/// 投影仪类型
/// </summary>
public enum ProjectionType
{
    /// <summary>
    /// 全场景
    /// </summary>
    ALL,
    /// <summary>
    /// 单场景
    /// </summary>
    ONE
}
/// <summary>
/// 灯控数据类型
/// </summary>
public enum LigntType
{
    /// <summary>
    /// SD卡模式
    /// </summary>
    SD,
    /// <summary>
    /// drd文件模式
    /// </summary>
    Drd,
    /// <summary>
    /// 播放
    /// </summary>
    Play,
    /// <summary>
    /// 暂停
    /// </summary>
    Pause,
    /// <summary>
    /// 停止
    /// </summary>
    Stop,
    /// <summary>
    /// 下一个
    /// </summary>
    Next,
    /// <summary>
    /// 上一个
    /// </summary>
    Last,
    /// <summary>
    /// 指定文件播放
    /// </summary>
    Appoint,
    /// <summary>
    /// 循环单个程序
    /// </summary>
    OneCycle,
    /// <summary>
    /// 播放完单个停止
    /// </summary>
    OneStop
}

/// <summary>
/// 灯控数据类型命令
/// </summary>
public class DMX
{
    public string play = "03 01";
    public string pause = "03 02";
    public string stop = "03 53";
    public string next = "03 05";
    public string last = "03 06";
    public string appoint = "03 09";
    public string sd = "03 0E 01 02";
    public string drd = "03 11 01 00";
    public string oneCycle = "03 0B 01 02";
    public string oneStop = "03 0B 01 01";
}

/// <summary>
/// 是否打开了场景
/// </summary>
public enum SceneType 
{
    None,
    particulars
}


public class SceneState
{
    public List<BoxState> boxState;
    public List<FuseState> fuseState;
    public LightState lightState;

}

public class BoxState
{
    /// <summary>
    /// 当前播放
    /// </summary>
    public int num = 0;

    /// <summary>
    /// 音量值
    /// </summary>
    public int volume_num = 15;

    /// <summary>
    /// 是否开机
    /// </summary>
    public bool isStart = true;

    /// <summary>
    /// 是否播放
    /// </summary>
    public bool isPlay = false;

    /// <summary>
    /// 是否静音
    /// </summary>
    public bool isVolume = false;

}
public class FuseState
{
    /// <summary>
    /// 当前播放
    /// </summary>
    public int num = 0;

    /// <summary>
    /// 音量值
    /// </summary>
    public int volume_num = 100;

    /// <summary>
    /// 是否播放
    /// </summary>
    public bool isPlay = false;

    /// <summary>
    /// 是否静音
    /// </summary>
    public bool isVolume = false;
}
public class LightState
{
    /// <summary>
    /// 当前播放
    /// </summary>
    public int num = 0;

    /// <summary>
    /// 是否播放
    /// </summary>
    public bool isPlay = false;
}
public class Account
{
    public string version;
    public List<User> users;
}
public class User
{
    /// <summary>
    /// 用户名
    /// </summary>
    public string user;
    /// <summary>
    /// 密码
    /// </summary>
    public string password;
    /// <summary>
    /// 权限
    /// </summary>
    public int jurisdiction;
}
public class Config
{
    public const string AccessKeyId = "LTAI4G1KjDcPs4DUcUPa3LiD";
    public const string AccessKeySecret = "Gp8gCSMEOu7wJUJ3aASKeGX6gdIvH5";
    public const string EndPoint = "oss-cn-beijing.aliyuncs.com";
    public const string Bucket = "red-fortress";
}
public class SceneState1
{
    public string scene;//场景名
    public List<Phase> phases;///几个状态
}
public class Phase
{
    public bool isStop;//是否暂停
    public int time;//暂停时间
    public string introduce;//状态名字
    public List<LightStates> LightState;
    public List<MovieStates> MovieStates;
    public List<ProjectStates> projectStates;
}
public class LightStates
{
    /// <summary>
    /// 灯控盒ip
    /// </summary>
    public string light_port;
    /// <summary>
    /// 灯控盒文件名
    /// </summary>
    public string light_file;
    /// <summary>
    /// 灯光循环类型
    /// </summary>
    public CycleType cycleType;
}
public class MovieStates
{
    /// <summary>
    /// 是否循环
    /// </summary>
    public bool isCycle;
    /// <summary>
    /// 声音盒ip
    /// </summary>
    public string movie_ip;
    /// <summary>
    /// 声音盒文件名
    /// </summary>
    public string movie_file;//
    /// <summary>
    /// 是否消音
    /// </summary>
    public bool isVanish;//
}
public class ProjectStates
{
    public string project_ip;//投影融合ip
    public string project_name;//投影融合节目单
}
/// <summary>
/// 下载更新媒体文件
/// </summary>
public class FileData
{
    public string filePath;
    public string savePath;
    public string file;

}
