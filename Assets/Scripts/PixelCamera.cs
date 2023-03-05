using UnityEngine;
using System.Collections;

public class PixelCamera : MonoBehaviour
{
    public RenderTexture RenderTarget;

    public LayerMask LayerMask;

    public Color BackgroundColor;

    [HideInInspector] public float WindowMidWidth;
    [HideInInspector] public float WindowMidHeight;

    [HideInInspector]
    public float aspect;

    public static float XLeftFrame;
    public static float XRightFrame;
    public static float YTopFrame;
    public static float YBottomFrame;

    private GameObject HUD_OBJ;

    public void Start()
    {
        HUD_OBJ = GameObject.Find("HUD");

        if (aspect != (RenderTarget.width / RenderTarget.height))
        {
            aspect = RenderTarget.width / RenderTarget.height;
            WindowMidHeight = RenderTarget.height / 2f;
            WindowMidWidth = RenderTarget.width / 2f;
        }

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

        Camera VirtualCamera = new GameObject("Virtual Camera", typeof(Camera)).GetComponent<Camera>();

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

        /*Camera BGCamera = new GameObject("BG Camera", typeof(Camera)).GetComponent<Camera>();

        BGCamera.transform.position = new Vector3(WindowMidWidth, -WindowMidHeight, -10f);
        BGCamera.orthographic = true;
        BGCamera.orthographicSize = WindowMidHeight;
        BGCamera.eventMask = BackgroundMask;
        BGCamera.cullingMask = BackgroundMask;
        BGCamera.nearClipPlane = -64f;
        BGCamera.farClipPlane = 256f;
        BGCamera.depth = -3f;
        BGCamera.targetTexture = RenderTarget;
        BGCamera.clearFlags = CameraClearFlags.Color;
        BGCamera.backgroundColor = BackgroundColor;*/

        Camera MainCamera = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener)) { tag = "MainCamera" }.GetComponent<Camera>();

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
        MainCamera.backgroundColor = BackgroundColor;

        XLeftFrame = Camera.main.transform.position.x - WindowMidWidth;
        XRightFrame = Camera.main.transform.position.x + WindowMidWidth;
        YTopFrame = Camera.main.transform.position.y + WindowMidHeight;
        YBottomFrame = Camera.main.transform.position.y - WindowMidHeight;

        /*Camera GUICamera = new GameObject("GUI Camera", typeof(Camera)).GetComponent<Camera>();

        GUICamera.transform.position = new Vector3(WindowMidWidth, -WindowMidHeight, -10f);
        GUICamera.orthographic = true;
        GUICamera.orthographicSize = WindowMidHeight;
        GUICamera.eventMask = 1 << 5;
        GUICamera.cullingMask = 1 << 5;
        GUICamera.nearClipPlane = -64f;
        GUICamera.farClipPlane = 256f;
        GUICamera.depth = -1f;
        GUICamera.targetTexture = RenderTarget;
        GUICamera.clearFlags = CameraClearFlags.Nothing;*/
    }

    public void LateUpdate()
    {
        XLeftFrame = Camera.main.transform.position.x - WindowMidWidth;
        XRightFrame = Camera.main.transform.position.x + WindowMidWidth;
        YTopFrame = Camera.main.transform.position.y + WindowMidHeight;
        YBottomFrame = Camera.main.transform.position.y - WindowMidHeight;

        if (HUD_OBJ != null)
        {
            HUD_OBJ.transform.position = new Vector3(XLeftFrame, YTopFrame, 0f);
        }
    }
}
