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
    public static bool KeyStart;

    public static bool KeyUpPressed;
    public static bool KeyDownPressed;
    public static bool KeyLeftPressed;
    public static bool KeyRightPressed;
    public static bool KeyActionAPressed;
    public static bool KeyActionBPressed;
    public static bool KeyActionCPressed;
    public static bool KeyStartPressed;

    public static bool OverrideInput;
    public static bool ClearInput;

    private void Awake()
    {
        OverrideInput = ClearInput = false;
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
            KeyStartPressed = KeyStart = false;

            ClearInput = false;
        }

        if (!OverrideInput)
        {
            KeyUpPressed = InputManager.KeyUpPressed;
            KeyUp = InputManager.KeyUp;

            KeyDownPressed = InputManager.KeyDownPressed;
            KeyDown = InputManager.KeyDown;

            KeyLeftPressed = InputManager.KeyLeftPressed;
            KeyLeft = InputManager.KeyLeft;

            KeyRightPressed = InputManager.KeyRightPressed;
            KeyRight = InputManager.KeyRight;

            KeyActionAPressed = InputManager.KeyActionAPressed;
            KeyActionA = InputManager.KeyActionA;

            KeyActionBPressed = InputManager.KeyActionBPressed;
            KeyActionB = InputManager.KeyActionB;

            KeyActionCPressed = InputManager.KeyActionCPressed;
            KeyActionC = InputManager.KeyActionC;

            KeyStartPressed = InputManager.KeyStartPressed;
            KeyStart = InputManager.KeyStart;
        }
    }
}
