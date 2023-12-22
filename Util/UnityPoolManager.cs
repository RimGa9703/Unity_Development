using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class UnityPoolManager : MonoBehaviour
{
    public static UnityPoolManager Instance = null;
    public Transform managedPoolRoot = null;
    [SerializeField]
    Dictionary<string, IObjectPool<GameObject>> managedPool;

    /*
     * *
     */

    public void Awake()
    {
        Instance = this;
        managedPoolRoot = Instance.transform;
        managedPool = new Dictionary<string, IObjectPool<GameObject>>();
        DontDestroyOnLoad(Instance.gameObject);
    }
    public void OnDestroy()
    {
        ClearAll();
        Destroy(Instance.gameObject);
        Instance = null;
    }
    public void ClearAll()
    {
        Transform child = null;
        //풀매니저 오브젝트에 자식으로서 관리되고 있는(사용되고 있지 않는 오브젝트들) 모두 제거
        
        foreach (IObjectPool <GameObject> item in managedPool.Values)
        {
            item.Clear();
        }
    }

    public void MakePool(GameObject prefab)
    {
        IObjectPool<GameObject> pool = new ObjectPool<GameObject>(() =>
        { return CreatePool(prefab); },
        GetPool, ReleasePool, DestroyPool, maxSize: 1);

        managedPool.Add(prefab.name, pool);
    }

    #region MakePool
    public GameObject CreatePool(GameObject prefab)
    {
        GameObject copy = Instantiate(prefab, managedPoolRoot);
        return copy;
    }
    public void GetPool(GameObject prefab)
    {
        prefab.gameObject.SetActive(true);
    }

    public void ReleasePool(GameObject prefab)
    {
        prefab.SetActive(false);
        prefab.transform.parent = managedPoolRoot;
    }
    public void DestroyPool(GameObject prefab)
    {
        Destroy(prefab);
    }
    #endregion

    #region Spawn
    public GameObject Spawn(GameObject prefab, Vector3 position, Vector3 scale, Quaternion rotation, bool active, Transform parent = null)
    {
        if (managedPool.ContainsKey(prefab.name) == false)
        {
            MakePool(prefab);
        }

        IObjectPool<GameObject> pool = managedPool[prefab.name];
        GameObject clone = pool.Get();
        clone.transform.position = position;
        clone.transform.parent = parent;
        clone.transform.rotation = rotation;
        clone.transform.localScale = scale;
        return clone;
    }
    public GameObject Spawn(GameObject prefab, Vector3 position, Transform parent = null)
    {
        if (managedPool.ContainsKey(prefab.name) == false)
        {
            MakePool(prefab);
        }

        IObjectPool<GameObject> pool = managedPool[prefab.name];
        GameObject clone = pool.Get();
        clone.transform.position = position;
        clone.transform.parent = parent;
        clone.transform.rotation = Quaternion.identity;
        return clone;
    }
    #endregion

    #region Release
    public void Release(string name,GameObject prefab)
    {
        IObjectPool<GameObject> pool = managedPool[name];
        pool.Release(prefab);
    }
    #endregion

}
