using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSorting : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        //Set the order in layer for the SpriteRenderer.
        int orderInLayer = (int)transform.position.y;
        spriteRenderer.sortingOrder = -orderInLayer;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
