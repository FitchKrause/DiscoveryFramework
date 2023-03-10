using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour
{
    public bool KeyUp;
    public bool KeyDown;
    public bool KeyLeft;
    public bool KeyRight;
    public bool KeyActionA;
    public bool KeyActionB;
    public bool KeyActionC;
    public bool KeyStart;

    public bool KeyUpPressed;
    public bool KeyDownPressed;
    public bool KeyLeftPressed;
    public bool KeyRightPressed;
    public bool KeyActionAPressed;
    public bool KeyActionBPressed;
    public bool KeyActionCPressed;
    public bool KeyStartPressed;

    public static InputManager instance;

    private void Awake()
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

    private void FixedUpdate()
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

        KeyStartPressed = Input.GetKey(KeyCode.Return) && !KeyStart;
        KeyStart = Input.GetKey(KeyCode.Return);
    }
}
