using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class SimpleAiMovement : MonoBehaviour
{
    public Tilemap tiles;
    public Tile tile;


    [SerializeField] Transform target;
    NavMeshAgent agent;
    public NavMeshSurface2d surface;


    public Vector3 newpos;
    public int steps;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        

      
        StartCoroutine(waitToCalculatePath());
        newpos = agent.transform.position;
    }

    public void Update()
    {
        agent.SetDestination(target.position);


        for (int i = 0; i < agent.path.corners.Length - 1; i++)
        {
            Debug.DrawLine(agent.path.corners[i], agent.path.corners[i + 1], Color.red);
               
            Vector3Int ipos = tiles.WorldToCell(agent.path.corners[i]);
            tiles.SetTileFlags(ipos, TileFlags.None);
            tiles.SetColor(ipos, Color.black);
        }
        
    }

    public IEnumerator waitToCalculatePath()
    {
        yield return new WaitForSeconds(.25f);
        for (int i = 0; i < agent.path.corners.Length - 1; i++)
        {
            calculatePathPositions(agent.path.corners[i], agent.path.corners[i + 1]);
        }




        yield return new WaitForSeconds(1f);
        surface.BuildNavMesh();
        newpos = agent.transform.position;
        StartCoroutine(waitToCalculatePath());

    }

    public void calculatePathPositions(Vector3 start, Vector3 end)
    {
        Vector3Int startPos = tiles.WorldToCell(start);
        Vector3Int endpos = tiles.WorldToCell(end);


        float step = Vector3Int.Distance(startPos, endpos);
        steps = Mathf.Abs(startPos.x - endpos.x) + Mathf.Abs(startPos.y - endpos.y) -1;

        Vector3 dir = endpos - startPos;
        dir.Normalize();   
        
        tiles.SetTileFlags(tiles.WorldToCell(newpos), TileFlags.None);
        tiles.SetColor(tiles.WorldToCell(newpos), Color.black);

        for (int i = 0; i < step; i++)
        {
            newpos = newpos + dir;

             
            Vector3Int pos = tiles.WorldToCell(newpos);
            tiles.SetTile(pos, tile);
            if ((dir.x > dir.y && dir.x > 0))
            {
                tiles.SetTile(pos + new Vector3Int(1,0,0), tile);              
                tiles.SetTile(pos + new Vector3Int(-1, 0, 0), tile);
            }          
        }
       
    }
}
