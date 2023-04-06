public struct HitBox
{
    public float XPosition;
    public float YPosition;
    public float WidthRadius;
    public float HeightRadius;

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
}
