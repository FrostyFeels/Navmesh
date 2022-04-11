using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingPlayer : MonoBehaviour
{
    [SerializeField] private LayerMask layer;

    public Camera mainCamera;
    LineRenderer line;

    Vector3 mousePosition;
    public int range;

    public void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    public void Update()
    {
        SetmousePosition();
        drawLines();

    }

    public void drawLines()
    {
        Vector3 endPos = mousePosition;
        Vector3 dir = endPos - transform.position;
        float dist = Mathf.Clamp(Vector3.Distance(transform.position, endPos), 0, range);
        endPos = transform.position + (dir.normalized * dist);

        line.SetPosition(0, transform.position);
        line.SetPosition(1, endPos);

        shooting(dir);
    }

    public void shooting(Vector3 dir)
    {
        dir.Normalize();
        RaycastHit2D longHit = Physics2D.Raycast(transform.position, dir, range, layer);

        if (longHit)
        {
            if (longHit.collider.CompareTag("Enemy"))
            {
                FlankerAi ai = longHit.collider.GetComponent<FlankerAi>();
                if(!ai.hiding)
                {
                    ai.hiding = true;
                }

                if(ai.ai.hits < 6)
                {
                    ai.state = EnemyAI.State.attacked;
                }            
            }
        }
    }


    private void SetmousePosition()
    {
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Cursor.visible = false;
        mouseWorldPosition.z = 0;
        mousePosition = mouseWorldPosition;
    }
}
