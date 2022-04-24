using UnityEngine;
using UnityEngine.AI;

public class SimpleAiMovement : EnemyAI
{
    [SerializeField] private LayerMask layer;
    public int range = 10;
    NavMeshAgent agent;
    public NavMeshSurface2d surface;
    public CoverManager coverManager;
    public GameObject cover;
    public GameObject node;
    public FlankerAiManager manager;
    LineRenderer line;
    public GameObject grenade;


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponentInChildren<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        manager = GetComponentInParent<FlankerAiManager>();

        line = GetComponent<LineRenderer>();
    }

     public override void Update()
     {
        Vector3 dir = coverManager.player.position - transform.position;
        dir.Normalize();
        RaycastHit2D longHit = Physics2D.Raycast(transform.position, dir, range, layer);

        //checks if the player can be shot
        if (longHit)
        {
           
            if (longHit.collider.gameObject.CompareTag("Player"))
            {
                state = State.Shooting;
                line.SetPosition(0, transform.position);
                line.SetPosition(1, longHit.point);
            }
            else
            {
               
            }
        }
        else
        {
            state = State.attack;
        }


        if(state == State.Shooting)
        {
            agent.isStopped = true;
            return;
        }

        if(state == State.attack)
        {
            agent.isStopped = false;
        }

        if (manager.hits >= (float)10 / (manager.flankers.Length + manager.normals.Length))
        {
            agent.SetDestination(manager.player.position);
            return;
        }

        if(cover != null)
        {
            agent.isStopped = false;
            agent.SetDestination(cover.transform.position);
        }

       
        if(agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance < 2)
        {
            if (manager.throwNade && grenade != null)
            {
                ThrowGrenade();
            }
        }
    }


    //Cover seeking for normal A.I
    public void GetCoverToHide()
    {
        float distance = 0;
        GameObject node = null;


        float distance2 = 0;

        if (coverManager.inRange.Count <= 0)
        {
            foreach (GameObject _cover in coverManager.hideable)
            {

                if (!coverManager.used.Contains(_cover))
                {
                    if (distance == 0)
                    {
                        distance = Vector3.Distance(_cover.transform.position, transform.position);
                        node = _cover;
                    }

                    float newDistance = Vector3.Distance(_cover.transform.position, transform.position);

                    if (distance > newDistance)
                    {
                        distance = newDistance;
                        node = _cover;
                    }
                }
            }
        }
        else
        {
            foreach (GameObject _cover in coverManager.inRange)
            {
                if (!coverManager.used.Contains(_cover))
                {
                    if (distance == 0)
                    {
                        distance = Vector3.Distance(_cover.transform.position, transform.position);
                        node = _cover;
                    }

                    float newDistance = Vector3.Distance(_cover.transform.position, transform.position);

                    if (distance > newDistance)
                    {
                        distance = newDistance;
                        node = _cover;
                    }
                }
            }
        }

        if (!coverManager.used.Contains(node))
        {
            coverManager.used.Add(node);
        }

        cover = node;

        if (cover == null)
        {

        }
        else
        {
            agent.SetDestination(cover.transform.position);
            state = State.CoverSeeking;
        }

        
    }

    //Nade throw
    public void ThrowGrenade()
    {
        GameObject _grenade = Instantiate(grenade, transform.parent);
        _grenade.transform.position = manager.player.position;

        manager.throwNade = false;
    }

    /*    public IEnumerator waitToCalculatePath()
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

        }*/

    /*    public void calculatePathPositions(Vector3 start, Vector3 end)
        {
            //gets the start and end position on the tilemap
            Vector3Int startPos = tiles.WorldToCell(start);
            Vector3Int endpos = tiles.WorldToCell(end);

            //meassure how many steps it has to take to reach the next corner of the pathfinding
            float step = Vector3Int.Distance(startPos, endpos);
            steps = Mathf.Abs(startPos.x - endpos.x) + Mathf.Abs(startPos.y - endpos.y) -1;

            Vector3 dir = endpos - startPos;
            dir.Normalize();   

            //Makes the tilemap tile edible
            tiles.SetTileFlags(tiles.WorldToCell(newpos), TileFlags.None);
            tiles.SetColor(tiles.WorldToCell(newpos), Color.black);

            //Goes through a loop that sets the tile on the tilemap and add a tile above and under or right and left to make a thicker path
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

                if(dir.y > dir.x && dir.y > 0)
                {
                    tiles.SetTile(pos + new Vector3Int(0, 1, 0), tile);
                    tiles.SetTile(pos + new Vector3Int(0, -1, 0), tile);
                }
            }

        }*/
}
