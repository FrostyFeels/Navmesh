using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class VisionCone : MonoBehaviour
{

    public Vector3 mousePosition;
    RaycastHit2D[] ray;

    public Tilemap tiles;
    public Tile tile;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);


        Vector2 dir = transform.position - mousePosition;
        dir.Normalize();

        ray = Physics2D.RaycastAll(transform.position, dir, 10);
        

        for (int i = 0; i < ray.Length; i++)
        {
         
            Vector3Int pos = tiles.WorldToCell(ray[i].point);
            Debug.Log(pos);
            tiles.SetTile(pos, tile);
        }
  

        Debug.DrawLine(transform.position, mousePosition, Color.yellow);
    }
}
