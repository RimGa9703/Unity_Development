using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                //하이라이어키에서 오브젝트 찾기
                instance = FindFirstObjectByType(typeof(T)) as T;
            }
            return instance;
        }
        set { instance = value; }
    }
    
    public virtual void InitInstance()
    {
        //싱글톤 스크립트가 달린 오브젝트에 부모가 있을 경우 부모를 DontDestroyOnLoad
        if (transform.parent != null && transform.root != null) DontDestroyOnLoad(this.transform.root.gameObject);
        else DontDestroyOnLoad(this.gameObject);
    }
    public virtual void InstanceClear()
    {
        Instance = null;
    }
}
