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


        ray = Physics2D.LinecastAll(transform.position, mousePosition);
        

        for (int i = 0; i < ray.Length; i++)
        {
            Debug.Log(ray[i].collider);


            Vector3Int pos = tiles.WorldToCell(ray[i].point);
            tiles.SetTile(pos, tile);


        }
  

        Debug.DrawLine(transform.position, mousePosition, Color.yellow);
    }
}
