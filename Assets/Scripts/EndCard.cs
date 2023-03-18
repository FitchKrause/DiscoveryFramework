using UnityEngine;
using System.Collections;

public class EndCard : MonoBehaviour
{
    public float XPosition;
    public float XSpeed;
    public float OriginX;
    public float TargetX;
    public int Direction;
    public int TriggerTime;

    private SpriteRenderer render;
    private int OWidth;

    private void Start()
    {
        render = GetComponent<SpriteRenderer>();
        OWidth = (int)render.sprite.rect.width;

        TargetX = transform.position.x;
        if (Direction > 0)
        {
            XPosition = SceneController.XLeftFrame + (GameController.instance.Width + OWidth);
        }
        else
        {
            XPosition = SceneController.XLeftFrame - (GameController.instance.Width + OWidth);
        }

        OriginX = transform.position.x;

        transform.position = new Vector3(XPosition, transform.position.y);
    }

    private void FixedUpdate()
    {
        if (GoalSign.EventTimer >= TriggerTime)
        {
            if (Direction > 0)
            {
                transform.position = new Vector3(Mathf.Max(transform.position.x - (XSpeed * Time.timeScale), TargetX + ((SceneController.XRightFrame + SceneController.XLeftFrame) / 2f) - ((SceneController.XRightFrame - SceneController.XLeftFrame) / 2f)), transform.position.y);
            }
            else
            {
                transform.position = new Vector3(Mathf.Min(transform.position.x + (XSpeed * Time.timeScale), TargetX + ((SceneController.XRightFrame + SceneController.XLeftFrame) / 2f) - ((SceneController.XRightFrame - SceneController.XLeftFrame) / 2f)), transform.position.y);
            }
        }
    }
}
