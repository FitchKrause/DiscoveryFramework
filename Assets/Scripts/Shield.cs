using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour
{
    public enum Shield_Types
    {
        Normal = 1,
        Flame = 2,
        Magnetic = 3,
        Aquatic = 4
    }
    public Shield_Types ShieldType;

    private PlayerPhysics player;
    private Renderer render;

    private void Start()
    {
        player = FindObjectOfType<PlayerPhysics>();
        render = GetComponent<Renderer>();
    }

    private void FixedUpdate()
    {
        render.enabled = player.Shield == (int)ShieldType;
        if (render.enabled)
        {
            render.sortingLayerName = player.render.sortingLayerName;
        }
    }

    private void LateUpdate()
    {
        float PosX = player.XPosition - (Mathf.Sin(player.AnimationAngle * Mathf.Deg2Rad) * 6f);
        float PosY = player.YPosition + (Mathf.Cos(player.AnimationAngle * Mathf.Deg2Rad) * 6f);
        transform.position = new Vector3(PosX, PosY);
    }
}
