using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SimpleMovement : MonoBehaviour
{
  
    public float xSpeed, ySpeed;
    public float speed;

    // Update is called once per frame
    void Update()
    {
        //small code for moving
        if (Input.GetKey(KeyCode.W)) ySpeed = speed;
        else if (Input.GetKey(KeyCode.S)) ySpeed = -speed;
        else ySpeed = 0;

        if (Input.GetKey(KeyCode.D)) xSpeed = speed;
        else if (Input.GetKey(KeyCode.A)) xSpeed = -speed;
        else xSpeed = 0;
    }

    public void FixedUpdate()
    {
        transform.Translate(new Vector2(xSpeed, ySpeed) * Time.fixedDeltaTime);
    }
}
