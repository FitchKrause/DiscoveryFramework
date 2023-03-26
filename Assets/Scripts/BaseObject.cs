#if UNITY_5_5_OR_NEWER
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#else
using UnityEngine;
using System.Collections;
#endif

public class BaseObject : MonoBehaviour
{
    public enum Directions
    {
        Left = -1,
        Right = 1
    }

    public struct SensorHit
    {
        public bool Collision;
        public float AddX;
        public float AddY;
        public RaycastHit2D Result;
    }

    public delegate void ObjectState();

    [Header("Object Values")]
    public bool AllowPhysics;
    public bool AllowCollision;
    public float WidthRadius;
    public float HeightRadius;
    public float PushRadius;
    public float WallShift;

    [HideInInspector] public string ObjectName;
    [HideInInspector] public int ObjectLoops;

    [HideInInspector] public float XPosition;
    [HideInInspector] public float YPosition;
    [HideInInspector] public float XSpeed;
    [HideInInspector] public float YSpeed;
    [HideInInspector] public float GroundSpeed;
    [HideInInspector] public float GroundAngle;
    [HideInInspector] public int Quadrant;
    [HideInInspector] public int CollisionLayer;
    [HideInInspector] public bool Ground;
    [HideInInspector] public bool Landed;
    [HideInInspector] public float LandingSpeed;
    [HideInInspector] public int LandFrame;
    [HideInInspector] public bool Fell;
    [HideInInspector] public string AnimationName;

    [Header("Object Transform Values")]
    [HideInInspector] public Directions Direction;
    [HideInInspector] public float Angle;

    [Header("Object Components")]
    [HideInInspector] public Collider2D ColliderBody;
    [HideInInspector] public Collider2D ColliderFloor;
    [HideInInspector] public Collider2D ColliderCeiling;
    [HideInInspector] public Collider2D ColliderWallLeft;
    [HideInInspector] public Collider2D ColliderWallRight;
#if UNITY_2019_1_OR_NEWER
    [HideInInspector] public ContactFilter2D filter;
#endif
    [HideInInspector] public Animator animator;
    [HideInInspector] public SpriteRenderer render;

    public virtual void ObjectCreated()
    {
        render = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        ColliderBody = GetComponent<Collider2D>();
        CollisionLayer = 1 << gameObject.layer;
#if UNITY_2019_1_OR_NEWER
        filter.SetLayerMask(CollisionLayer);
#endif

        transform.position = new Vector3(XPosition, YPosition, transform.position.z);
    }

    protected void Start()
    {
        render = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        ColliderBody = GetComponent<Collider2D>();
        CollisionLayer = 1 << gameObject.layer;
#if UNITY_2019_1_OR_NEWER
        filter.SetLayerMask(CollisionLayer);
#endif

        XPosition = transform.position.x;
        YPosition = transform.position.y;

        Direction = Directions.Right;
    }

    protected void LateUpdate()
    {
#if UNITY_5_6_OR_NEWER
        transform.SetPositionAndRotation(new Vector3(Mathf.Floor(XPosition), Mathf.Floor(YPosition), transform.position.z), Quaternion.Euler(0f, 0f, Angle));
#else
        transform.position = new Vector3(Mathf.Floor(XPosition), Mathf.Floor(YPosition), transform.position.z);
        transform.rotation = Quaternion.Euler(0f, 0f, Angle);
#endif
    }

    public void ProcessMovement()
    {
        if (Landed)
        {
            LandFrame++;
            if (LandFrame > 1)
            {
                Landed = false;
            }
        }
        else
        {
            LandFrame = 0;
        }

#if UNITY_2019_1_OR_NEWER
        if (filter.layerMask != CollisionLayer)
        {
            filter.SetLayerMask(CollisionLayer);
        }
#endif

        if (AllowPhysics && Ground)
        {
            XSpeed = GroundSpeed * Mathf.Cos(GroundAngle * Mathf.Deg2Rad);
            YSpeed = GroundSpeed * Mathf.Sin(GroundAngle * Mathf.Deg2Rad);
        }

        float AddX = XSpeed * Time.timeScale;
        float AddY = YSpeed * Time.timeScale;

        ObjectLoops = (int)(Mathf.Sqrt((XSpeed * XSpeed) + (YSpeed * YSpeed)) + 1f);

        AddX /= ObjectLoops;
        AddY /= ObjectLoops;

        for (int i = 0; i < ObjectLoops; i++)
        {
            XPosition += AddX;
            YPosition += AddY;

            if (!AllowCollision) continue;

            if ((Ground ? GroundSpeed : XSpeed) >= 0f)
            {
                SensorHit wallRight = SensorCast(new Vector2(0f, WallShift), Vector2.right, PushRadius);

                if (wallRight.Collision)
                {
                    XPosition += wallRight.AddX;
                    if (Ground)
                    {
                        YPosition += wallRight.AddY;
                        GroundSpeed = 0f;
                    }
                    else
                    {
                        XSpeed = 0f;
                    }
                }
            }

            if ((Ground ? GroundSpeed : XSpeed) <= 0f)
            {
                SensorHit wallLeft = SensorCast(new Vector2(0f, WallShift), Vector2.left, PushRadius);

                if (wallLeft.Collision)
                {
                    XPosition += wallLeft.AddX;
                    if (Ground)
                    {
                        YPosition += wallLeft.AddY;
                        GroundSpeed = 0f;
                    }
                    else
                    {
                        XSpeed = 0f;
                    }
                }
            }

            if (!Ground)
            {
                if (YSpeed > 0f)
                {
                    SensorHit ceilingLeft = SensorCast(new Vector2(-WidthRadius, 0f), Vector2.up, HeightRadius);
                    SensorHit ceilingRight = SensorCast(new Vector2(WidthRadius, 0f), Vector2.up, HeightRadius);
                    SensorHit ceilingHit = SensorWithShortestDistance(ceilingLeft, ceilingRight);

                    if (ceilingHit.Collision)
                    {
                        YPosition += ceilingHit.AddY;
                        YSpeed = 0f;
                    }
                }
                else
                {
                    SensorHit floorLeft = SensorCast(new Vector2(-WidthRadius, 0f), Vector2.down, HeightRadius, true);
                    SensorHit floorRight = SensorCast(new Vector2(WidthRadius, 0f), Vector2.down, HeightRadius, true);
                    SensorHit floorHit = SensorWithShortestDistance(floorLeft, floorRight);

                    if (floorHit.Collision)
                    {
                        YPosition += floorHit.AddY;

                        DetectAngle(true);

                        if (GroundAngle >= 45f && GroundAngle < 315f)
                        {
                            XSpeed += YSpeed * Mathf.Sin(GroundAngle * Mathf.Deg2Rad);
                        }
                        else if (GroundAngle >= 22.5f && GroundAngle < 337.5f)
                        {
                            XSpeed += YSpeed * 0.5f * Mathf.Sin(GroundAngle * Mathf.Deg2Rad);
                        }

                        GroundSpeed = XSpeed;
                        LandingSpeed = YSpeed;
                        Ground = true;
                        Landed = true;
                        Fell = false;
                    }
                }
            }
            else if (AllowPhysics)
            {
                DetectAngle();

                SensorHit floorLeft = SensorCast(new Vector2(-WidthRadius, 0f), Vector2.down, HeightRadius, true, 12f);
                SensorHit floorRight = SensorCast(new Vector2(WidthRadius, 0f), Vector2.down, HeightRadius, true, 12f);
                SensorHit floorHit = SensorWithShortestDistance(floorLeft, floorRight);

                if (floorHit.Collision)
                {
                    XPosition += floorHit.AddX;
                    YPosition += floorHit.AddY;
                }
                else
                {
                    Ground = false;
                    XSpeed = GroundSpeed * Mathf.Cos(GroundAngle * Mathf.Deg2Rad);
                    YSpeed = GroundSpeed * Mathf.Sin(GroundAngle * Mathf.Deg2Rad);
                    GroundAngle = 0f;
                    Fell = true;
                }
            }
            else
            {
                SensorHit floorLeft = SensorCast(new Vector2(-WidthRadius, 0f), Vector2.down, HeightRadius, true);
                SensorHit floorRight = SensorCast(new Vector2(WidthRadius, 0f), Vector2.down, HeightRadius, true);
                SensorHit floorHit = SensorWithShortestDistance(floorLeft, floorRight);

                if (floorHit.Collision)
                {
                    XPosition += floorHit.AddX;
                    YPosition += floorHit.AddY;
                }
                else
                {
                    Ground = false;
                    GroundAngle = 0f;
                    Fell = true;
                }
            }
        }

        ColliderFloor = OverlapBox(new Vector2(0f, -HeightRadius + (Ground ? 0f : Mathf.Min(YSpeed, 0f))), new Vector2(WidthRadius * 2f, WidthRadius), true);
        ColliderCeiling = OverlapBox(new Vector2(0f, HeightRadius + (Ground ? 0f : Mathf.Max(YSpeed, 0f))), new Vector2(WidthRadius * 2f, WidthRadius));
        ColliderWallLeft = OverlapBox(new Vector2(-PushRadius + Mathf.Min(Ground ? GroundSpeed : XSpeed, 0f), WallShift), new Vector2(PushRadius, PushRadius / 2f));
        ColliderWallRight = OverlapBox(new Vector2(PushRadius + Mathf.Max(Ground ? GroundSpeed : XSpeed, 0f), WallShift), new Vector2(PushRadius, PushRadius / 2f));
    }

    public void DetectAngle(bool useNormals = false)
    {
        if (!AllowPhysics) return;

        SensorHit angleLeft = SensorCast(new Vector2(-WidthRadius, 0f), Vector2.down, HeightRadius + 20f, true);
        SensorHit angleRight = SensorCast(new Vector2(WidthRadius, 0f), Vector2.down, HeightRadius + 20f, true);

        if (angleLeft.Collision && angleRight.Collision)
        {
            float PointX = (angleRight.Result.point - angleLeft.Result.point).x;
            float PointY = (angleRight.Result.point - angleLeft.Result.point).y;

            GroundAngle = AngleCalculator(PointX, PointY);
        }
        else if (useNormals)
        {
            if (!angleLeft.Collision && angleRight.Collision)
            {
                float PointX = angleRight.Result.normal.x;
                float PointY = angleRight.Result.normal.y;

                GroundAngle = AngleCalculatorAlt(PointX, PointY);
            }
            else if (angleLeft.Collision && !angleRight.Collision)
            {
                float PointX = angleLeft.Result.normal.x;
                float PointY = angleLeft.Result.normal.y;

                GroundAngle = AngleCalculatorAlt(PointX, PointY);
            }
        }

        Quadrant = ((int)GroundAngle + (360 + 45)) % 360 / 90;
    }

    public float AngleCalculator(float angleCalculatorX, float angleCalculatorY)
    {
        float angleCalculatorResult = Mathf.Atan2(angleCalculatorY, angleCalculatorX) * Mathf.Rad2Deg;
        return (720f + angleCalculatorResult) % 360f;
    }

    public float AngleCalculatorAlt(float angleCalculatorX, float angleCalculatorY)
    {
        float angleCalculatorResult = Mathf.Atan2(angleCalculatorX, angleCalculatorY) * Mathf.Rad2Deg;
        return (720f - angleCalculatorResult) % 360f;
    }

    public SensorHit SensorWithShortestDistance(SensorHit left, SensorHit right)
    {
        if (left.Collision && right.Collision)
        {
            if (left.Result.distance < right.Result.distance)
            {
                return left;
            }
            else
            {
                return right;
            }
        }
        else if (!left.Collision && right.Collision)
        {
            return right;
        }
        else if (left.Collision && !right.Collision)
        {
            return left;
        }

        return default(SensorHit);
    }

    public SensorHit SensorCast(Vector2 offset, Vector2 direction, float distance, bool allowPlatforms = false, float extension = 0f)
    {
        SensorHit result = default(SensorHit);

#if UNITY_2019_1_OR_NEWER
        List<RaycastHit2D> results = new List<RaycastHit2D>();
#else
        RaycastHit2D[] results = new RaycastHit2D[16];
#endif

        Vector2 start = Quaternion.Euler(0f, 0f, GroundAngle) * offset;
        Vector2 dir = Quaternion.Euler(0f, 0f, GroundAngle) * direction;
        Vector2 anchor = start + (dir * distance);
        Vector2 end = start + (dir * (distance + extension));

        start.x += XPosition;
        start.y += YPosition;
        anchor.x += XPosition;
        anchor.y += YPosition;
        end.x += XPosition;
        end.y += YPosition;

        int num = -1;
        float num2 = float.MaxValue;

#if UNITY_2019_1_OR_NEWER
        for (int i = 0; i < Physics2D.Linecast(start, end, filter, results); i++)
#else
        for (int i = 0; i < Physics2D.LinecastNonAlloc(start, end, results, CollisionLayer); i++)
#endif
        {
            if (results[i].collider == ColliderBody) continue;

            if (results[i].collider.CompareTag("Solid") || allowPlatforms &&
                results[i].collider.CompareTag("Platform") && YSpeed <= 0f && (YPosition - HeightRadius) > (results[i].transform.position.y - 4f))
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

        Debug.DrawLine(start, anchor, Color.magenta);

        if (num > -1)
        {
            result.Collision = true;
            result.AddX = (results[num].point - anchor).x;
            result.AddY = (results[num].point - anchor).y;
            result.Result = results[num];
        }

        return result;
    }

    public Collider2D OverlapBox(Vector2 offset, Vector2 size, bool allowPlatforms = false)
    {
#if UNITY_2019_1_OR_NEWER
        List<Collider2D> results = new List<Collider2D>();
#else
        Collider2D[] results = new Collider2D[16];
#endif

        Vector2 pos = Quaternion.Euler(0f, 0f, GroundAngle) * offset;
        pos.x += XPosition;
        pos.y += YPosition;

        int num = -1;

#if UNITY_2019_1_OR_NEWER
        for (int i = 0; i < Physics2D.OverlapBox(pos, size, GroundAngle, filter, results); i++)
#else
        for (int i = 0; i < Physics2D.OverlapBoxNonAlloc(pos, size, GroundAngle, results, CollisionLayer); i++)
#endif
        {
            if (results[i] == ColliderBody) continue;

            if (results[i].CompareTag("Solid") || allowPlatforms &&
                results[i].CompareTag("Platform") && YSpeed <= 0f && (YPosition - HeightRadius) > (results[i].transform.position.y - 4f))
            {
                num = i;
                break;
            }
            else
            {
                continue;
            }
        }

        Vector2 topLeft = Quaternion.Euler(0f, 0f, GroundAngle) * (new Vector2(-size.x, size.y) / 2f);
        Vector2 topRight = Quaternion.Euler(0f, 0f, GroundAngle) * (new Vector2(size.x, size.y) / 2f);
        Vector2 bottomRight = Quaternion.Euler(0f, 0f, GroundAngle) * (new Vector2(size.x, -size.y) / 2f);
        Vector2 bottomLeft = Quaternion.Euler(0f, 0f, GroundAngle) * (new Vector2(-size.x, -size.y) / 2f);

        Debug.DrawLine(pos + topLeft, pos + topRight, Color.magenta);
        Debug.DrawLine(pos + topRight, pos + bottomRight, Color.magenta);
        Debug.DrawLine(pos + bottomRight, pos + bottomLeft, Color.magenta);
        Debug.DrawLine(pos + bottomLeft, pos + topLeft, Color.magenta);

        if (num > -1)
        {
            return results[num];
        }

        return null;
    }

    public Collider2D OverlapPoint(Vector2 offset, bool allowPlatforms = false)
    {
#if UNITY_2019_1_OR_NEWER
        List<Collider2D> results = new List<Collider2D>();
#else
        Collider2D[] results = new Collider2D[16];
#endif

        Vector2 pos = Quaternion.Euler(0f, 0f, GroundAngle) * offset;
        pos.x += XPosition;
        pos.y += YPosition;
        int num = -1;

#if UNITY_2019_1_OR_NEWER
        for (int i = 0; i < Physics2D.OverlapPoint(pos, filter, results); i++)
#else
        for (int i = 0; i < Physics2D.OverlapPointNonAlloc(pos, results, CollisionLayer); i++)
#endif
        {
            if (results[i] == ColliderBody) continue;

            if (results[i].CompareTag("Solid") || allowPlatforms &&
                results[i].CompareTag("Platform") && YSpeed <= 0f && (YPosition - HeightRadius) > (results[i].transform.position.y - 4f))
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
