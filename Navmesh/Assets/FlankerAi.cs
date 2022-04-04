using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class FlankerAi : EnemyAI
{
    [SerializeField] private LayerMask layer, outerLayer;

    public FlankerAiManager ai;
    public CoverManager cover;

    public Transform player;


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

    public int nodeToTravel;

    public float range;

    public GameObject coverToSeek;

    public override void Start()
    {
        agent = GetComponentInChildren<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        _DistanceToFirstNode = 0;

    }

    // Update is called once per frame
    public override void Update()
    {

        if (cover.inRange.Count <= 0 && state == State.PathFinding)
        {
            findCover();
            state = State.CoverSeeking;
        }

        if(Vector3.Distance(transform.position, ai.player.position) < range && state == State.PathFinding)
        {
            findClosestCover();
            state = State.CoverSeeking;
            
        }

        if(state == State.PathFinding && agent.isStopped)
        {
            agent.isStopped = false;
            agent.SetDestination(_RouteNodes[stepCount - 1].transform.position);
        }


        if (agent.remainingDistance < 5 && steps > stepCount && state == State.PathFinding)
        {
            agent.SetDestination(_RouteNodes[stepCount].transform.position); //Just add a limit
            stepCount++;
        }

        if(state == State.CoverSeeking)
        {
            agent.SetDestination(coverToSeek.transform.position);
            
            if (agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance < 2)
            {
                state = State.foundCover;
            }
        }

        if(state == State.attack)
        {
            agent.SetDestination(player.position);
        }

        cover.InPosition();
    }

    public void attack()
    {
        state = State.attack;
    }

    public void findCover()
    {
        float distance = 0;
        GameObject node = null;
        foreach (GameObject _cover in cover.hideable)
        {
            if (!cover.used.Contains(_cover))
            {
                if (distance == 0)
                {
                    distance = Vector3.Distance(_cover.transform.position, player.position);
                    node = _cover;
                }

                float newDistance = Vector3.Distance(_cover.transform.position, player.position);

                if (distance > newDistance)
                {
                    distance = newDistance;
                    node = _cover;
                }
            }
        }
        cover.used.Add(node);
        coverToSeek = node;
        agent.SetDestination(node.transform.position);
    }

    public void findClosestCover()
    {
        float distance = 0;
        GameObject node = null;
        foreach (GameObject _cover in cover.hideable)
        {
            if(!cover.used.Contains(_cover))
            {
                if (distance == 0)
                {
                    distance = Vector3.Distance(_cover.transform.position, player.position);
                    node = _cover;
                }

                float newDistance = Vector3.Distance(_cover.transform.position, player.position);

                if (distance > newDistance)
                {
                    distance = newDistance;
                    node = _cover;
                }
            }
        }
        cover.used.Add(node);
        coverToSeek = node;
        agent.SetDestination(node.transform.position);
    }

    //Code to find the closest node you want the A.I to move too
    public void FindClosetsNode(int number)
    {
        lastNode = currentNode;
        float distance;
        for (int i = 0; i < ai.nodemanager._FlankNodes.Length; i++)
        {
            
            distance = Vector3.Distance(ai.nodemanager._FlankNodes[i].transform.position, transform.position);

            if(_DistanceToFirstNode == 0)
            {
                _DistanceToFirstNode = distance;
                closestNode = ai.nodemanager._FlankNodes[i];
                closestNodeNumber = i;
                currentNode = i;
            }

            if (_DistanceToFirstNode > distance)
            {
                _DistanceToFirstNode = distance;
                closestNode = ai.nodemanager._FlankNodes[i];
                closestNodeNumber = i;
                currentNode = i;

            }            
        }
        _DistanceToFirstNode = 0;

        if(number == 0)
        {
            FirstPathCreation(number);
        }
        else
        {
            CreatePath(number);
        }

      
    }
    public void CreatePath(int number)
    {

        _RouteNodes = new GameObject[ai.nodemanager._FlankNodes.Length];
        _RouteNodes[0] = closestNode;

        float priority = 0;
        int travelPoint = 0;


        float[] priorityNodes;
        ai.PriorityNodes.TryGetValue(ai._FirstFlank, out priorityNodes);


        for (int i = 0; i < priorityNodes.Length; i++)
        {
           
            if (priority == 0)
            {
                priority = priorityNodes[i];
                travelPoint = ai.availableNodes[i];
            }

            if (priorityNodes[i] > priority)
            {
                priority = priorityNodes[i];
                travelPoint = ai.availableNodes[i];
            }
        }


        nodeToTravel = travelPoint;
        


        CalculateShortestPath(number);
    }
    public void FirstPathCreation(int number)
    {
        _RouteNodes = new GameObject[ai.nodemanager._FlankNodes.Length];
        _RouteNodes[0] = closestNode;

        int random = UnityEngine.Random.Range(0, ai._AvailableNodes.Length);

        nodeToTravel = ai.availableNodes[random];
        ai._FirstFlank = nodeToTravel;
        CalculateShortestPath(number);
    }
    public void CalculateShortestPath(int number)
    {

        int currentNode = closestNodeNumber;
        bottomCount = 0;
        topCount = 0;



        if (currentNode != nodeToTravel)
        {
            if(currentNode < nodeToTravel)
            {
                topCount = nodeToTravel - currentNode;
            }
           else if(currentNode > nodeToTravel)
           {
                topCount += (ai.nodemanager._FlankNodes.Length - currentNode) + nodeToTravel;
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
                bottomCount += currentNode + (ai.nodemanager._FlankNodes.Length - nodeToTravel);
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
            _RouteNodes[x + 1] = ai.nodemanager._FlankNodes[i];
            steps++;
            if (i <= 0)
            {
                i = ai.nodemanager._FlankNodes.Length;
            }
        }
        stepCount = 0;
        agent.SetDestination(_RouteNodes[0].transform.position);
        stepCount++;
    }
    public void CalculateTopPath(int numberOfSteps)
    {
        steps = 1;
        accesableBotPaths = 0;

        for (int i = closestNodeNumber + 1, x = 0, y = 0; y < numberOfSteps; i++, x++, y++)
        {
            if (i >= ai.nodemanager._FlankNodes.Length)
            {
                i = 0;
                _RouteNodes[x + 1] = ai.nodemanager._FlankNodes[i];
                steps++;
            }
            else
            {
                _RouteNodes[x + 1] = ai.nodemanager._FlankNodes[i];
                steps++;
            }
        }
        stepCount = 0;
        agent.SetDestination(_RouteNodes[0].transform.position);
        stepCount++;

    }

 /*   private void RemoveElement<T>(ref T[] arr, int index)
    {

        for (int i = index; i < arr.Length - 1; i++)
        {
            arr[i] = arr[i + 1];         
        }

        Array.Resize(ref arr, arr.Length - 1);
    }*/

}
