using UnityEngine;
using System.Collections;

public class BaseObject : MonoBehaviour
{
    [HideInInspector] public string ObjectName;
    [HideInInspector] public int ObjectLoops;
    [HideInInspector] public float XPosition;
    [HideInInspector] public float YPosition;
    [HideInInspector] public float XSpeed;
    [HideInInspector] public float YSpeed;
    [HideInInspector] public float GroundSpeed;
    [HideInInspector] public float GroundAngle;
    [HideInInspector] public int Quadrant;
    [HideInInspector] public bool Ground;
    [HideInInspector] public int CollisionLayer;
    [HideInInspector] public float AnimationAngle;
    [HideInInspector] public Collider2D ColliderBody;
    [HideInInspector] public SpriteRenderer render;
    [HideInInspector] public Animator animator;
    [Header("Object Values")]
    public float WidthRadius;
    public float HeightRadius;
    public float WallShift;

    public virtual void ObjectCreated()
    {
        render = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        ColliderBody = GetComponent<Collider2D>();

        transform.position = new Vector3(XPosition, YPosition, transform.position.z);
    }

    protected void Start()
    {
        render = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        ColliderBody = GetComponent<Collider2D>();
        CollisionLayer = gameObject.layer;

        XPosition = transform.position.x;
        YPosition = transform.position.y;
    }

    public void ProcessMovement()
    {
        if (Ground)
        {
            XSpeed = GroundSpeed * Mathf.Cos(GroundAngle * Mathf.Deg2Rad);
            YSpeed = GroundSpeed * Mathf.Sin(GroundAngle * Mathf.Deg2Rad);
        }

        float AddX = XSpeed;
        float AddY = YSpeed;

        ObjectLoops = (int)(Mathf.Sqrt((XSpeed * XSpeed) + (YSpeed * YSpeed)) + 1f);

        AddX /= ObjectLoops;
        AddY /= ObjectLoops;

        for (int i = 0; i < ObjectLoops; i++)
        {
            XPosition += AddX;
            YPosition += AddY;
        }
    }
    
    protected void LateUpdate()
    {
        transform.position = new Vector3(XPosition, YPosition, transform.position.z);
        transform.rotation = Quaternion.Euler(0f, 0f, AnimationAngle);
    }

    public RaycastHit2D SensorCast(Vector2 offset, Vector2 direction, float distance)
    {
        RaycastHit2D result = default(RaycastHit2D);
        float hitDistance = 32f;

        Vector2 pos = Quaternion.Euler(0f, 0f, GroundAngle) * offset;
        Vector2 dir = Quaternion.Euler(0f, 0f, GroundAngle) * direction;

        Debug.DrawRay(new Vector2(XPosition, YPosition) + pos, dir * distance);

        foreach (RaycastHit2D hit in Physics2D.RaycastAll(new Vector2(XPosition, YPosition) + pos, dir, distance, 1 << CollisionLayer))
        {
            if (hit.collider == ColliderBody) continue;

            if (hit.collider.tag == "Solid" ||
                hit.collider.tag == "Platform" && direction.y < 0f && YSpeed <= 0f && (YPosition - HeightRadius) > (hit.collider.transform.position.y - 4f))
            {
                if (hit.distance < hitDistance)
                {
                    result = hit;
                    hitDistance = hit.distance;
                }
                else
                {
                    break;
                }
            }
            else
            {
                continue;
            }
        }

        return result;
    }
}
