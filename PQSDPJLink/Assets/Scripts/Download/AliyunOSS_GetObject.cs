using Aliyun.OSS;
using Aliyun.OSS.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

public class AliyunOSS_GetObject : MonoBehaviour
{
    string filePath;
    string savePath;
    Thread thread;
    Action GetObjectSuccessCallback;


    OssClient ossClient;
    // Start is called before the first frame update
    void Start()
    {
        ossClient = new OssClient(Config.EndPoint, Config.AccessKeyId, Config.AccessKeySecret);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetObjectByThread(() => {
                Debug.Log("下载成功");
            },
            "LB/video.mp4",
             @"C:\Users\姜龙哲\Desktop\PQSD512\" + "video.mp4"
            );
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="action">下载成功事件</param>
    /// <param name="filePath">文件所在目录</param>
    /// <param name="savePath">保存路径</param>
    public void GetObjectByThread(Action action, string filePath, string savePath)
    {
        this.GetObjectSuccessCallback = action;
        this.filePath = filePath;
        this.savePath = savePath;
        thread = new Thread(GetObject);
        thread.Start();
    }

    void GetObject()
    {
        try
        {
            OssObject result = ossClient.GetObject(Config.Bucket, filePath);
            using (var resultStream = result.Content)
            {
                using (var fs = File.Open(savePath, FileMode.OpenOrCreate))
                {
                    int length = (int)resultStream.Length;
                    byte[] bytes = new byte[length];
                    do
                    {
                        length = resultStream.Read(bytes, 0, length);
                        fs.Write(bytes, 0, length);

                    } while (length != 0);
                    this.GetObjectSuccessCallback();
                }

            }
        }
        catch (OssException e)
        {
            print("下载文件出错：" + e);
        }
        catch (Exception e)
        {
            print("下载文件出错：" + e);
        }
        finally
        {

            thread.Abort();
            this.GetObjectSuccessCallback = null;
        }

    }
}


