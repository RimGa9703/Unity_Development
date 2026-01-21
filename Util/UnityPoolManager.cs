using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// 오브젝트 풀링을 관리하는 싱글턴 매니저 클래스.
/// 게임 오브젝트의 생성/파괴 비용을 줄여 성능을 최적화합니다.
/// </summary>
public class UnityPoolManager : MonoSingleton<UnityPoolManager>
{
    // 풀링된 오브젝트를 보관하는 부모 Transform
    public Transform managedPoolRoot = null;

    // 게임 플레이 중인 오브젝트의 부모 Transform
    public Transform playRoot = null;

    // 프리팹 이름을 키로 사용하여 IObjectPool 인스턴스를 저장하는 딕셔너리
    private Dictionary<string, IObjectPool<GameObject>> _managedPool;

    /// <summary>
    /// 싱글턴 인스턴스 초기화.
    /// </summary>
    public override void InitInstance()
    {
        base.InitInstance();
        managedPoolRoot = Instance.transform;
        _managedPool = new Dictionary<string, IObjectPool<GameObject>>();
    }

    /// <summary>
    /// 매니저 오브젝트가 파괴될 때 풀에 있는 모든 오브젝트를 정리하고 인스턴스를 파괴합니다.
    /// </summary>
    public void OnDestroy()
    {
        ClearAllPools();
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
            Instance = null;
        }
    }

    /// <summary>
    /// 모든 풀에 있는 오브젝트를 제거합니다.
    /// </summary>
    public void ClearAllPools()
    {
        foreach (var item in _managedPool.Values)
        {
            item.Clear();
        }
        _managedPool.Clear();
    }

    /// <summary>
    /// 지정된 프리팹에 대한 오브젝트 풀을 생성하거나, 이미 존재하면 반환합니다.
    /// </summary>
    /// <param name="prefab">풀링할 원본 GameObject 프리팹.</param>
    /// <param name="defaultSize">초기에 생성할 오브젝트 수.</param>
    /// <param name="maxSize">풀에 보관할 수 있는 오브젝트의 최대 개수.</param>
    public IObjectPool<GameObject> MakePool(GameObject prefab, int defaultSize = 10, int maxSize = 100)
    {
        string name = prefab.name;

        if (!_managedPool.ContainsKey(name))
        {
            IObjectPool<GameObject> pool = new ObjectPool<GameObject>(
                // CreateFunc: 오브젝트를 새로 생성할 때 호출되는 함수
                () => CreatePooledObject(prefab),
                // ActionOnGet: 풀에서 오브젝트를 가져올 때 호출되는 함수
                (obj) => { obj.gameObject.SetActive(true); },
                // ActionOnRelease: 풀에 오브젝트를 반환할 때 호출되는 함수
                (obj) => {
                    obj.SetActive(false);
                    obj.transform.parent = managedPoolRoot;
                },
                // ActionOnDestroy: 풀이 파괴될 때 오브젝트를 파괴하는 함수
                (obj) => { Destroy(obj); },
                // CollectionCheck: Get, Release 시 에러 체크 여부
                true,
                defaultSize,
                maxSize
            );

            _managedPool.Add(name, pool);

            // 미리 오브젝트를 생성하여 풀을 채웁니다.
            for (int i = 0; i < defaultSize; i++)
            {
                pool.Release(pool.Get());
            }
        }

        return _managedPool[name];
    }

    /// <summary>
    /// 풀에서 사용할 오브젝트를 생성합니다.
    /// </summary>
    /// <param name="prefab">원본 프리팹.</param>
    /// <returns>생성된 GameObject.</returns>
    private GameObject CreatePooledObject(GameObject prefab)
    {
        GameObject newObj = Instantiate(prefab, managedPoolRoot);
        // 오브젝트에 풀 정보를 저장하는 컴포넌트를 추가합니다.
        SpawnedObject spawnedInfo = newObj.AddComponent<SpawnedObject>();
        spawnedInfo.poolManager = this;
        spawnedInfo.originalPrefabName = prefab.name;
        return newObj;
    }

    /// <summary>
    /// 오브젝트 풀에서 오브젝트를 가져옵니다.
    /// </summary>
    /// <param name="prefab">원본 프리팹.</param>
    /// <param name="position">오브젝트의 초기 위치.</param>
    /// <param name="parent">오브젝트가 속할 부모 Transform.</param>
    /// <returns>활성화된 GameObject.</returns>
    public GameObject Spawn(GameObject prefab, Vector3 position, Transform parent = null)
    {
        IObjectPool<GameObject> pool = MakePool(prefab);
        GameObject clone = pool.Get();

        clone.transform.parent = parent == null ? playRoot : parent;
        clone.transform.position = position;
        clone.transform.rotation = Quaternion.identity;

        return clone;
    }

    /// <summary>
    /// 풀에서 오브젝트를 가져와 위치, 회전, 크기를 설정합니다.
    /// </summary>
    /// <param name="prefab">원본 프리팹.</param>
    /// <param name="position">초기 위치.</param>
    /// <param name="rotation">초기 회전.</param>
    /// <param name="scale">초기 크기.</param>
    /// <param name="parent">오브젝트가 속할 부모 Transform.</param>
    /// <returns>활성화된 GameObject.</returns>
    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 scale, Transform parent = null)
    {
        IObjectPool<GameObject> pool = MakePool(prefab);
        GameObject clone = pool.Get();

        clone.transform.parent = parent == null ? playRoot : parent;

        clone.transform.position = position;
        clone.transform.rotation = rotation;
        clone.transform.localScale = scale;

        return clone;
    }

    /// <summary>
    /// 풀에서 오브젝트를 가져와 컴포넌트를 반환합니다.
    /// </summary>
    /// <typeparam name="T">가져올 컴포넌트 타입.</typeparam>
    /// <param name="prefab">원본 프리팹.</param>
    /// <param name="position">초기 위치.</param>
    /// <param name="parent">부모 Transform.</param>
    /// <returns>지정된 타입의 컴포넌트.</returns>
    public T Spawn<T>(GameObject prefab, Vector3 position, Transform parent = null) where T : Component
    {
        GameObject clone = Spawn(prefab, position, parent);
        return clone.GetComponent<T>();
    }

    /// <summary>
    /// 오브젝트를 풀에 반환합니다.
    /// </summary>
    /// <param name="obj">반환할 GameObject.</param>
    public void Release(GameObject obj)
    {
        if (obj == null) return;

        // SpawnedObject 컴포넌트를 통해 원본 프리팹의 이름을 안전하게 가져옵니다.
        if (obj.TryGetComponent<SpawnedObject>(out SpawnedObject spawnedInfo))
        {
            if (_managedPool.TryGetValue(spawnedInfo.originalPrefabName, out IObjectPool<GameObject> pool))
            {
                pool.Release(obj);
            }
            else
            {
                Debug.LogWarning($"풀이 존재하지 않아 오브젝트 '{obj.name}'를 파괴합니다.");
                Destroy(obj);
            }
        }
        else
        {
            // 풀을 통해 생성되지 않은 오브젝트는 바로 파괴합니다.
            Destroy(obj);
        }
    }
}

/// <summary>
/// 풀을 통해 생성된 오브젝트에 부착되어 원본 프리팹 정보를 저장하는 헬퍼 컴포넌트.
/// </summary>
public class SpawnedObject : MonoBehaviour
{
    public UnityPoolManager poolManager;
    public string originalPrefabName;

    // 풀에 오브젝트를 반환하기 위한 편의 메서드
    public void Release()
    {
        poolManager.Release(this.gameObject);
    }
}