using UnityEngine;
using System.Collections;

public class Parallax : MonoBehaviour
{
    [Range(0f, 1f)] public float FactorX;
    [Range(0f, 1f)] public float FactorY;
    public float XSpeed;
    public float YSpeed;
    public bool RepeatX;
    public bool RepeatY;
    public float Width;
    public float Height;
    [HideInInspector] public float AddX;
    [HideInInspector] public float AddY;
    [HideInInspector] public float InitialX;
    [HideInInspector] public float InitialY;

    private void Start()
    {
        InitialX = transform.position.x;
        InitialY = transform.position.y;

        if (GetComponent<SpriteRenderer>() != null)
        {
            Width = (int)GetComponent<SpriteRenderer>().sprite.rect.width;
            Height = (int)GetComponent<SpriteRenderer>().sprite.rect.height;
        }
        else
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).GetComponent<SpriteRenderer>() != null)
                {
                    Width = (int)transform.GetChild(i).GetComponent<SpriteRenderer>().sprite.rect.width;
                    Height = (int)transform.GetChild(i).GetComponent<SpriteRenderer>().sprite.rect.height;
                    break;
                }
            }
        }

        if (Width <= 0 || Height <= 0)
        {
            Destroy(this);
        }

        /*for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (x == 1 && y == 1) continue;
                Vector3 vector = new Vector3(Width * (x - 1), Height * (y - 1), transform.position.z);
                Instantiate(transform.GetChild(0).gameObject, vector, Quaternion.identity, transform);
            }
        }*/
    }

    private void FixedUpdate()
    {
        AddX += XSpeed * Time.timeScale;
        AddY += YSpeed * Time.timeScale;
    }

    private void LateUpdate()
    {
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

        float PosX = (((GameController.XLeftFrame + GameController.XRightFrame) / 2f) - GameController.WindowMidWidth) * FactorX;
        float PosY = (((GameController.YBottomFrame + GameController.YTopFrame) / 2f) + GameController.WindowMidHeight) * -FactorY;

        float XPosition = GameController.WindowMidWidth + (RepeatX ? Mathf.Repeat(PosX + AddX, Width) : (PosX + AddX));
        float YPosition = -GameController.WindowMidHeight - (RepeatY ? Mathf.Repeat(PosY + AddY, Height) : (PosY + AddY));

        transform.position = new Vector3(InitialX + (Camera.main.transform.position.x - XPosition),
                                         InitialY + (Camera.main.transform.position.y - YPosition),
                                         transform.position.z);
    }
}
