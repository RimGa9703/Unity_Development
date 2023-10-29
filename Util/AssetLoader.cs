using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Resource.Load를 통해 로드되었던 Asset을 재활용하기 위한 클래스
public class AssetLoader
{
    public static AssetLoader Instance = null;

    public Dictionary<string, Object> assetDic = null;

    public static void Init()
    {
        if (Instance == null)
        {
            Instance = new AssetLoader();
            Instance.assetDic = new Dictionary<string, Object>();
        }
        
    }

    public void Clear()
    {
        assetDic.Clear();
    }

    public Object Load(string key, string path)
    {
        if (assetDic.ContainsKey(key))
            return assetDic[key];
        else
        {
            Object obj = Resources.Load<Object>(path);
            if (obj != null)
            {
                assetDic.Add(key, obj);
                return obj;
            }
            else
                throw new System.Exception("Failed to load asset at path: " + path);
            
        }
    }
    //TODO: 필요할 때 만들자
    //public Object[] LoadAll(string key, string path)
    //{
       
    //}
}
