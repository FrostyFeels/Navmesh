using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NodeManager : MonoBehaviour
{
    public GameObject[] _FlankNodes;
    public Vector3[] _FlankDirections;

    public GameObject prefab;

    public float flankNodeDistance;

    public float minimalNeededDistance;

    public int NodeSpawnCount;

    public float distanceToPlayer;


    [SerializeField] private LayerMask layer, outerLayer;

    public Transform player;
    public Vector3 homePoint;
    public float distanceFromHomepoint;
    public float maxDistance;
    public int availableNodesNumber;
    public int count;


    public FlankerAiManager[] ai;
    public GameObject[] _AvailableNodes;
    public int[] availableNodes;

    public int nodes;

    public int segmentLenght;
    public int segmentNode;

    public float interval;
    public float intervalCount;



    public void Start()
    {


        segmentLenght = nodes / 8;
        _FlankNodes = new GameObject[nodes];
        _FlankDirections = new Vector3[nodes];

        interval = 1 / (float)segmentLenght;
        
        for (int i = 0; i < _FlankNodes.Length; i++)
        {
            _FlankNodes[i] = Instantiate(prefab, gameObject.transform);
        }

        for (int i = (nodes - (segmentLenght)), x = 0, y = 0; x < nodes; x++)
        {
            
            if(i == nodes)
            {
                i = 0;
            }

            if (intervalCount > segmentLenght * 2)
            {
                Debug.Log(intervalCount);
                intervalCount = 1;
            }

            if ((i < nodes && i >= nodes - segmentLenght) || i <= segmentLenght) 
            {
                _FlankDirections[i] = new Vector3(-1 + (interval * intervalCount), 1, 0);
                intervalCount++;
            }

            if(i > segmentLenght && i <= segmentLenght * 3)
            {
                _FlankDirections[i] = new Vector3(1, 1 - (interval * intervalCount), 0);
                intervalCount++;
            }

            if (i > segmentLenght * 3 && i <= segmentLenght * 5)
            {

                _FlankDirections[i] = new Vector3(1 - (intervalCount * interval), -1, 0);
                intervalCount++;
            }

            if (i > segmentLenght * 5 && i < segmentLenght * 7)
            {
                Debug.Log(i);
                _FlankDirections[i] = new Vector3(-1, -1 + (interval * intervalCount), 0);
                intervalCount++;
            }

            i++;



        }
        DrawFlankNodes();
        CheckAvailableNodes();
    }
    public void Update()
    {
        CheckHomePointAvailability();
    }
    public void DrawFlankNodes()
    {
        for (int i = 0; i < _FlankNodes.Length; i++)
        {
            _FlankNodes[i].name = NodeSpawnCount.ToString();          
            Vector3 endPoint = homePoint + (_FlankDirections[i] * flankNodeDistance);
            _FlankNodes[i].transform.position = endPoint;
            NodeSpawnCount++;

        }
        NodeSpawnCount = 0;
    }
    public void CheckAvailableNodes()
    {

        _AvailableNodes = new GameObject[_FlankDirections.Length];
        availableNodes = new int[_FlankDirections.Length];

        for (int i = 0, x = 0; i < _FlankNodes.Length; i++)
        {
            Vector3 endPoint = homePoint + (_FlankDirections[i] * flankNodeDistance);
            RaycastHit2D hit = Physics2D.Linecast(homePoint, _FlankNodes[i].transform.position, layer);
            RaycastHit2D edgeHit = Physics2D.Linecast(homePoint, endPoint, outerLayer);

            if (hit)
            {
                _FlankNodes[i].GetComponent<SpriteRenderer>().color = Color.yellow;
                CheckWallCollision(_FlankDirections[i], hit.point, i);
            }


            if (edgeHit)
            {
                _FlankNodes[i].transform.position = edgeHit.point;
            }

            float disx = player.position.x - _FlankNodes[i].transform.position.x;
            float disy = player.position.y - _FlankNodes[i].transform.position.y;
            disy = Mathf.Abs(disy);
            disx = Mathf.Abs(disx);

            if (!hit && (disx > minimalNeededDistance || disy > minimalNeededDistance))
            {
                _FlankNodes[i].GetComponent<SpriteRenderer>().color = Color.green;
                _AvailableNodes[x] = _FlankNodes[i];
                availableNodes[x] = i;
                x++;
                count++;
            }
            else
            {
                _FlankNodes[i].GetComponent<SpriteRenderer>().color = Color.yellow;
                RemoveElement(ref _AvailableNodes, x);
                RemoveElement(ref availableNodes, x);
            }

        }
        availableNodesNumber = count;
        count = 0;


        SendNodes();
    }
    public void CheckWallCollision(Vector3 direction, Vector3 point, int number)
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
                    Vector3 wantedposition = homePoint + (_FlankDirections[number] * flankNodeDistance);


                    float distance1 = Vector3.Distance(_FlankNodes[number].transform.position, homePoint);
                    float distance2 = Vector3.Distance(wantedposition, homePoint);


                    if (distance1 < distance2)
                    {
                        _FlankNodes[number].transform.position = homePoint + (_FlankDirections[number] * flankNodeDistance);
                    }
                    return;
                }
                else
                {
                    distanceToPlayer = distance;
                }
                startLocation = hit.point + (Vector2)direction;
            }
        }

        _FlankNodes[number].transform.position = homePoint + (_FlankDirections[number] * flankNodeDistance);
    }
    public void CheckHomePointAvailability()
    {
        distanceFromHomepoint = Vector3.Distance(homePoint, player.transform.position);

        if (distanceFromHomepoint > maxDistance)
        {
            homePoint = player.transform.position;
            DrawFlankNodes();
            CheckAvailableNodes();
        }
    }
    private void RemoveElement<T>(ref T[] arr, int index)
    {

        for (int i = index; i < arr.Length - 1; i++)
        {
            arr[i] = arr[i + 1];
        }

        Array.Resize(ref arr, arr.Length - 1);
    }
    public void SendNodes()
    {
        foreach (FlankerAiManager _ai in ai)
        {
            _ai.setFlankers();
            _ai.availableNodes = availableNodes;
            _ai._AvailableNodes = _AvailableNodes;
            _ai.PriorityNodes.Clear();
            _ai.GiveFlanksPriority();
            _ai.flankers[0].FindClosetsNode(0);
        }
    }
}
