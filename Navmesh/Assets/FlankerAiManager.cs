using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlankerAiManager : MonoBehaviour
{
    [SerializeField] private LayerMask layer, outerLayer;

    public Transform player;
    public Vector3 homePoint;
    public float distanceFromHomepoint;
    public float maxDistance;



    public float flankNodeDistance;

    public GameObject prefab;

    public int availableNodesNumber;
    public int count;



    public float minimalNeededDistance;


    public float distanceToPlayer;

    public int NodeSpawnCount;

    public GameObject[] _FlankNodes;
    public GameObject[] _AvailableNodes;
    public int[] availableNodes;
    public Vector3[] _FlankDirections;

    public FlankerAi[] flankers;


    public bool _CalculatePath;
    public float _Wait;
    public float _currentWait;

    



    void Start()
    {
        _FlankNodes = new GameObject[_FlankDirections.Length];
        _AvailableNodes = new GameObject[_FlankDirections.Length];
        for (int i = 0; i < _FlankNodes.Length; i++)
        {
            _FlankNodes[i] = Instantiate(prefab);

        }

        homePoint = player.transform.position;
        DrawFlankNodes();
        CheckAvailableNodes();


        flankers[0].FindClosetsNode(0);
    }

    // Update is called once per frame
    void Update()
    {
        CheckHomePointAvailability();


        if(_CalculatePath)
        {
            _currentWait += Time.deltaTime;

            if(_currentWait >= _Wait)
            {
                flankers[0].FindClosetsNode(0);
                _currentWait = 0;
                _CalculatePath = false;
            }
        }
    }
    public void DrawFlankNodes()
    {
        for (int i = 0; i < _FlankNodes.Length; i++)
        {
            _FlankNodes[i].name = NodeSpawnCount.ToString();
            NodeSpawnCount++;
            Vector3 endPoint = homePoint + (_FlankDirections[i] * flankNodeDistance);
            RaycastHit2D hit = Physics2D.Linecast(homePoint, endPoint, outerLayer);

            if (hit)
            {
                _FlankNodes[i].transform.position = hit.point;
            }
            else
            {
                _FlankNodes[i].transform.position = endPoint;
            }
        }
        NodeSpawnCount = 0;
    }
    public void CheckAvailableNodes()
    {
        for (int i = 0; i < _FlankNodes.Length; i++)
        {
            RaycastHit2D hit = Physics2D.Linecast(homePoint, _FlankNodes[i].transform.position, layer);
            float distanceToNode = Vector3.Distance(player.transform.position, _FlankNodes[i].transform.position);

            if (!hit && distanceToNode >= minimalNeededDistance)
            {
                count++;
                _FlankNodes[i].GetComponent<SpriteRenderer>().color = Color.red;
                _AvailableNodes[i] = _FlankNodes[i];
                
            }
            else
            {

                _FlankNodes[i].GetComponent<SpriteRenderer>().color = Color.cyan;
                
                _AvailableNodes[i] = null;
                CheckWallCollision(_FlankDirections[i], hit.point, i);
            }
        }
        availableNodesNumber = count;
        availableNodes = new int[availableNodesNumber];
        count = 0;
    }
    public void CheckWallCollision(Vector3 direction, Vector3 point, int number)
    {
        Vector3 startLocation = point + direction;

        for (int i = 0; i < 10; i++)
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
    }
    public void CheckHomePointAvailability()
    {
        distanceFromHomepoint = Vector3.Distance(homePoint, player.transform.position);

        if (distanceFromHomepoint > maxDistance)
        {
            homePoint = player.transform.position;
            DrawFlankNodes();
            CheckAvailableNodes();
            _CalculatePath = true; //MAKE IT THAT IT WAITES A BIT BEFORE RUNNING THE SCRIPT. SO IT ONLY RUNS THE CODE WHEN THE PLAYER INS'T MOVING
        }
    }


}
