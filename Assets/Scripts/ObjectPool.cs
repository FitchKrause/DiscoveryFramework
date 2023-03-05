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

        for (int i = 0; i < PoolSize; i++)
        {
            BaseObject objRef = Instantiate(ObjectToPool).GetComponent<BaseObject>();
            objRef.ObjectName = PoolName;
            objRef.gameObject.SetActive(false);
            PooledObjects.Add(objRef);
        }
        StageController.CurrentStage.ObjectPools.Add(this);
    }
}
