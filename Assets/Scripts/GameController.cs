using UnityEngine;
using System.IO;
using System;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public RenderTexture TargetTexture;

    public static int Frame;
    private float FrameTimer;

    public static int WindowWidth;
    public static int WindowHeight;
    public static int WindowMidWidth;
    public static int WindowMidHeight;

    public static float XLeftFrame;
    public static float XRightFrame;
    public static float YTopFrame;
    public static float YBottomFrame;

    public int CurrentSlot = 1;
    public static int GameCharacter = 0;
    public static int Lives = 3;
    public static int Score = 0;
    private const string DATA_SEPARATOR = "#";

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(gameObject);

        SetGameSpeed(1f);
    }

    private void Start()
    {
        if (TargetTexture == null)
        {
            TargetTexture = Resources.Load("Pixel Art Buffer") as RenderTexture;
        }

        WindowMidWidth = (WindowWidth = TargetTexture.width) / 2;
        WindowMidHeight = (WindowHeight = TargetTexture.height) / 2;

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

        GameObject VirtualScreen = new GameObject("Virtual Screen", typeof(MeshFilter), typeof(MeshRenderer)) { layer = 31 };
        VirtualScreen.transform.position = new Vector3(-WindowWidth, WindowHeight, 0f);
        VirtualScreen.GetComponent<MeshFilter>().mesh = mesh;
        VirtualScreen.GetComponent<MeshRenderer>().material = Resources.Load("Unlit/Texture") as Material;
        VirtualScreen.GetComponent<MeshRenderer>().material.mainTexture = TargetTexture;
        DontDestroyOnLoad(VirtualScreen);

        Camera VirtualCamera = new GameObject("Virtual Camera").AddComponent<Camera>();
        VirtualCamera.transform.position = new Vector3(-WindowWidth, WindowHeight, -10f);
        VirtualCamera.orthographic = true;
        VirtualCamera.orthographicSize = WindowMidHeight;
        VirtualCamera.eventMask = 1 << 31;
        VirtualCamera.cullingMask = 1 << 31;
        VirtualCamera.nearClipPlane = -64f;
        VirtualCamera.farClipPlane = 256f;
        VirtualCamera.depth = 0f;
        VirtualCamera.clearFlags = CameraClearFlags.Color;
        VirtualCamera.backgroundColor = Color.black;
        DontDestroyOnLoad(VirtualCamera.gameObject);

        if (CurrentSlot >= 1)
        {
            if (!File.Exists(Application.dataPath + string.Format("/save{0}.sav", CurrentSlot)))
            {
                string[] dataContent = new string[]
                {
                    "NULL",
                    //Score.ToString(),
                    GameCharacter.ToString(),
                    (Lives >= 1) ? Lives.ToString() : "3",
                };
                string dataTXT = string.Join(DATA_SEPARATOR, dataContent);
                File.WriteAllText(Application.dataPath + string.Format("/save{0}.sav", CurrentSlot), dataTXT);
            }
            else if (CurrentSlot >= 1)
            {
                string dataTXT = File.ReadAllText(Application.dataPath + string.Format("/save{0}.sav", CurrentSlot));
                string[] dataContent = dataTXT.Split(new[] { DATA_SEPARATOR }, StringSplitOptions.None);

                //Score = int.Parse(dataContent[0]);
                GameCharacter = int.Parse(dataContent[1]);

                if (Lives > 0)
                {
                    Lives = int.Parse(dataContent[2]);
                }
                else
                {
                    Lives = 3;
                }
            }
        }
    }

    private void OnApplicationQuit()
    {
        if (CurrentSlot >= 1)
        {
            string[] dataContent = new string[]
            {
                "NULL",
                //Score.ToString(),
                GameCharacter.ToString(),
                (Lives >= 1) ? Lives.ToString() : "3",
            };
            string dataTXT = string.Join(DATA_SEPARATOR, dataContent);
            File.WriteAllText(Application.dataPath + string.Format("/save{0}.sav", CurrentSlot), dataTXT);
        }
    }

    private void Update()
    {
        if (Camera.main.targetTexture != TargetTexture)
        {
            Camera.main.targetTexture = TargetTexture;
        }
    }

    private void FixedUpdate()
    {
        Score = Mathf.Clamp(Score, 0, 9999999);
        Lives = Mathf.Clamp(Lives, 0, 99);

        FrameTimer -= Time.timeScale;

        if (FrameTimer > 0f)
        {
            Frame = 0;
        }
        else
        {
            Frame = 1;
            FrameTimer = 1f;
        }
    }

    private void LateUpdate()
    {
        XLeftFrame = Camera.main.transform.position.x - WindowMidWidth;
        XRightFrame = Camera.main.transform.position.x + WindowMidWidth;
        YTopFrame = Camera.main.transform.position.y + WindowMidHeight;
        YBottomFrame = Camera.main.transform.position.y - WindowMidHeight;
    }

    public static void SetGameSpeed(float speed)
    {
        Time.timeScale = speed;
        Time.fixedDeltaTime = 1f / 60f * Time.timeScale;
    }
}
