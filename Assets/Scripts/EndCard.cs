using UnityEngine;
using System.Collections;

public class EndCard : MonoBehaviour
{
    public float XSpeed;
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
    }

    private void FixedUpdate()
    {
        if (GoalSign.EventTimer >= TriggerTime)
        {
            if (Direction > 0)
            {
                transform.position = new Vector3(Mathf.Max(transform.position.x - (XSpeed * Time.timeScale), TargetX + ((GameController.XRightFrame + GameController.XLeftFrame) / 2f) - ((GameController.XRightFrame - GameController.XLeftFrame) / 2f)), transform.position.y);
            }
            else
            {
                transform.position = new Vector3(Mathf.Min(transform.position.x + (XSpeed * Time.timeScale), TargetX + ((GameController.XRightFrame + GameController.XLeftFrame) / 2f) - ((GameController.XRightFrame - GameController.XLeftFrame) / 2f)), transform.position.y);
            }
        }
        else
        {
            if (Direction > 0)
            {
                transform.position = new Vector3(GameController.XLeftFrame + (GameController.WindowWidth + OWidth), transform.position.y);
            }
            else
            {
                transform.position = new Vector3(GameController.XLeftFrame - (GameController.WindowWidth + OWidth), transform.position.y);
            }
        }
    }
}
