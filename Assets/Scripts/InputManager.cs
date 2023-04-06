using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour
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

    public static bool IgnoreInput;

    private static InputManager instance;

    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void FixedUpdate()
    {
        KeyUpPressed = !IgnoreInput && Input.GetKey(KeyCode.UpArrow) && !KeyUp;
        KeyUp = !IgnoreInput && Input.GetKey(KeyCode.UpArrow);

        KeyDownPressed = !IgnoreInput && Input.GetKey(KeyCode.DownArrow) && !KeyDown;
        KeyDown = !IgnoreInput && Input.GetKey(KeyCode.DownArrow);

        KeyLeftPressed = !IgnoreInput && Input.GetKey(KeyCode.LeftArrow) && !KeyLeft;
        KeyLeft = !IgnoreInput && Input.GetKey(KeyCode.LeftArrow);

        KeyRightPressed = !IgnoreInput && Input.GetKey(KeyCode.RightArrow) && !KeyRight;
        KeyRight = !IgnoreInput && Input.GetKey(KeyCode.RightArrow);

        KeyActionAPressed = !IgnoreInput && Input.GetKey(KeyCode.Z) && !KeyActionA;
        KeyActionA = !IgnoreInput && Input.GetKey(KeyCode.Z);

        KeyActionBPressed = !IgnoreInput && Input.GetKey(KeyCode.X) && !KeyActionB;
        KeyActionB = !IgnoreInput && Input.GetKey(KeyCode.X);

        KeyActionCPressed = !IgnoreInput && Input.GetKey(KeyCode.C) && !KeyActionC;
        KeyActionC = !IgnoreInput && Input.GetKey(KeyCode.C);

        KeyStartPressed = !IgnoreInput && Input.GetKey(KeyCode.Return) && !KeyStart;
        KeyStart = !IgnoreInput && Input.GetKey(KeyCode.Return);
    }
}
