using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FlankerAiManager : MonoBehaviour
{
    public Transform player;
    public Vector3 homePoint;
    public GameObject[] _AvailableNodes;
    public int[] availableNodes;
    public Dictionary<int, float[]> PriorityNodes = new Dictionary<int, float[]>();
    public FlankerAi[] flankers;
    public int _FirstFlank;
    public NodeManager nodemanager;

   
    void Start()
    {
        homePoint = player.transform.position;
    }

    public void setFlankers()
    {
        foreach (FlankerAi _flanker in flankers)
        {
            _flanker.state = EnemyAI.State.PathFinding;
        }
    }
    public void GiveFlanksPriority()
    {
        int nodeCount = 0;
        int nodeCount2 = 0;
        foreach (GameObject _node in _AvailableNodes)
        {
            float[] nodes = new float[_AvailableNodes.Length];
            foreach (GameObject _node2 in _AvailableNodes)
            {
                if(_node != _node2)
                {
                    if(nodes.Length >= nodeCount2)
                    {
                        nodes[nodeCount2] = givePriority(_node.transform, _node2.transform);
                    }
                }
                nodeCount2++;
            }

       
            PriorityNodes.Add(availableNodes[nodeCount], nodes);

            nodeCount2 = 0;
            nodeCount++;
        }
    }
    public float givePriority(Transform start, Transform end)
    {
        Vector3 startDir = start.position - homePoint;
        Vector3 endDir = end.position - homePoint;

        endDir.Normalize();
        startDir.Normalize();

        float diff = Vector3.Distance(endDir, startDir);
        diff = Mathf.Abs(diff);

        return diff;
    }
    public void attack()
    {
        foreach (FlankerAi _flanker in flankers)
        {
            _flanker.attack();
        }
    }

}
