using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    public static SceneController CurrentScene;

    public int Width;
    public int Height;

    public static bool AllowPause;
    public bool Paused;
    private int PauseTimer;

    [HideInInspector] public List<BaseObject> ObjectList;
    [HideInInspector] public List<ObjectPool> ObjectPools;
    [HideInInspector] public int ObjectCount;

    protected void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(new Vector3(Width, -Height) / 2f, new Vector3(Width, Height));
    }

    protected void Awake()
    {
        Paused = false;

        ObjectList = new List<BaseObject>();
        ObjectPools = new List<ObjectPool>();
        ObjectCount = 0;

        CurrentScene = this;
    }

    protected void Start()
    {
        foreach (BaseObject objRef in FindObjectsOfType<BaseObject>())
        {
            if (objRef.ObjectName == string.Empty)
            {
                objRef.ObjectName = objRef.GetType().Name;
            }
            ObjectList.Add(objRef);
            ObjectCount++;
        }

        foreach (ObjectPool pool in ObjectPools)
        {
            for (int i = 0; i < pool.PoolSize; i++)
            {
                BaseObject objRef = Instantiate(pool.ObjectToPool).GetComponent<BaseObject>();
                objRef.ObjectName = pool.PoolName;
                objRef.gameObject.SetActive(false);
                pool.PooledObjects.Add(objRef);
            }
        }
    }

    protected void FixedUpdate()
    {
        if (PauseTimer > 0)
        {
            PauseTimer -= GameController.Frame;
        }
    }

    protected void LateUpdate()
    {
        Camera.main.transform.position = new Vector3()
        {
            x = Mathf.Clamp(Camera.main.transform.position.x, GameController.WindowMidWidth, Width - GameController.WindowMidWidth),
            y = Mathf.Clamp(Camera.main.transform.position.y, -Height + GameController.WindowMidHeight, -GameController.WindowMidHeight),
            z = -10f
        };

        GameController.XLeftFrame = Camera.main.transform.position.x - GameController.WindowMidWidth;
        GameController.XRightFrame = Camera.main.transform.position.x + GameController.WindowMidWidth;
        GameController.YTopFrame = Camera.main.transform.position.y + GameController.WindowMidHeight;
        GameController.YBottomFrame = Camera.main.transform.position.y - GameController.WindowMidHeight;
    }

    public void PauseScene(bool value)
    {
        if (!AllowPause) return;

        if (PauseTimer == 0)
        {
            Paused = value;

            foreach (BaseObject objRef in ObjectList)
            {
                objRef.enabled = !Paused;
            }

            foreach (Animator animator in FindObjectsOfType<Animator>())
            {
                animator.enabled = !Paused;
            }

            if (Paused) AudioController.Pause();
            else AudioController.Resume();

            PauseTimer = 5;
        }
    }

    public static BaseObject CreateStageObject(string objName, float PosX, float PosY)
    {
        foreach (ObjectPool pool in CurrentScene.ObjectPools)
        {
            if (pool.PoolName == objName)
            {
                foreach (BaseObject objRef in pool.PooledObjects)
                {
                    if (!objRef.gameObject.activeSelf)
                    {
                        objRef.XPosition = PosX;
                        objRef.YPosition = PosY;
                        objRef.transform.position = new Vector3(PosX, PosY, 0f);
                        objRef.gameObject.SetActive(true);
                        objRef.ObjectCreated();
                        CurrentScene.ObjectCount++;
                        CurrentScene.ObjectList.Add(objRef);
                        return objRef;
                    }
                }
            }
        }

        return null;
    }

    public static void DestroyStageObject(BaseObject objRef)
    {
        if (objRef.gameObject.activeSelf)
        {
            objRef.gameObject.SetActive(false);
            CurrentScene.ObjectCount--;
            CurrentScene.ObjectList.Remove(objRef);
        }
    }

    public static BaseObject FindStageObject(string objName)
    {
        foreach (BaseObject objRef in CurrentScene.ObjectList)
        {
            if (objRef.ObjectName == objName)
            {
                return objRef;
            }
        }

        return null;
    }

    public static BaseObject[] FindStageObjects(string objName)
    {
        List<BaseObject> objsRef = new List<BaseObject>();

        foreach (BaseObject objRef in CurrentScene.ObjectList)
        {
            if (objRef.ObjectName == objName)
            {
                objsRef.Add(objRef);
            }
        }

        return objsRef.ToArray();
    }
}
