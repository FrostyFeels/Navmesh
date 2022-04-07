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

    public CoverAi coverAi;

    public Transform player;




    public GameObject[] _RouteNodes;
    public int[] _AvailableNodes;


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
    public float attackRange;

    public GameObject coverToSeek;
    private LineRenderer line;
    public override void Start()
    {
        agent = GetComponentInChildren<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        _DistanceToFirstNode = 0;
        line = GetComponent<LineRenderer>();

    }

    // Update is called once per frame
    public override void Update()
    {

        if(state == State.PathFinding)
        {
            if(cover.inRange.Count <=0)
            {
                state = State.CoverSeeking;
            }
        }

        if (cover.inRange.Count <= 0 && state == State.PathFinding)
        {
            state = State.CoverSeeking;
        }

        if(Vector3.Distance(transform.position, ai.player.position) < range && state == State.PathFinding)
        {
            state = State.CoverSeeking;
            
        }
/*
        if(state == State.PathFinding && agent.isStopped)
        {
            agent.isStopped = false;
            agent.SetDestination(_RouteNodes[stepCount].transform.position);
        }*/

        if (agent.remainingDistance < 5 && steps > stepCount && state == State.PathFinding)
        {
            agent.isStopped = false;
            agent.SetDestination(_RouteNodes[stepCount].transform.position); //Just add a limit
            stepCount++;
        }

        if(state == State.attack)
        {
            if (agent.remainingDistance < 10 && steps > stepCount)
            {
                agent.SetDestination(_RouteNodes[stepCount].transform.position); //Just add a limit
                stepCount++;
            }

            if(steps == stepCount)
            {
                state = State.Shooting;
            }


            Vector3 dir2 = player.position - transform.position;
            dir2.Normalize();
            RaycastHit2D longHit = Physics2D.Raycast(transform.position, dir2, range);

            if (longHit)
            {
                if (longHit.collider.gameObject.CompareTag("Player"))
                {
                    agent.isStopped = false;
                    state = State.Shooting;
                }
            }


        }

   

        if(state == State.Shooting)
        {
            
            agent.SetDestination(player.position);
        }

        Vector3 dir = player.position - transform.position;
        dir.Normalize();
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, attackRange);
        
        if(hit)
        {
            if(hit.collider.gameObject.CompareTag("Player"))
            {
                agent.isStopped = true;
                line.enabled = true;
                line.SetPosition(0, transform.position);
                line.SetPosition(1, hit.point);
                state = State.Shooting;
            }
            else
            {
                line.enabled = false;
                agent.isStopped = false;
            }
        }







    }

    public void attack()
    {
        state = State.attack;
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
}
