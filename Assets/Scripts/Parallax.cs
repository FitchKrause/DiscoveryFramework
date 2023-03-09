using UnityEngine;

public class Parallax : MonoBehaviour
{
    [HideInInspector] public float InitialX;
    [HideInInspector] public float InitialY;

    [Range(0f, 1f)]
    public float XFactor, YFactor;

    private void Start()
    {
        InitialX = transform.position.x;
        InitialY = transform.position.y;
    }

    private void LateUpdate()
    {
        float PosX = ((PixelCamera.XLeftFrame + PixelCamera.XRightFrame) / 2f) - ((PixelCamera.XRightFrame - PixelCamera.XLeftFrame) / 2f);
        float PosY = ((PixelCamera.YBottomFrame + PixelCamera.YTopFrame) / 2f) + ((PixelCamera.YTopFrame - PixelCamera.YBottomFrame) / 2f);

        float XPosition = InitialX + Mathf.Floor((PosX * (XFactor)));
        float YPosition = InitialY + Mathf.Floor((PosY * (YFactor)));

        transform.position = new Vector3(XPosition, YPosition, transform.position.z);
    }
}
