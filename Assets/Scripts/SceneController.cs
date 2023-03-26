using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    public static SceneController CurrentScene;

    public int Width;
    public int Height;

    [HideInInspector] public List<BaseObject> ObjectList;
    [HideInInspector] public List<ObjectPool> ObjectPools;
    [HideInInspector] public int ObjectCount;

    public LayerMask LayerMask;

    public static float WindowMidWidth;
    public static float WindowMidHeight;

    public static float XLeftFrame;
    public static float XRightFrame;
    public static float YTopFrame;
    public static float YBottomFrame;

    protected void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(new Vector3(Width, -Height) / 2f, new Vector3(Width, Height));
    }

    protected void Awake()
    {
        ObjectList = new List<BaseObject>();
        ObjectPools = new List<ObjectPool>();
        ObjectCount = 0;

        CurrentScene = this;
    }

    protected void Start()
    {
        RenderTexture RenderTarget = new RenderTexture(GameController.instance.Width, GameController.instance.Height, 24, RenderTextureFormat.ARGB32)
        {
            filterMode = FilterMode.Point
        };

        WindowMidWidth = GameController.instance.Width / 2f;
        WindowMidHeight = GameController.instance.Height / 2f;

        Vector3[] vertices = new Vector3[4];
        Vector2[] uv = new Vector2[4];
        int[] triangles = new int[6];

        vertices[0] = new Vector3(0f - WindowMidWidth, 0f + WindowMidHeight);
        vertices[1] = new Vector3(0f + WindowMidWidth, 0f + WindowMidHeight);
        vertices[2] = new Vector3(0f - WindowMidWidth, 0f - WindowMidHeight);
        vertices[3] = new Vector3(0f + WindowMidWidth, 0f - WindowMidHeight);

        uv[0] = new Vector2(0f, 1f);
        uv[1] = new Vector2(1f, 1f);
        uv[2] = new Vector2(0f, 0f);
        uv[3] = new Vector2(1f, 0f);

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        triangles[3] = 2;
        triangles[4] = 1;
        triangles[5] = 3;

        Mesh mesh = new Mesh
        {
            vertices = vertices,
            uv = uv,
            triangles = triangles
        };

        GameObject VirtualScreen = new GameObject("Virtual Screen", typeof(MeshFilter), typeof(MeshRenderer));
        VirtualScreen.transform.position = new Vector3(-WindowMidWidth, WindowMidHeight, -10f);
        VirtualScreen.GetComponent<MeshFilter>().mesh = mesh;
        VirtualScreen.GetComponent<MeshRenderer>().material = Resources.Load("Unlit/Texture") as Material;
        VirtualScreen.GetComponent<MeshRenderer>().material.mainTexture = RenderTarget;

        Camera VirtualCamera = new GameObject("Virtual Camera").AddComponent<Camera>();

        VirtualCamera.transform.position = new Vector3(-WindowMidWidth, WindowMidHeight, -10f);
        VirtualCamera.orthographic = true;
        VirtualCamera.orthographicSize = WindowMidHeight;
        VirtualCamera.eventMask = 1 << 0;
        VirtualCamera.cullingMask = 1 << 0;
        VirtualCamera.nearClipPlane = -64f;
        VirtualCamera.farClipPlane = 256f;
        VirtualCamera.depth = 0f;
        VirtualCamera.clearFlags = CameraClearFlags.Color;
        VirtualCamera.backgroundColor = Color.black;

        Camera MainCamera = new GameObject("Main Camera", typeof(AudioListener)) { tag = "MainCamera" }.AddComponent<Camera>();

        MainCamera.transform.position = new Vector3(WindowMidWidth, -WindowMidHeight, -10f);
        MainCamera.orthographic = true;
        MainCamera.orthographicSize = WindowMidHeight;
        MainCamera.eventMask = LayerMask;
        MainCamera.cullingMask = LayerMask;
        MainCamera.nearClipPlane = -64f;
        MainCamera.farClipPlane = 256f;
        MainCamera.depth = -1f;
        MainCamera.targetTexture = RenderTarget;
        MainCamera.clearFlags = CameraClearFlags.SolidColor;

        XLeftFrame = Camera.main.transform.position.x - WindowMidWidth;
        XRightFrame = Camera.main.transform.position.x + WindowMidWidth;
        YTopFrame = Camera.main.transform.position.y + WindowMidHeight;
        YBottomFrame = Camera.main.transform.position.y - WindowMidHeight;

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

    protected void LateUpdate()
    {
        Camera.main.transform.position = new Vector3()
        {
            x = Mathf.Clamp(Camera.main.transform.position.x, WindowMidWidth, Width - WindowMidWidth),
            y = Mathf.Clamp(Camera.main.transform.position.y, -Height + WindowMidHeight, -WindowMidHeight),
            z = -10f
        };

        XLeftFrame = Camera.main.transform.position.x - WindowMidWidth;
        XRightFrame = Camera.main.transform.position.x + WindowMidWidth;
        YTopFrame = Camera.main.transform.position.y + WindowMidHeight;
        YBottomFrame = Camera.main.transform.position.y - WindowMidHeight;
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
