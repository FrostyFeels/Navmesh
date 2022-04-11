using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeCharger : MonoBehaviour
{
    Color color;
    SpriteRenderer renderer;
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        color = renderer.color;
    }

    // Update is called once per frame
    void Update()
    {
        color.a += (Time.deltaTime / 4);
        renderer.color = color;


        if(renderer.color.a >= 1)
        {
            Destroy(gameObject);
        }
    }
}
