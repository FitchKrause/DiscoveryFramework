#if UNITY_5_5_OR_NEWER
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#else
using UnityEngine;
using System.Collections;
#endif

public delegate void ObjectState();

public class BaseObject : MonoBehaviour
{
    [Header("Object Values")]
    public bool AllowMovement;
    public bool AllowCollision;
    public bool DisableFloorCollision;
    public bool DisableCeilingCollision;
    public bool DisableWallCollision;
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
    [HideInInspector] public int CeilingLand;
    [HideInInspector] public float LandingSpeed;
    [HideInInspector] public int LandFrame;
    [HideInInspector] public string CurrentAnimation;

    [Header("Object Transform Values")]
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
    }

    protected void LateUpdate()
    {
#if UNITY_5_6_OR_NEWER
        transform.SetPositionAndRotation(new Vector3(XPosition, YPosition, transform.position.z), Quaternion.Euler(0f, 0f, Angle));
#else
        transform.position = new Vector3(XPosition, YPosition, transform.position.z);
        transform.rotation = Quaternion.Euler(0f, 0f, Angle);
#endif
    }

    public void ProcessMovement()
    {
        if (!AllowMovement)
        {
            return;
        }
        else
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

            if (Ground)
            {
                XSpeed = GroundSpeed * Mathf.Cos(GroundAngle * Mathf.Deg2Rad);
                YSpeed = GroundSpeed * Mathf.Sin(GroundAngle * Mathf.Deg2Rad);
            }

            float AddX = XSpeed * Time.timeScale;
            float AddY = YSpeed * Time.timeScale;

            ObjectLoops = (int)(Mathf.Sqrt((XSpeed * XSpeed) + (YSpeed * YSpeed)) + 1f);

            AddX /= ObjectLoops;
            AddY /= ObjectLoops;

            ColliderFloor = ColliderCeiling = ColliderWallLeft = ColliderWallRight = null;

            for (int i = 0; i < ObjectLoops; i++)
            {
                XPosition += AddX;
                YPosition += AddY;

                if (AllowCollision)
                {
                    if (!DisableWallCollision)
                    {
                        if ((Ground ? GroundSpeed : XSpeed) >= 0f)
                        {
                            RaycastHit2D rightWall = SensorCast(new Vector2(0f, WallShift), Vector2.right, PushRadius);

                            if (rightWall)
                            {
                                Vector2 vector = Quaternion.Euler(0f, 0f, GroundAngle) * new Vector2(PushRadius, WallShift);
                                vector.x += XPosition;
                                vector.y += YPosition;

                                XPosition += (rightWall.point - vector).x;
                                if (Ground)
                                {
                                    YPosition += (rightWall.point - vector).y;
                                    GroundSpeed = 0f;
                                }
                                else
                                {
                                    XSpeed = 0f;
                                }
                            }

                            ColliderWallRight = SensorCast(new Vector2(0f, WallShift), Vector2.right, PushRadius, false, Mathf.Abs(Ground ? GroundSpeed : XSpeed)).collider;
                        }


                        if ((Ground ? GroundSpeed : XSpeed) <= 0f)
                        {
                            RaycastHit2D leftWall = SensorCast(new Vector2(0f, WallShift), Vector2.left, PushRadius);

                            if (leftWall)
                            {
                                Vector2 vector = Quaternion.Euler(0f, 0f, GroundAngle) * new Vector2(-PushRadius, WallShift);
                                vector.x += XPosition;
                                vector.y += YPosition;

                                XPosition += (leftWall.point - vector).x;
                                if (Ground)
                                {
                                    YPosition += (leftWall.point - vector).y;
                                    GroundSpeed = 0f;
                                }
                                else
                                {
                                    XSpeed = 0f;
                                }
                            }

                            ColliderWallLeft = SensorCast(new Vector2(0f, WallShift), Vector2.left, PushRadius, false, Mathf.Abs(Ground ? GroundSpeed : XSpeed)).collider;
                        }
                    }

                    if (!(DisableFloorCollision || !Ground && YSpeed > 0f))
                    {
                        RaycastHit2D leftFloor = SensorCast(new Vector2(-WidthRadius, 0f), Vector2.down, HeightRadius, true);
                        RaycastHit2D rightFloor = SensorCast(new Vector2(WidthRadius, 0f), Vector2.down, HeightRadius, true);

                        RaycastHit2D leftAngle = SensorCast(new Vector2(-WidthRadius, 0f), Vector2.down, HeightRadius + 24f, true);
                        RaycastHit2D rightAngle = SensorCast(new Vector2(WidthRadius, 0f), Vector2.down, HeightRadius + 24f, true);

                        if (!Ground && YSpeed <= 0f && (leftFloor || rightFloor))
                        {
                            if (leftAngle && rightAngle)
                            {
                                float angleX = (rightAngle.point - leftAngle.point).x;
                                float angleY = (rightAngle.point - leftAngle.point).y;
                                GroundAngle = Utils.AngleCalculator(angleX, angleY);
                            }
                            else
                            {
                                if (rightAngle)
                                {
                                    float angleX = rightAngle.normal.x;
                                    float angleY = rightAngle.normal.y;
                                    GroundAngle = Utils.AngleCalculator(angleX, angleY, true);
                                }
                                else if (leftAngle)
                                {
                                    float angleX = leftAngle.normal.x;
                                    float angleY = leftAngle.normal.y;
                                    GroundAngle = Utils.AngleCalculator(angleX, angleY, true);
                                }
                            }

                            if (GroundAngle >= 45f && GroundAngle <= 315f)
                            {
                                XSpeed += YSpeed * Mathf.Sin(GroundAngle * Mathf.Deg2Rad);
                            }
                            else if (GroundAngle >= 22.5f && GroundAngle <= 337.5f)
                            {
                                XSpeed += YSpeed * 0.5f * Mathf.Sin(GroundAngle * Mathf.Deg2Rad);
                            }

                            CeilingLand = 0;
                            GroundSpeed = XSpeed;
                            Ground = Landed = true;
                        }

                        if (Ground)
                        {
                            if (leftAngle && rightAngle)
                            {
                                float angleX = (rightAngle.point - leftAngle.point).x;
                                float angleY = (rightAngle.point - leftAngle.point).y;
                                GroundAngle = Utils.AngleCalculator(angleX, angleY);
                            }

                            RaycastHit2D leftSlope = SensorCast(new Vector2(-WidthRadius, 0f), Vector2.down, HeightRadius, true, 12f);
                            RaycastHit2D rightSlope = SensorCast(new Vector2(WidthRadius, 0f), Vector2.down, HeightRadius, true, 12f);

                            if (leftSlope && rightSlope)
                            {
                                if (leftSlope.distance < rightSlope.distance)
                                {
                                    Vector2 vector = Quaternion.Euler(0f, 0f, GroundAngle) * new Vector2(-WidthRadius, -HeightRadius);
                                    vector.x += XPosition;
                                    vector.y += YPosition;

                                    XPosition += (leftSlope.point - vector).x;
                                    YPosition += (leftSlope.point - vector).y;
                                }
                                else
                                {
                                    Vector2 vector = Quaternion.Euler(0f, 0f, GroundAngle) * new Vector2(WidthRadius, -HeightRadius);
                                    vector.x += XPosition;
                                    vector.y += YPosition;

                                    XPosition += (rightSlope.point - vector).x;
                                    YPosition += (rightSlope.point - vector).y;
                                }
                            }
                            else
                            {
                                if (rightSlope)
                                {
                                    Vector2 vector = Quaternion.Euler(0f, 0f, GroundAngle) * new Vector2(WidthRadius, -HeightRadius);
                                    vector.x += XPosition;
                                    vector.y += YPosition;

                                    XPosition += (rightSlope.point - vector).x;
                                    YPosition += (rightSlope.point - vector).y;
                                }
                                else if (leftSlope)
                                {
                                    Vector2 vector = Quaternion.Euler(0f, 0f, GroundAngle) * new Vector2(-WidthRadius, -HeightRadius);
                                    vector.x += XPosition;
                                    vector.y += YPosition;

                                    XPosition += (leftSlope.point - vector).x;
                                    YPosition += (leftSlope.point - vector).y;
                                }
                                else
                                {
                                    Ground = false;
                                    XSpeed = GroundSpeed * Mathf.Cos(GroundAngle * Mathf.Deg2Rad);
                                    YSpeed = GroundSpeed * Mathf.Sin(GroundAngle * Mathf.Deg2Rad);
                                    GroundAngle = 0f;
                                    CeilingLand = -1;
                                }
                            }
                        }

                        if (leftFloor && rightFloor)
                        {
                            if (leftFloor.distance < rightFloor.distance)
                            {
                                Vector2 vector = Quaternion.Euler(0f, 0f, GroundAngle) * new Vector2(-WidthRadius, -HeightRadius);
                                vector.x += XPosition;
                                vector.y += YPosition;

                                XPosition += (leftFloor.point - vector).x;
                                YPosition += (leftFloor.point - vector).y;
                            }
                            else
                            {
                                Vector2 vector = Quaternion.Euler(0f, 0f, GroundAngle) * new Vector2(WidthRadius, -HeightRadius);
                                vector.x += XPosition;
                                vector.y += YPosition;

                                XPosition += (rightFloor.point - vector).x;
                                YPosition += (rightFloor.point - vector).y;
                            }
                        }
                        else
                        {
                            if (rightFloor)
                            {
                                Vector2 vector = Quaternion.Euler(0f, 0f, GroundAngle) * new Vector2(WidthRadius, -HeightRadius);
                                vector.x += XPosition;
                                vector.y += YPosition;

                                XPosition += (rightFloor.point - vector).x;
                                YPosition += (rightFloor.point - vector).y;
                            }
                            else if (leftFloor)
                            {
                                Vector2 vector = Quaternion.Euler(0f, 0f, GroundAngle) * new Vector2(-WidthRadius, -HeightRadius);
                                vector.x += XPosition;
                                vector.y += YPosition;

                                XPosition += (leftFloor.point - vector).x;
                                YPosition += (leftFloor.point - vector).y;
                            }
                        }

                        ColliderFloor = SensorCast(new Vector2(-WidthRadius, 0f), Vector2.down, HeightRadius, true, Mathf.Abs(Ground ? 12f : YSpeed)).collider ??
                                        SensorCast(new Vector2(WidthRadius, 0f), Vector2.down, HeightRadius, true, Mathf.Abs(Ground ? 12f : YSpeed)).collider;
                    }

                    if (!DisableCeilingCollision && !Ground && YSpeed > 0f)
                    {
                        RaycastHit2D leftCeiling = SensorCast(new Vector2(-WidthRadius, 0f), Vector2.up, HeightRadius);
                        RaycastHit2D rightCeiling = SensorCast(new Vector2(WidthRadius, 0f), Vector2.up, HeightRadius);

                        if (YSpeed > 0f && (leftCeiling || rightCeiling))
                        {
                            if (leftCeiling && rightCeiling)
                            {
                                if (leftCeiling.distance < rightCeiling.distance)
                                {
                                    Vector2 vector = Quaternion.Euler(0f, 0f, GroundAngle) * new Vector2(-WidthRadius, HeightRadius);
                                    vector.x += XPosition;
                                    vector.y += YPosition;

                                    XPosition += (leftCeiling.point - vector).x;
                                    YPosition += (leftCeiling.point - vector).y;
                                }
                                else
                                {
                                    Vector2 vector = Quaternion.Euler(0f, 0f, GroundAngle) * new Vector2(WidthRadius, HeightRadius);
                                    vector.x += XPosition;
                                    vector.y += YPosition;

                                    XPosition += (rightCeiling.point - vector).x;
                                    YPosition += (rightCeiling.point - vector).y;
                                }
                            }
                            else
                            {
                                if (rightCeiling)
                                {
                                    Vector2 vector = Quaternion.Euler(0f, 0f, GroundAngle) * new Vector2(WidthRadius, HeightRadius);
                                    vector.x += XPosition;
                                    vector.y += YPosition;

                                    XPosition += (rightCeiling.point - vector).x;
                                    YPosition += (rightCeiling.point - vector).y;
                                }
                                else if (leftCeiling)
                                {
                                    Vector2 vector = Quaternion.Euler(0f, 0f, GroundAngle) * new Vector2(-WidthRadius, HeightRadius);
                                    vector.x += XPosition;
                                    vector.y += YPosition;

                                    XPosition += (leftCeiling.point - vector).x;
                                    YPosition += (leftCeiling.point - vector).y;
                                }
                            }

                            if (CeilingLand == 0)
                            {
                                CeilingLand = 1;
                                GroundAngle = 180f;

                                if (CeilingLand == 1)
                                {
                                    RaycastHit2D leftFloor = SensorCast(new Vector2(-WidthRadius, 0f), Vector2.down, HeightRadius, false, 12f);
                                    RaycastHit2D rightFloor = SensorCast(new Vector2(WidthRadius, 0f), Vector2.down, HeightRadius, false, 12f);

                                    if (leftFloor || rightFloor)
                                    {
                                        CeilingLand = 2;
                                    }

                                    if (CeilingLand == 2)
                                    {
                                        RaycastHit2D leftAngle = SensorCast(new Vector2(-WidthRadius, 0f), Vector2.down, HeightRadius + 24f);
                                        RaycastHit2D rightAngle = SensorCast(new Vector2(WidthRadius, 0f), Vector2.down, HeightRadius + 24f);

                                        if (leftAngle && rightAngle)
                                        {
                                            float angleX = (rightAngle.point - leftAngle.point).x;
                                            float angleY = (rightAngle.point - leftAngle.point).y;
                                            GroundAngle = Utils.AngleCalculator(angleX, angleY);
                                        }
                                        else
                                        {
                                            if (rightAngle)
                                            {
                                                float angleX = rightAngle.normal.x;
                                                float angleY = rightAngle.normal.y;
                                                GroundAngle = Utils.AngleCalculator(angleX, angleY, true);
                                            }
                                            else if (leftAngle)
                                            {
                                                float angleX = leftAngle.normal.x;
                                                float angleY = leftAngle.normal.y;
                                                GroundAngle = Utils.AngleCalculator(angleX, angleY, true);
                                            }
                                        }

                                        if (GroundAngle >= 170f && GroundAngle <= 190f)
                                        {
                                            CeilingLand = -1;
                                            GroundAngle = 0f;
                                        }
                                        else
                                        {
                                            CeilingLand = 3;
                                        }

                                        if (CeilingLand == 3)
                                        {
                                            CeilingLand = 0;
                                            XSpeed += YSpeed * (Mathf.Sin(GroundAngle * Mathf.Deg2Rad) / 0.5f);
                                            GroundSpeed = XSpeed;
                                            Ground = Landed = true;
                                        }
                                    }
                                }
                            }

                            if (!Ground && CeilingLand > 0 && CeilingLand <= 3)
                            {
                                CeilingLand = 0;
                                GroundAngle = 0f;
                            }

                            YSpeed = 0f;
                        }


                        ColliderCeiling = SensorCast(new Vector2(-WidthRadius, 0f), Vector2.up, HeightRadius, true, Mathf.Abs(Ground ? 0f : YSpeed)).collider ??
                                          SensorCast(new Vector2(WidthRadius, 0f), Vector2.up, HeightRadius, true, Mathf.Abs(Ground ? 0f : YSpeed)).collider;
                    }
                }

                Quadrant = (int)(GroundAngle + (360f + 45f)) % 360 / 90;
            }
        }
    }

    public RaycastHit2D SensorCast(Vector2 offset, Vector2 direction, float distance, bool allowPlatforms = false, float extension = 0f)
    {
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

        Debug.DrawLine(start, anchor);

        if (num > -1)
        {
            return results[num];
        }
        else
        {
            return default(RaycastHit2D);
        }
    }

    /*public Collider2D OverlapBox(Vector2 offset, Vector2 size, bool allowPlatforms = false)
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
    }*/
}
