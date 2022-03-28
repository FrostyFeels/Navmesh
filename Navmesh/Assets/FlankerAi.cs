using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class FlankerAi : MonoBehaviour
{
    [SerializeField] private LayerMask layer, outerLayer;

    public FlankerAiManager ai;

    public Transform player;
    public Vector3 homePoint;


    [Header("List of nodes")]
    //public GameObject[] _FlankNodes;
    //public GameObject[] _AvailableNodes;

    public GameObject[] _RouteNodes;
    public int[] _AvailableNodes;

    public int[] _BottemNodes;
    public int[] _TopNodes;



    [Header("Prefabs")]


    public GameObject closestNode;
    public int closestNodeNumber;
    public float _DistanceToFirstNode;
    public int currentNode;
    public int lastNode;

    


    public bool topPath, bottomPath;
    public int topCount, bottomCount;
    public int stepCount;
    public int steps;

    public int nodesToTop, nodesToBot;

    public int accesableBotPaths, accesableTopPaths;

    [SerializeField] Transform target;
    public NavMeshAgent agent;
    public NavMeshSurface2d surface;

    void Start()
    {
        agent = GetComponentInChildren<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;


        _DistanceToFirstNode = 0;


        homePoint = player.transform.position;

       

    }

    // Update is called once per frame
    void Update()
    {
        if (agent.remainingDistance < 2 && steps > stepCount)
        {
            agent.SetDestination(_RouteNodes[stepCount].transform.position); //Just add a limit
            stepCount++;
        }



    }

    //Draw 8 squares each coming from the home point
    /*public void DrawFlankNodes()
    {
        for (int i = 0; i < _FlankNodes.Length; i++)
        {
            _FlankNodes[i].name = NodeSpawnCount.ToString();
            NodeSpawnCount++;
            Vector3 endPoint = homePoint + (_FlankDirections[i] * flankNodeDistance);
            RaycastHit2D hit = Physics2D.Linecast(homePoint, endPoint, outerLayer);

            if(hit)
            {
                _FlankNodes[i].transform.position = hit.point;
            }
            else
            {
                _FlankNodes[i].transform.position = endPoint;
            }        
        }
        NodeSpawnCount = 0;
    }*/


    //Code for checking if the node can see the player without walls
    //Color the nodes red if they have vision or blue if vision is blocked
    /*public void CheckAvailableNodes()
    {
        for (int i = 0; i < _FlankNodes.Length; i++)
        {
            RaycastHit2D hit = Physics2D.Linecast(homePoint, _FlankNodes[i].transform.position, layer);
            float distanceToNode = Vector3.Distance(player.transform.position, _FlankNodes[i].transform.position);

                if (!hit && distanceToNode >= minimalNeededDistance && _FlankNodes[i] != closetNode)
                {
                    _FlankNodes[i].GetComponent<SpriteRenderer>().color = Color.red;
                    _AvailableNodes[i] = _FlankNodes[i];

                }
                else
                {
                    if(_FlankNodes[i] != closetNode)
                    {
                        _FlankNodes[i].GetComponent<SpriteRenderer>().color = Color.cyan;
                    }
                    _AvailableNodes[i] = null;
                    CheckWallCollision(_FlankDirections[i], hit.point, i);
                }     
        }
    }*/

    //Code to find the closest node you want the A.I to move too
    public void FindClosetsNode(int number)
    {
        lastNode = currentNode;
        float distance;
        for (int i = 0; i < ai._FlankNodes.Length; i++)
        {
            
            distance = Vector3.Distance(ai._FlankNodes[i].transform.position, transform.position);

            if(_DistanceToFirstNode == 0)
            {
                _DistanceToFirstNode = distance;
                closestNode = ai._FlankNodes[i];
                closestNodeNumber = i;
                currentNode = i;
            }

            if (_DistanceToFirstNode > distance)
            {
                _DistanceToFirstNode = distance;
                closestNode = ai._FlankNodes[i];
                closestNodeNumber = i;
                currentNode = i;

            }            
        }
        _DistanceToFirstNode = 0;

        CreatePath(number);
    }

    //Check if the player hasn't moved to far away from the homepoint and reset the homepoints position to the player
    /*    public void CheckHomePointAvailability()
    {
        distanceFromHomepoint = Vector3.Distance(homePoint, player.transform.position);

        if(distanceFromHomepoint > maxDistance)
        {
            homePoint = player.transform.position;
            DrawFlankNodes();
            CheckAvailableNodes();  
        }
    }*/

    //Check if a note is not stuck inside a wall
    /*public void CheckWallCollision(Vector3 direction, Vector3 point, int number)
    {
        Vector3 startLocation = point + direction;

            for (int i = 0; i < 100; i++)
            {
                RaycastHit2D hit = Physics2D.Linecast(startLocation, homePoint, layer);
                
                if (hit)
                {
                    float distance = Vector3.Distance(hit.point, homePoint);

                    if (distanceToPlayer == 0)
                    {
                        distanceToPlayer = distance;
                    }
                    else if (distanceToPlayer == distance)
                    {
                        _FlankNodes[number].transform.position = hit.point + (Vector2)direction;
                        return;
                    }
                    else
                    {
                        distanceToPlayer = distance;
                    }
                    startLocation = hit.point + (Vector2)direction;
                }       
        }
    }*/

    public void CreatePath(int number)
    {

        _RouteNodes = new GameObject[ai._FlankNodes.Length];
        _RouteNodes[0] = closestNode;

        for (int i = 0, x = 0; i < ai._AvailableNodes.Length; i++)
        {
            if(ai._AvailableNodes[i] != null)
            {
                ai.availableNodes[x] = i;
                x++;
            }
        }

        CalculateShortestPath(number);
    }
    public void CalculateShortestPath(int number)
    {
        int random = UnityEngine.Random.Range(0, ai.availableNodes.Length);
        int nodeToTravel = ai.availableNodes[random];
        int currentNode = closestNodeNumber;

        Debug.Log(nodeToTravel + " " + number);

        RemoveElement(ref ai.availableNodes, random);
    
        ai._AvailableNodes[nodeToTravel] = null;

        if(currentNode != nodeToTravel)
        {
            if(currentNode < nodeToTravel)
            {
                topCount = nodeToTravel - currentNode;
            }
           else if(currentNode > nodeToTravel)
           {
                topCount += (ai._FlankNodes.Length - currentNode) + nodeToTravel + 1;
           }         
        }

        if (currentNode != nodeToTravel)
        {
            if(currentNode > nodeToTravel)
            {
                bottomCount = currentNode - nodeToTravel;
            }
            else if (currentNode < nodeToTravel)
            {
                bottomCount += currentNode + (ai._FlankNodes.Length - nodeToTravel) + 1;
            }
            
        }




        if (bottomCount < topCount)
        {
            CalculateBotPath(bottomCount);

        }
        else
        {
            CalculateTopPath(topCount);
        }

        bottomCount = 0;
        topCount = 0;

        if(ai.flankers.Length > number + 1)
        {
            ai.flankers[number + 1].FindClosetsNode(number + 1);
        }
    }
    public void CalculateBotPath(int numberOfSteps)
    {
        steps = 1;
        accesableBotPaths = 0;

        for (int i = closestNodeNumber - 1, x = 0, y = 0; y < numberOfSteps; y++, i--, x++)
        {
            _RouteNodes[x + 1] = ai._FlankNodes[i];
            steps++;
            if (i <= 0)
            {
                i = ai._FlankNodes.Length;
            }
        }
        agent.SetDestination(_RouteNodes[0].transform.position);
        stepCount++;
    }
    public void CalculateTopPath(int numberOfSteps)
    {
        steps = 1;
        accesableBotPaths = 0;

        for (int i = closestNodeNumber + 1, x = 0, y = 0; y < numberOfSteps; i++, x++, y++)
        {
            if (i >= ai._FlankNodes.Length)
            {
                i = 0;
                _RouteNodes[x + 1] = ai._FlankNodes[i];
                steps++;
            }
            else
            {
                _RouteNodes[x + 1] = ai._FlankNodes[i];
                steps++;
            }
        }
        stepCount = 0;
        agent.SetDestination(_RouteNodes[0].transform.position);
        stepCount++;

    }

    private void RemoveElement<T>(ref T[] arr, int index)
    {

        for (int i = index; i < arr.Length - 1; i++)
        {
            arr[i] = arr[i + 1];         
        }

        Array.Resize(ref arr, arr.Length - 1);
    }

}
