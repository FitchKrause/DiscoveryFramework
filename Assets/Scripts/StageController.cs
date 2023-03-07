using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController : MonoBehaviour
{
    public static StageController CurrentStage;
    public static float LevelTimer;
    public static float GlobalTimer;

    public static bool STOP;
    public float GameTimer;
    public int Milliseconds;
    public int Seconds;
    public int Minutes;

    public int Width;
    public int Height;

    public int Rings;

    public List<BaseObject> ObjectList;
    public List<ObjectPool> ObjectPools;
    public int ObjectCount;

    private static float CreationStrength;
    private static float InitialStrength = 4f;
    private static float StepStrength = 2f;

    private void Awake()
    {
        STOP = false;

        ObjectList = new List<BaseObject>();
        ObjectPools = new List<ObjectPool>();
        ObjectCount = 0;

        CurrentStage = this;

        LevelTimer = 0f;
        GlobalTimer = 0f;
    }

    private void Start()
    {
        foreach (BaseObject objRef in FindObjectsOfType<BaseObject>())
        {
            if (objRef.ObjectName == string.Empty)
            {
                objRef.ObjectName = objRef.GetType().Name;
            }
            CurrentStage.ObjectList.Add(objRef);
            CurrentStage.ObjectCount++;
        }
    }

    private void FixedUpdate()
    {
        Rings = Mathf.Clamp(Rings, 0, 999);

        if (!STOP)
        {
            LevelTimer += 1f;
            GlobalTimer += 1f;

            GameTimer += 1000f / 60f;

            Milliseconds = Mathf.FloorToInt(GameTimer / 10) % 100;
            Seconds = Mathf.FloorToInt(GameTimer / 1000) % 60;
            Minutes = Mathf.FloorToInt(GameTimer / 60000);
        }
    }

    public static void RingLoss(int ringsToCreate, float creationX, float creationY)
    {
        ringsToCreate = Mathf.Min(ringsToCreate, 32);
        CreationStrength = InitialStrength;
        for (int i = 0; i < 1 + Mathf.CeilToInt(ringsToCreate / 16); i++)
        {
            int ringsInThisRow = Mathf.Min(16, ringsToCreate);
            ringsToCreate = Mathf.Max(ringsToCreate - 16, 0);
            int CreationDirection = 0;
            float CreationAngle = 11.25f;
            for (int j = 0; j < ringsInThisRow; j++)
            {
                if (CreationDirection == 0)
                {
                    Ring movingRing = CreateStageObject("Moving Ring", creationX, creationY) as Ring;
                    movingRing.MovementActivated = true;
                    movingRing.XPosition = creationX;
                    movingRing.YPosition = creationY;
                    movingRing.XSpeed = -Mathf.Sin(CreationAngle * Mathf.Deg2Rad) * CreationStrength;
                    movingRing.YSpeed = Mathf.Cos(CreationAngle * Mathf.Deg2Rad) * CreationStrength;
                    movingRing.transform.position = new Vector3(movingRing.XPosition, movingRing.YPosition, 0f);
                }
                else if (CreationDirection == 1)
                {
                    Ring movingRing = CreateStageObject("Moving Ring", creationX, creationY) as Ring;
                    movingRing.MovementActivated = true;
                    movingRing.XPosition = creationX;
                    movingRing.YPosition = creationY;
                    movingRing.XSpeed = Mathf.Sin(CreationAngle * Mathf.Deg2Rad) * CreationStrength;
                    movingRing.YSpeed = Mathf.Cos(CreationAngle * Mathf.Deg2Rad) * CreationStrength;
                    movingRing.transform.position = new Vector3(movingRing.XPosition, movingRing.YPosition, 0f);
                    CreationAngle += 22.5f;
                }
                CreationDirection = 1 - CreationDirection;
            }
            CreationStrength -= StepStrength;
        }
    }

    public static BaseObject CreateStageObject(string objName, float PosX, float PosY)
    {
        foreach (ObjectPool pool in CurrentStage.ObjectPools)
        {
            if (pool.PoolName == objName)
            {
                foreach (BaseObject objRef in pool.PooledObjects)
                {
                    if (!objRef.gameObject.activeSelf)
                    {
                        objRef.XPosition = PosX;
                        objRef.YPosition = PosY;
                        objRef.transform.position = new Vector3(PosX, PosY, 0f);
                        objRef.gameObject.SetActive(true);
                        objRef.ObjectCreated();
                        CurrentStage.ObjectCount++;
                        CurrentStage.ObjectList.Add(objRef);
                        return objRef;
                    }
                }
            }
        }

        return null;
    }

    public static void DestroyStageObject(BaseObject objRef)
    {
        if (objRef.gameObject.activeSelf != false)
        {
            objRef.gameObject.SetActive(false);
            CurrentStage.ObjectCount--;
            CurrentStage.ObjectList.Remove(objRef);
        }
    }

    public static bool AABB(HitBox rectA, HitBox rectB)
    {
        float combinedXRadius = rectB.WidthRadius + rectA.WidthRadius;
        float combinedYRadius = rectB.HeightRadius + rectA.HeightRadius;

        float combinedXDiameter = combinedXRadius * 2f;
        float combinedYDiameter = combinedYRadius * 2f;

        float left_difference = (rectB.XPosition - rectA.XPosition) + combinedXRadius;
        float top_difference = (rectB.YPosition - rectA.YPosition) + combinedYRadius;

        if (left_difference < 0f ||
            left_difference > combinedXDiameter ||
            top_difference < 0f ||
            top_difference > combinedYDiameter)
        {
            return false;
        }

        return true;
    }

    public static bool CircleCast(BaseObject objRef, Vector2 position, float radius)
    {
        bool flag = false;

        foreach (Collider2D col in Physics2D.OverlapCircleAll(position, radius, 1 << objRef.CollisionLayer))
        {
            if (col == objRef.ColliderBody) continue;

            if (col.tag == "Solid" ||
                col.tag == "Platform" && objRef.YSpeed <= 0f && (objRef.YPosition - objRef.HeightRadius) >= (col.transform.position.y - 4f))
            {
                flag = true;
                break;
            }
        }

        return flag;
    }

    public static bool PointCast(BaseObject objRef, Vector2 position)
    {
        bool flag = false;

        foreach (Collider2D col in Physics2D.OverlapPointAll(position, 1 << objRef.CollisionLayer))
        {
            if (col == objRef.ColliderBody) continue;

            if (col.tag == "Solid" ||
                col.tag == "Platform" && objRef.YSpeed <= 0f && (objRef.YPosition - objRef.HeightRadius) >= (col.transform.position.y - 4f))
            {
                flag = true;
                break;
            }
        }

        return flag;
    }
}
