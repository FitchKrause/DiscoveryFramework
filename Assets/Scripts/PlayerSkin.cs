using UnityEngine;
using System.Collections;

public class PlayerSkin : MonoBehaviour
{
    public int CharacterID;
    public bool SuperSkin;
    public GameObject Child;

    [HideInInspector] public SpriteRenderer render;
    [HideInInspector] public Animator animator;

    private void Start()
    {
        render = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Child != null)
        {
            Child.SetActive(render.enabled);
            Child.GetComponent<SpriteRenderer>().flipX = render.flipX;
            Child.GetComponent<SpriteRenderer>().sortingLayerName = render.sortingLayerName;
            Child.GetComponent<SpriteRenderer>().sortingOrder = render.sortingOrder - 1;
        }
    }
}
