using UnityEngine;
using System.Collections;

public class Digit : MonoBehaviour
{
    public Sprite[] Digits = new Sprite[1];
    public int Value;

    private SpriteRenderer render;

    private void Start()
    {
        render = GetComponent<SpriteRenderer>();
    }

    public void SetValue(int value)
    {
        Value = value;
        if (Value < 0)
        {
            Value = 0;
        }
        if (Value >= Digits.Length)
        {
            Value = Digits.Length - 1;
        }
        render.sprite = Digits[Value];
    }
}
