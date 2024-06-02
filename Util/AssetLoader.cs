using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Resource.Load를 통해 로드되었던 Asset을 재활용하기 위한 클래스
public class AssetLoader
{
    public static AssetLoader Instance = null;

    public Dictionary<string, SAssetData> assetDic = null;

    public struct SAssetData
    {
        public System.Type type;
        public Object asset;
        public bool dontClear;
    }

    public static void Init()
    {
        if (Instance == null)
        {
            Instance = new AssetLoader();
            Instance.assetDic = new Dictionary<string, SAssetData>();
        }

    }

    public void Clear()
    {
        assetDic.Clear();
    }

    public T GetAsset<T>(string path, string assetName) where T : Object
    {
        SAssetData data = AssetLoad(path, assetName, typeof(T));
        return data.asset as T;
    }
    public SAssetData AssetLoad(string path, string assetName, System.Type _type, bool dontClear = false)
    {
        SAssetData data;
        if (assetDic.ContainsKey(assetName) == true)
            data = assetDic[assetName];
        else
        {
            data = default(SAssetData);
            string fullpath = string.Format("{0}/{1}", path, assetName);

            Object asset = Resources.Load(fullpath, _type);

            if (asset == null)
                return default(SAssetData);

            data = new SAssetData();
            data.type = _type;
            data.asset = asset;
            data.dontClear = dontClear;

            assetDic.Add(assetName, data);
        }


        return data;
    }
}
