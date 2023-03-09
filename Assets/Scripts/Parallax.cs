using UnityEngine;
using System.Collections;

public class Parallax : MonoBehaviour
{
    [HideInInspector] public float InitialX;
    [HideInInspector] public float InitialY;

    [Range(0f, 1f)]
    public float XFactor, YFactor;

    public float XSpeed;
    public float YSpeed;
    public float AddX;
    public float AddY;

    public bool RepeatX;
    public bool RepeatY;

    public int Width;
    public int Height;

    private void Start()
    {
        InitialX = transform.position.x;
        InitialY = transform.position.y;

        Width = (int)transform.GetChild(0).GetComponent<SpriteRenderer>().sprite.rect.width;
        Height = (int)transform.GetChild(0).GetComponent<SpriteRenderer>().sprite.rect.height;

        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (x == 1 && y == 1) continue;
                Vector3 vector = new Vector3(Width * (x - 1), Height * (y - 1), transform.position.z);
                Instantiate(transform.GetChild(0).gameObject, vector, Quaternion.identity, transform);
            }
        }
    }

    private void LateUpdate()
    {
        if (!StageController.Paused)
        {
            AddX += XSpeed;
            AddY += YSpeed;
        }

        if (RepeatX)
        {
            if (AddX < 0f)
            {
                AddX += Width;
            }
            else if (AddX > Width)
            {
                AddX -= Width;
            }
        }
        if (RepeatY)
        {
            if (AddY < 0f)
            {
                AddY += Height;
            }
            else if (AddY > Height)
            {
                AddY -= Height;
            }
        }

        float PosX = (((PixelCamera.XLeftFrame + PixelCamera.XRightFrame) / 2f) - ((PixelCamera.XRightFrame - PixelCamera.XLeftFrame) / 2f)) * (1f - XFactor);
        float PosY = (((PixelCamera.YBottomFrame + PixelCamera.YTopFrame) / 2f) + ((PixelCamera.YTopFrame - PixelCamera.YBottomFrame) / 2f)) * (1f - YFactor);

        float XPosition = InitialX + PixelCamera.XLeftFrame - (RepeatX ? Mathf.Repeat(PosX + AddX, Width) : (PosX + AddX));
        float YPosition = InitialY + PixelCamera.YTopFrame - (RepeatY ? Mathf.Repeat(PosY + AddY, Height) : (PosY + AddY));

        transform.position = new Vector3(XPosition, YPosition, transform.position.z);
    }
}
