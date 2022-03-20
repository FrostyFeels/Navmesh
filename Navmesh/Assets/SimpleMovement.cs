using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SimpleMovement : MonoBehaviour
{
    public Tilemap tiles;
   

    public float xSpeed, ySpeed;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        tiles.SetColor(new Vector3Int(3, 3, 0), Color.black);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W)) ySpeed = speed;
        else if (Input.GetKey(KeyCode.S)) ySpeed = -speed;
        else ySpeed = 0;

        if (Input.GetKey(KeyCode.D)) xSpeed = speed;
        else if (Input.GetKey(KeyCode.A)) xSpeed = -speed;
        else xSpeed = 0;


       
/*        Vector3Int ipos = tiles.WorldToCell(transform.position);
        tiles.SetTileFlags(ipos, TileFlags.None);
        tiles.SetColor(ipos, Color.black);*/
        
  
        
    
    }

    public void FixedUpdate()
    {
        transform.Translate(new Vector2(xSpeed, ySpeed) * Time.fixedDeltaTime);
    }
}
