using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlankerAi : MonoBehaviour
{
    [SerializeField] private LayerMask layer;

    public Transform player;
    public Vector3 homePoint;

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


    
    
    void Start()
    {
        _DistanceToFirstNode = 0;
        _FlankNodes = new GameObject[8];
        for (int i = 0; i < _FlankNodes.Length; i++)
        {        
            _FlankNodes[i] = Instantiate(prefab);
        }
    }

    // Update is called once per frame
    void Update()
    {
        homePoint = player.transform.position;

        DrawFlankNodes();
        FindClosetsNode();
        CheckAvailableNodes();


    }



    public void DrawFlankNodes()
    {
        for (int i = 0; i < _FlankNodes.Length; i++)
        {
            Vector3 endPoint = homePoint + (_FlankDirections[i] * flankNodeDistance);

            Debug.DrawLine(homePoint, endPoint, Color.black);

            _FlankNodes[i].transform.position = endPoint;
            
            
        }
    }
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
    public void CheckAvailableNodes()
    {
        for (int i = 0; i < _FlankNodes.Length; i++)
        {
            RaycastHit2D hit = Physics2D.Linecast(homePoint, _FlankNodes[i].transform.position, layer);
            if (!hit)
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
