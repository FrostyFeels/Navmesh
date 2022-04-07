using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverManager : MonoBehaviour
{
    public FlankerAi ai;
    public FlankerAiManager flankManager;

    public List<GameObject> covers = new List<GameObject>();
    public List<GameObject> hideable = new List<GameObject>();
    public List<GameObject> found = new List<GameObject>();

    public List<GameObject> inRange = new List<GameObject>();

    public List<GameObject> used = new List<GameObject>();

    public Transform player;

    [SerializeField] private LayerMask layer;
    public void Start()
    {
        UpdateNodes();
        NodesInRange();
    }
    public void Update()
    {
        UpdateNodes();
        NodesInRange();
    }


    public void UpdateNodes()
    {
        foreach (GameObject _cover in covers)
        {
            RaycastHit2D ray = Physics2D.Linecast(_cover.transform.position, player.position, layer);


            if (!ray)
            {
                if (!found.Contains(_cover))
                {
                    found.Add(_cover);
                    hideable.Remove(_cover);
                }
            }

            if (ray)
            {
                if (!hideable.Contains(_cover))
                {
                    found.Remove(_cover);
                    hideable.Add(_cover);
                    NodesInRange();
                }
            }
        }
    }
    public void NodesInRange()
    {
        foreach (GameObject _hideable in hideable)
        {
            float distance = Vector3.Distance(_hideable.transform.position, player.position);
            if(distance <= ai.range)
            {
                if(!inRange.Contains(_hideable))
                {
                    inRange.Add(_hideable);
                }
           
            }
            else
            {
                if(inRange.Contains(_hideable))
                {
                    inRange.Remove(_hideable);
                }
            }
        }
    }
    public void InPosition()
    {
        foreach (FlankerAi _flanker in flankManager.flankers)
        {
            if (_flanker.state != EnemyAI.State.foundCover && _flanker.state != EnemyAI.State.attack && _flanker.state != EnemyAI.State.Shooting)
                return;


        }
        flankManager.attack();
    }

}
