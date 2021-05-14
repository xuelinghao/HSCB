using Aliyun.OSS;
using Aliyun.OSS.Common;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using UnityEngine;

public class DownloadControl : MonoBehaviour
{
    public static DownloadControl Instance;

    string filePath;
    string savePath;
    Thread thread;
    Action GetObjectSuccessCallback;

    static string key = "jiamikeydechangduxushisanshierge";

    /// <summary>
    /// 用户名文件地址
    /// </summary>
    string userPath;
    /// <summary>
    /// 更新文件地址
    /// </summary>
    string updateuserPath;

    /// <summary>
    /// 版本号
    /// </summary>
    public string version;
    public Account account;
    OssClient ossClient;
    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    public void Init()
    {
        ossClient = new OssClient(Config.EndPoint, Config.AccessKeyId, Config.AccessKeySecret);
        num = 0;
    }
    bool isDownload = false;
    bool isUpdate = false;
    bool isMedia = false;

    bool isDownloadMedia = false;


    // Update is called once per frame
    void Update()
    {
        if (isOK&& GetObjectSuccessCallback!=null)
        {
            isOK = false;
            GetObjectSuccessCallback();
        }
        if (isDownload)
        {
            string strt = File.ReadAllText(userPath);
            File.WriteAllText(userPath, Encrypt(strt, key));
            JsonData jsonData1 = JsonMapper.ToObject(strt);
            if (jsonData1 != null)
            {
                account = JsonMapper.ToObject<Account>(jsonData1.ToJson());
                transform.GetComponent<RegisterControl>().users = account.users;
            }
            isDownload = false;

        }
        if (isUpdate)
        {
            string strt = File.ReadAllText(updateuserPath);

            Account update_account = new Account();
            JsonData jsonData1 = JsonMapper.ToObject(strt);
            if (jsonData1 != null)
                update_account = JsonMapper.ToObject<Account>(jsonData1.ToJson());
            if (version!= update_account.version)
            {
                version = update_account.version;
                account = update_account;
                transform.GetComponent<RegisterControl>().users = update_account.users;
                File.WriteAllText(userPath, Encrypt(strt, key));
                RedCastleControl.Instance.UpdateMedia();
            }
            File.Delete(updateuserPath);

            isUpdate = false;
        }
        if (isMedia)
        {
            string strt = File.ReadAllText(mediaPath);
            JsonData jsonData1 = JsonMapper.ToObject(strt);
            if (jsonData1 != null)
                RedCastleControl.Instance.fileDatas = JsonMapper.ToObject<List<FileData>>(jsonData1.ToJson());
        }
        if (isDownloadMedia)
        {
            isDownloadMedia = false;
            DownLoadMedia();
        }
    }
    public void ReadUser(string _userPath)
    {
        userPath = _userPath;
        JsonData jsonData1 = JsonMapper.ToObject(Decrypt(File.ReadAllText(_userPath), key));
        if (jsonData1 != null)
        {
            account = JsonMapper.ToObject<Account>(jsonData1.ToJson());
            version = account.version;
            transform.GetComponent<RegisterControl>().users = account.users;
        }
    }
    public void DownLoadUser(string path,string file)
    {
        userPath = path + @"\" + file;
        GetObjectByThread(() => {
            Debug.Log("下载成功");
            isDownload = true;
        },
            file,
             userPath
            );
    }
    public void UpdateUser(string path, string file)
    {
        updateuserPath = path + @"\" + file;
        GetObjectByThread(() => {
            Debug.Log("下载成功");
            isUpdate = true;
        },
            file,
             updateuserPath
            );
    }
    public int num;
    string mediaPath;
    public void DownLoadMedia(string path, string file)
    {
        mediaPath = path + @"\" + file;
        Debug.Log(mediaPath);

        GetObjectByThread(() => {
            Debug.Log("下载媒体文件成功");
            isMedia = true;
        },
            file,
             mediaPath
            );
    }
    public void UpdateMeida()
    {
        List<SceneData> sceneDatas = FacilityControl.Instance.sceneDatas;
        for (int i = 0; i < sceneDatas.Count; i++)
        {
            for (int j = 0; j < sceneDatas[i].pcHost.Count; j++)
            {
                UDPServer.Instance.SocketSendIP(sceneDatas[i].pcHost[j], "update");
            }
        }
        DownLoadMedia();
    }
    public void DownLoadMedia()
    {
        if (num < RedCastleControl.Instance.fileDatas.Count) 
        {
            if (!Directory.Exists(RedCastleControl.Instance.fileDatas[num].savePath))
            {
                //文件夹不存在则创建该文件夹
                Directory.CreateDirectory(RedCastleControl.Instance.fileDatas[num].savePath);
            }
            GetObjectByThread(() => {
                Debug.Log("下载成功:" + RedCastleControl.Instance.fileDatas[num].file);
                num++;
                isDownloadMedia = true;
            },
            RedCastleControl.Instance.fileDatas[num].filePath,
                        RedCastleControl.Instance.fileDatas[num].savePath+@"\"+ RedCastleControl.Instance.fileDatas[num].file
            );
        }
    }
    bool isOK=false;
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
        thread = new Thread(()=> { GetObject(filePath); });
        thread.Start();
    }

    void GetObject(string file)
    {
        try
        {
            OssObject result = ossClient.GetObject(Config.Bucket, filePath);
            using (var resultStream = result.Content)
            {
                using (var fs = File.Open(savePath, FileMode.Create))
                {
                    int length = (int)resultStream.Length;
                    byte[] bytes = new byte[length];

                    do
                    {
                        length = resultStream.Read(bytes, 0, length);
                        fs.Write(bytes, 0, length);
                    } while (length != 0);
                    isOK = true;
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
    #region ========加密========
    /// <summary> 
    /// 加密数据 
    /// </summary> 
    /// <param name="Text">要加密的内容</param> 
    /// <param name="sKey">key，必须为32位</param> 
    /// <returns></returns> 
    public static string Encrypt(string Text, string sKey)
    {
        byte[] keyArray = UTF8Encoding.UTF8.GetBytes(sKey);

        RijndaelManaged encryption = new RijndaelManaged();

        encryption.Key = keyArray;

        encryption.Mode = CipherMode.ECB;

        encryption.Padding = PaddingMode.PKCS7;

        ICryptoTransform cTransform = encryption.CreateEncryptor();

        byte[] _EncryptArray = UTF8Encoding.UTF8.GetBytes(Text);

        byte[] resultArray = cTransform.TransformFinalBlock(_EncryptArray, 0, _EncryptArray.Length);

        return Convert.ToBase64String(resultArray, 0, resultArray.Length);

    }

    #endregion

    #region ========解密========
    /// <summary> 
    /// 解密数据 
    /// </summary> 
    /// <param name="Text"></param> 
    /// <param name="sKey"></param> 
    /// <returns></returns> 
    public static string Decrypt(string Text, string sKey)
    {
        byte[] keyArray = UTF8Encoding.UTF8.GetBytes(sKey);

        RijndaelManaged decipher = new RijndaelManaged();

        decipher.Key = keyArray;

        decipher.Mode = CipherMode.ECB;

        decipher.Padding = PaddingMode.PKCS7;

        ICryptoTransform cTransform = decipher.CreateDecryptor();

        byte[] _EncryptArray = Convert.FromBase64String(Text);

        byte[] resultArray = cTransform.TransformFinalBlock(_EncryptArray, 0, _EncryptArray.Length);

        return UTF8Encoding.UTF8.GetString(resultArray);

    }

    #endregion
}
