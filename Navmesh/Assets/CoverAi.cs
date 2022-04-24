using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CoverAi : MonoBehaviour
{
    public FlankerAi flankerai;
    public CoverManager coverManager;
    public GameObject cover;
    public NavMeshAgent agent;

    public Transform player;

    private void Start()
    {
        flankerai = GetComponent<FlankerAi>();
        player = GameObject.Find("Player").GetComponent<Transform>();
        agent = GetComponentInChildren<NavMeshAgent>();
    }
    public void Update()
    {
        //checks if they are seeking cover
        if(flankerai.state == EnemyAI.State.CoverSeeking)
        {
            if(cover == null)
            {
                GetCoverToHide();
            }
            CheckIfNotSeen();

            //if close enough to cover set the foundCover state
            if (agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance < 2)
            {
                flankerai.state = EnemyAI.State.foundCover;
            }
        }

        //checks if both A.I are ready to flank
        coverManager.InPosition();

    }

    //Goes through a list of inrange covers or hideable covers
    //It checks the cover distance from the player and from the path it would take after finding cover
    public void GetCoverToHide()
    {
        float distance = 0;
        GameObject node = null;


        float distance2 = 0;

        if(coverManager.inRange.Count <= 0)
        {
            foreach (GameObject _cover in coverManager.hideable)
            {
                distance2 = Vector3.Distance(_cover.transform.position, flankerai._RouteNodes[flankerai.stepCount - 1].transform.position);
                if (!coverManager.used.Contains(_cover))
                {
                    if (distance == 0)
                    {
                        distance = Vector3.Distance(_cover.transform.position, player.position) + distance2;
                        node = _cover;
                    }

                    float newDistance = Vector3.Distance(_cover.transform.position, player.position) + distance2;

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
                distance2 = Vector3.Distance(_cover.transform.position, flankerai._RouteNodes[flankerai.stepCount -1].transform.position);
                if (!coverManager.used.Contains(_cover))
                {
                    if (distance == 0)
                    {
                        distance = Vector3.Distance(_cover.transform.position, player.position) + distance2;
                        node = _cover;
                    }

                    float newDistance = Vector3.Distance(_cover.transform.position, player.position) + distance2;

                    if (distance > newDistance)
                    {
                        distance = newDistance;
                        node = _cover;
                    }
                }
            }
        }

        if(!coverManager.used.Contains(node))
        {
            coverManager.used.Add(node);
        }
   
        cover = node;

        if(cover == null)
        {

        }
        else
        {
            agent.SetDestination(cover.transform.position);
        }

    }

    //Check if the cover stays unseen to the player
    public void CheckIfNotSeen()
    {
        if (coverManager.found.Contains(cover))
        {
            GetCoverToHide();
            coverManager.used.Remove(cover);
            cover = null;
        }
    }

  
}
