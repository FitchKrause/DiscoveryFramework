using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public string PoolName;
    public int PoolSize;
    public GameObject ObjectToPool;
    public List<BaseObject> PooledObjects;

    private void Start()
    {
        PooledObjects = new List<BaseObject>();
        StageController.CurrentStage.ObjectPools.Add(this);
    }
}
