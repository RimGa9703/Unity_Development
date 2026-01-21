using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Only1Games.Util;
using System.Linq;

/*
 목적: '원본' 리소스(Resource 폴더)를 가져오고 buffer에 저장하여 관리한다.
     - 원본 리소스이기 때문에 수정 및 제거하면 안된다, 원본이 수정된다.
     - 이미 한번이라도 GetObject된 리소스는 Buffer에 저장되며,
         다음 GetObject 콜에서 Buffer에 저장된 리소스를 찾는다면,
         Resource 로드를 하지 않는다.
     - 게임이 지속되면서 다양한 리소스가 필요하며 buffer의 크기가 커진다.
         그럼으로 반드시 적절한 상황에서 Clear 해야 한다.
         씬이 이동하거나, 더 이상 특정 오브젝트를 사용하지 않은 상황
*/

public class AssetManager : Singleton<AssetManager>
{

    public struct SData
    {
        public string assetName;
        public System.Type type;
        public Object asset;
        public bool dontUnload;
    }

    //< path, < assetName, SData > >
    Dictionary<string, List<SData>> buffer = new Dictionary<string, List<SData>>();

    /*
     * *
     */

    public void DoDestroy()
    {
        if (Instance == null)
            return;
        Instance.ClearAll();
        Clear();
    }
    public void Init()
    {
        //에셋들 미리 로드
        LoadAllAsset<EnemyCharacter>(AssetPath.Enemys, typeof(EnemyCharacter));
        LoadAllAsset<GameObject>(AssetPath.FieldObeject, typeof(GameObject));
    }

    #region GetAsset
    public List<T> GetAssetAll<T>(string _path, bool dontUnload = false) where T : Object
    {
        return GetAssetLoadAll<T>(_path, typeof(T), dontUnload); ;
    }

    public T GetAsset<T>(string _path, string _assetName, string _extension = null) where T : Object
    {
        StringMaker.Clear();
        StringMaker.stringBuilder.Append(_assetName);
        if (_extension != null)
            StringMaker.stringBuilder.Append(_extension);
        string assetName = StringMaker.stringBuilder.ToString();

        SData data = AssetLoad(_path, assetName, typeof(T), false);
        return data.asset as T;
    }
    #endregion

    public T GetAssetDontUnload<T>(string _path, string _assetName, string _extension = null) where T : Object
    {
        StringMaker.Clear();
        StringMaker.stringBuilder.Append(_assetName);
        if (_extension != null)
            StringMaker.stringBuilder.Append(_extension);
        string assetName = StringMaker.stringBuilder.ToString();

        SData data = AssetLoad(_path, assetName, typeof(T), true);
        return (T)data.asset;
    }

    //////////////////////////////////////////////////////////////////////////////////////////
    //
    public void ReleaseAsset<T>(string _path, string _assetName) where T : Object
    {
        AssetUnload(_path, _assetName, typeof(T));
    }

    //////////////////////////////////////////////////////////////////////////////////////////
    //
    public void ClearAll()
    {
        SData data;
        int Count = 0;
        List<SData> assets = null;
        foreach (KeyValuePair<string, List<SData>> item in buffer)
        {
            assets = item.Value;
            for (int i = Count - 1; i >= 0; i--)
            {
                data = assets[i];
                if (data.dontUnload == true)
                    continue;
                Resources.UnloadAsset(data.asset);
                assets.RemoveAt(i);
            }
        }
        //buffer.Clear();
    }

    public IEnumerator DoRefresh()
    {
        yield return Resources.UnloadUnusedAssets();
    }

    //////////////////////////////////////////////////////////////////////////////////////////
    //
    SData Find(string _path, string _assetName, System.Type _type)
    {
        if (buffer.ContainsKey(_path) == false)
            return default(SData);

        SData data;
        List<SData> list = buffer[_path];
        int Count = list.Count;
        for (int i = 0; i < Count; i++)
        {
            data = list[i];
            if (data.assetName.Equals(_assetName) == false)
                continue;
            if (data.type.Equals(_type) == false)
                continue;

            /**/
            return data;
        }

        return default(SData);
    }

    // Sprite[], Sprite Atlas를 불러오기 위함
    // 번들 폴더 경로의 모든 Asset을 가져와야 하는 방법 필요.

    List<T> GetAssetLoadAll<T>(string _path, System.Type _type, bool dontUnload = false) where T : Object
    {
        if (buffer.ContainsKey(_path) == false)
            CreatePath(_path);

        List<T> targetList = new List<T>();
        List<SData> sDatas = buffer[_path];
        for (int i = 0; i < sDatas.Count; i++)
        {
            SData data = sDatas[i];
            if (data.type.Equals(_type) == true)
                targetList.Add(data.asset as T);
        }
        return targetList;
    }
    void LoadAllAsset<T>(string _path, System.Type _type, bool dontUnload = false) where T : Object
    {
        // 경로 없으면 새로 생성
        if (buffer.ContainsKey(_path) == false)
            CreatePath(_path); // buffer[_path] = new List<SData>();

        // 리소스 로드
        T[] loadedAssets = Resources.LoadAll<T>(_path);

        foreach (var asset in loadedAssets)
        {
            // 동일한 type + assetName 이 이미 있는지 확인
            bool exists = buffer[_path].Any(d => d.type == _type && d.assetName == asset.name);

            if (!exists)
            {
                var data = new SData
                {
                    type = _type,
                    assetName = asset.name,
                    asset = asset,
                    dontUnload = dontUnload
                };

                buffer[_path].Add(data);
            }
        }
    }


    SData AssetLoad(string _path, string _assetName, System.Type _type, bool dontUnload = false)
    {
        SData data = Find(_path, _assetName, _type);
        if (data.Equals(default(SData)) == false)
            return data;

        StringMaker.Clear();
        StringMaker.stringBuilder.Append(_path);
        StringMaker.stringBuilder.Append(_assetName);
        string fullPath = StringMaker.stringBuilder.ToString();

        Object asset = null;
        //if (AssetBundleInterface.Contains(fullPath) == true)
        //    asset = AssetBundleInterface.Instance.LoadAsset(fullPath, _type);
        //else
        asset = Resources.Load(fullPath, _type);

        if (asset == null)
        {
            //DebugLog.LogError(_path + _assetName + " is not find !!");
            return default(SData);
        }

        data = new SData();
        data.type = _type;
        data.assetName = _assetName;
        data.asset = asset;
        data.dontUnload = dontUnload;

        if (buffer.ContainsKey(_path) == false)
            CreatePath(_path);
        buffer[_path].Add(data);

        return data;
    }

    void AssetUnload(string _path, string _assetName, System.Type _type)
    {
        SData data = Find(_path, _assetName, _type);
        //if (data.Equals(default(SData)) == false)
        if (data.Equals(default(SData)))
            return;

        if (data.dontUnload == true)
            return;

        buffer[_path].Remove(data);
        if (buffer[_path].Count <= 0)
            buffer.Remove(_path);

        Resources.UnloadAsset(data.asset);
    }

    void CreatePath(string _path)
    {
        if (buffer.ContainsKey(_path) == true)
            return;

        buffer.Add(_path, new List<SData>());
    }

    //////////////////////////////////////////////////////////////////////////////////////////
    //
    public static string GetAssetName(string _fullPath)
    {
        try
        {
            int index = _fullPath.LastIndexOf('/');
            return _fullPath.Substring(index + 1);
        }
        catch (System.Exception)
        {
            return null;
        }
    }

    public static string GetAssetPath(string _fullPath)
    {
        try
        {
            int index = _fullPath.LastIndexOf('/');
            return _fullPath.Substring(0, index + 1);
        }
        catch (System.Exception)
        {
            return null;
        }
    }
}
