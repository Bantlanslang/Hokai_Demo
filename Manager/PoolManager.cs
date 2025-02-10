using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PoolManager : UnitySingleton<PoolManager>
{
    [SerializeField]
    Pool[] FightUIAmount;

    static Dictionary<GameObject, Pool> PoolDic = new Dictionary<GameObject, Pool>();

    private void Start()
    {
        Initialize(FightUIAmount);
    }


#if UNITY_EDITOR
    private void OnDestroy()
    {
        CheckPoolSize(FightUIAmount);
    }

#endif
    void CheckPoolSize(Pool[] pools)
    {
        foreach (var pool in pools)
        {
            if(pool.RuntimeSize > pool.Size)
            {
                Debug.LogWarning($"Pool: {pool.obj.name} has a RunTimeSize {pool.RuntimeSize} Bigger than size {pool.Size}");
            }

        }
    }


    void Initialize(Pool[] pools)
    {
        foreach (Pool pool in pools)
        {

#if UNITY_EDITOR
            if (PoolDic.ContainsKey(pool.obj))
            {
                Debug.LogError("same Prefab in the Pools: " + pool.obj.name);
                continue;
            }
#endif
            PoolDic.Add(pool.obj, pool);
            pool.InitPool(pool.parent);
        }
    }
    /// <summary>
    /// <para>Return a Specifed<paramref name="prefab"></paramref> gameObject in the Pool.</para>
    /// <para>根據傳入的<paramref name="prefab"></paramref>參數,返回對象池中預備好的物件</para>
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns></returns>
    public static GameObject Release(GameObject prefab)
    {

#if UNITY_EDITOR
        if (!PoolDic.ContainsKey(prefab))
        {
            Debug.LogError($"PoolManager could not Find Prefab: {prefab.name}");
            return null;
        }
#endif
        return PoolDic[prefab].PrepareObject();
    }

    public static GameObject Release(GameObject prefab,Vector3 position)
    {

#if UNITY_EDITOR
        if (!PoolDic.ContainsKey(prefab))
        {
            Debug.LogError($"PoolManager could not Find Prefab: {prefab.name}");
            return null;
        }
#endif
        return PoolDic[prefab].PrepareObject(position);
    }

    public static GameObject Release(GameObject prefab,Vector3 position,Quaternion rotation)
    {

#if UNITY_EDITOR
        if (!PoolDic.ContainsKey(prefab))
        {
            Debug.LogError($"PoolManager could not Find Prefab: {prefab.name}");
            return null;
        }
#endif
        return PoolDic[prefab].PrepareObject(position,rotation);
    }

    public static GameObject Release(GameObject prefab,Vector3 position,Quaternion rotation, Vector3 scale)
    {

#if UNITY_EDITOR
        if (!PoolDic.ContainsKey(prefab))
        {
            Debug.LogError($"PoolManager could not Find Prefab: {prefab.name}");
            return null;
        }
#endif
        return PoolDic[prefab].PrepareObject(position,rotation,scale);
    }

}
