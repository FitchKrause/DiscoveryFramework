using UnityEngine;
using System.Collections;

public static class Utils
{
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
