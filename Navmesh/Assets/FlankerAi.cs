using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlankerAi : MonoBehaviour
{
    [SerializeField] private LayerMask layer, outerLayer;

    public Transform player;
    public Vector3 homePoint;
    public float distanceFromHomepoint;
    public float maxDistance;

    public float minimalDistance;

    [Header("List of nodes")]
    public GameObject[] _FlankNodes;
    public Vector3[] _FlankDirections;   
    public Vector3[] _RouteNodes;
    public Vector3[] _AvailableNodes;


    public float flankNodeDistance;


    [Header("Prefabs")]
    public GameObject prefab;


    public GameObject closetNode;
    public float _DistanceToFirstNode;
    public int currentNode;
    public int lastNode;

    public float minimalNeededDistance;
    public float distanceFromNode;
    
    
    void Start()
    {
        _DistanceToFirstNode = 0;
        _FlankNodes = new GameObject[8];
        _AvailableNodes = new Vector3[8];
        for (int i = 0; i < _FlankNodes.Length; i++)
        {        
            _FlankNodes[i] = Instantiate(prefab);
            
        }

        homePoint = player.transform.position;
        DrawFlankNodes();
        FindClosetsNode();
        CheckHomePointAvailability();
    }

    // Update is called once per frame
    void Update()
    {
        CheckHomePointAvailability();
        FindClosetsNode();

    }

    //Draw 8 squares each coming from the home point
    public void DrawFlankNodes()
    {
        for (int i = 0; i < _FlankNodes.Length; i++)
        {

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
            Debug.DrawLine(homePoint, endPoint, Color.black);

          
            
            
        }
    }


    //Code for checking if the node can see the player without walls
    //Color the nodes red if they have vision or blue if vision is blocked
    public void CheckAvailableNodes()
    {
        for (int i = 0; i < _FlankNodes.Length; i++)
        {
            RaycastHit2D hit = Physics2D.Linecast(homePoint, _FlankNodes[i].transform.position, layer);
            float distanceToNode = Vector3.Distance(player.transform.position, _FlankNodes[i].transform.position);

            if (_FlankNodes[i] != closetNode)
            {
                if (!hit && distanceToNode >= minimalNeededDistance)
                {
                    Debug.DrawRay(homePoint, _FlankDirections[i], Color.green);
                    _FlankNodes[i].GetComponent<SpriteRenderer>().color = Color.red;

                }
                else
                {
                    Debug.Log(hit.collider);
                    _FlankNodes[i].GetComponent<SpriteRenderer>().color = Color.cyan;
                }
            }
        }
    }

    //Code to find the closest node you want the A.I to move too
    public void FindClosetsNode()
    {
        lastNode = currentNode;
        float distance;
        for (int i = 0; i < _FlankNodes.Length; i++)
        {
            
            distance = Vector3.Distance(_FlankNodes[i].transform.position, transform.position);

            if(_DistanceToFirstNode == 0)
            {
                _DistanceToFirstNode = distance;
                closetNode = _FlankNodes[i];
                currentNode = i;
            }

            if (_DistanceToFirstNode > distance)
            {
                _DistanceToFirstNode = distance;
                closetNode = _FlankNodes[i];
                currentNode = i;

            }

            
        }

        _DistanceToFirstNode = 0;
        if(currentNode != lastNode)
        {
            closetNode.GetComponent<SpriteRenderer>().color = Color.yellow;
            _FlankNodes[lastNode].GetComponent<SpriteRenderer>().color = Color.cyan;

        }
        
    }

    //Check if the player hasn't moved to far away from the homepoint and reset the homepoints position to the player
    public void CheckHomePointAvailability()
    {
        distanceFromHomepoint = Vector3.Distance(homePoint, player.transform.position);

        if(distanceFromHomepoint > maxDistance)
        {
            homePoint = player.transform.position;
            DrawFlankNodes();
            CheckAvailableNodes();  
        }
    }
}
