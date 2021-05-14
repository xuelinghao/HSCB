using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonControl : MonoBehaviour
{
    public static JsonControl Instance = new JsonControl();
    private JsonControl()
    {

    }

    public bool WriteStringToFile(string fullPath, string data)
    {
        try
        {
            string directoryName = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            File.WriteAllText(fullPath, data);
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError(string.Format("WriteStringToFile fail, file=[{0}], Exception msg:[{1}]", fullPath, ex.ToString()));
            return false;
        }
    }

    public bool WriteToFile(string fullPath, JsonData data)
    {
        return WriteStringToFile(fullPath, data.ToJson());
    }

    public JsonData ReadFromFile(string fullPath)
    {
        try
        {
            var json = ReadStringFromFile(fullPath);
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            return JsonMapper.ToObject(json);
        }
        catch (Exception ex)
        {
            Debug.LogErrorFormat("ReadFromFile {0} failed!, Exception:{1}", new object[2] { fullPath, ex.ToString() });
            return null;
        }
    }

    public string ReadStringFromFile(string fullPath)
    {
        try
        {
            if (!File.Exists(fullPath))
            {
                return null;
            }

            return File.ReadAllText(fullPath);
        }
        catch (Exception ex)
        {
            Debug.LogErrorFormat("ReadFromFile {0} failed!, Exception:{1}", new object[2] { fullPath, ex.ToString() });
            return null;
        }
    }

}
