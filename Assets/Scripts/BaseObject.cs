using UnityEngine;
using System.Collections;

public class BaseObject : MonoBehaviour
{
    public const int RESULT_COUNT = 9;

    [HideInInspector] public string ObjectName;
    [HideInInspector] public int ObjectLoops;
    [HideInInspector] public float XPosition;
    [HideInInspector] public float YPosition;
    [HideInInspector] public float XSpeed;
    [HideInInspector] public float YSpeed;
    [HideInInspector] public float GroundSpeed;
    [HideInInspector] public float GroundAngle;
    [HideInInspector] public bool Ground;
    [HideInInspector] public int CollisionLayer;
    [HideInInspector] public float Angle;
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

        float AddX = XSpeed * GameController.DeltaTime;
        float AddY = YSpeed * GameController.DeltaTime;

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
        transform.position = new Vector3(Mathf.Floor(XPosition), Mathf.Floor(YPosition), transform.position.z);
        transform.rotation = Quaternion.Euler(0f, 0f, Angle);
    }

    public RaycastHit2D SensorCast(Vector2 offset, Vector2 direction, float distance, bool platforms = true)
    {
        RaycastHit2D[] results = new RaycastHit2D[RESULT_COUNT];

        Vector2 pos = Quaternion.Euler(0f, 0f, GroundAngle) * offset;
        Vector2 dir = Quaternion.Euler(0f, 0f, GroundAngle) * direction;

        Debug.DrawRay(new Vector2(XPosition, YPosition) + pos, dir * distance);

        int num = -1;
        float num2 = float.MaxValue;

        for (int i = 0; i < Physics2D.RaycastNonAlloc(new Vector2(XPosition, YPosition) + pos, dir, results, distance, 1 << CollisionLayer); i++)
        {
            if (results[i].collider == ColliderBody) continue;

            if (results[i].collider.tag == "Solid" ||
                results[i].collider.tag == "Platform" && platforms && YSpeed <= 0f && (YPosition - HeightRadius) > (results[i].collider.transform.position.y - 4f))
            {
                if (results[i].distance < num2)
                {
                    num = i;
                    num2 = results[i].distance;
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

        if (num > -1)
        {
            return results[num];
        }

        return default(RaycastHit2D);
    }

    public Collider2D OverlapBox(Vector2 offset, Vector2 size, bool platforms = true)
    {
        Collider2D[] results = new Collider2D[RESULT_COUNT];

        Vector2 pos = Quaternion.Euler(0f, 0f, GroundAngle) * offset;
        Vector2 pos2 = new Vector2(XPosition, YPosition) + pos;

        Vector2 vector = Quaternion.Euler(0f, 0f, GroundAngle) * (new Vector2(-size.x, size.y) / 2f);
        Vector2 vector2 = Quaternion.Euler(0f, 0f, GroundAngle) * (new Vector2(size.x, size.y) / 2f);
        Vector2 vector3 = Quaternion.Euler(0f, 0f, GroundAngle) * (new Vector2(size.x, -size.y) / 2f);
        Vector2 vector4 = Quaternion.Euler(0f, 0f, GroundAngle) * (new Vector2(-size.x, -size.y) / 2f);

        Debug.DrawLine(pos2 + vector, pos2 + vector2);
        Debug.DrawLine(pos2 + vector2, pos2 + vector3);
        Debug.DrawLine(pos2 + vector3, pos2 + vector4);
        Debug.DrawLine(pos2 + vector4, pos2 + vector);

        int num = -1;

        for (int i = 0; i < Physics2D.OverlapBoxNonAlloc(pos2, size, GroundAngle, results, 1 << CollisionLayer); i++)
        {
            if (results[i] == ColliderBody) continue;

            if (results[i].tag == "Solid" ||
                results[i].tag == "Platform" && platforms && YSpeed <= 0f && (YPosition - HeightRadius) > (results[i].transform.position.y - 4f))
            {
                num = i;
                break;
            }
            else
            {
                continue;
            }
        }

        if (num > -1)
        {
            return results[num];
        }

        return null;
    }

    public Collider2D OverlapPoint(Vector2 offset, bool platforms = true)
    {
        Collider2D[] results = new Collider2D[RESULT_COUNT];

        Vector2 pos = Quaternion.Euler(0f, 0f, GroundAngle) * offset;
        Vector2 pos2 = new Vector2(XPosition, YPosition) + pos;

        int num = -1;

        for (int i = 0; i < Physics2D.OverlapPointNonAlloc(pos2, results, 1 << CollisionLayer); i++)
        {
            if (results[i] == ColliderBody) continue;

            if (results[i].tag == "Solid" ||
                results[i].tag == "Platform" && platforms && YSpeed <= 0f && (YPosition - HeightRadius) > (results[i].transform.position.y - 4f))
            {
                num = i;
                break;
            }
            else
            {
                continue;
            }
        }

        if (num > -1)
        {
            return results[num];
        }

        return null;
    }
}
