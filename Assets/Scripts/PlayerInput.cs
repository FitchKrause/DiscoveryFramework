using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour
{
    public static bool KeyUp;
    public static bool KeyDown;
    public static bool KeyLeft;
    public static bool KeyRight;
    public static bool KeyActionA;
    public static bool KeyActionB;
    public static bool KeyActionC;

    public static bool KeyUpPressed;
    public static bool KeyDownPressed;
    public static bool KeyLeftPressed;
    public static bool KeyRightPressed;
    public static bool KeyActionAPressed;
    public static bool KeyActionBPressed;
    public static bool KeyActionCPressed;

    public static bool IgnoreInput;
    public static bool OverrideInput;
    public static bool ClearInput;

    private void Awake()
    {
        IgnoreInput = OverrideInput = ClearInput = false;
    }

    private void FixedUpdate()
    {
        if (ClearInput)
        {
            KeyUpPressed = KeyUp = false;
            KeyDownPressed = KeyDown = false;
            KeyLeftPressed = KeyLeft = false;
            KeyRightPressed = KeyRight = false;
            KeyActionAPressed = KeyActionA = false;
            KeyActionBPressed = KeyActionB = false;
            KeyActionCPressed = KeyActionC = false;

            ClearInput = false;
        }

        if (!IgnoreInput && !OverrideInput)
        {
            KeyUpPressed = Input.GetKey(KeyCode.UpArrow) && !KeyUp;
            KeyUp = Input.GetKey(KeyCode.UpArrow);

            KeyDownPressed = Input.GetKey(KeyCode.DownArrow) && !KeyDown;
            KeyDown = Input.GetKey(KeyCode.DownArrow);

            KeyLeftPressed = Input.GetKey(KeyCode.LeftArrow) && !KeyLeft;
            KeyLeft = Input.GetKey(KeyCode.LeftArrow);

            KeyRightPressed = Input.GetKey(KeyCode.RightArrow) && !KeyRight;
            KeyRight = Input.GetKey(KeyCode.RightArrow);

            KeyActionAPressed = Input.GetKey(KeyCode.Z) && !KeyActionA;
            KeyActionA = Input.GetKey(KeyCode.Z);

            KeyActionBPressed = Input.GetKey(KeyCode.X) && !KeyActionB;
            KeyActionB = Input.GetKey(KeyCode.X);

            KeyActionCPressed = Input.GetKey(KeyCode.C) && !KeyActionC;
            KeyActionC = Input.GetKey(KeyCode.C);
        }
    }
}
