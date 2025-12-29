using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class FieldOfView : MonoBehaviour
{
    [Header("Settings")]
    public float radius = 15f;
    [Range(0,360)]
    public float angle = 160f;
    public float delay = 0.2f;

    [Header("References")]
    public GameObject playerRef;
    public LayerMask targetMask;
    public LayerMask obstructionMask;

    [Header("Status")]
    public bool canSeePlayer;

    private Collider[] _overlapBuffer = new Collider[1];

    // Start is called before the first frame update
    void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");
        radius = 15f;
        angle = 160f;
        StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(this.delay);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position, radius, _overlapBuffer, targetMask);

        if (count > 0)
        {
            Transform target = _overlapBuffer[0].transform;

            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                    canSeePlayer = true;
                else
                    canSeePlayer = false;
            }
            else
                canSeePlayer = false;
        }
        else if (canSeePlayer)
            canSeePlayer = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        //Gizmos.DrawWireSphere(transform.position, radius);

        #if UNITY_EDITOR
        Handles.color = Color.white;
        Vector3 viewAngle01 = DirectionFromAngle(transform.eulerAngles.y, -angle / 2);
        Vector3 viewAngle02 = DirectionFromAngle(transform.eulerAngles.y, angle / 2);

        Handles.DrawLine(transform.position, transform.position + viewAngle01 * radius);
        Handles.DrawLine(transform.position, transform.position + viewAngle02 * radius);

        Handles.DrawWireArc(transform.position, Vector3.up, viewAngle01, angle, radius);
        
        if (canSeePlayer && playerRef != null)
        {
            Handles.color = Color.green;
            Handles.DrawLine(transform.position, playerRef.transform.position);
        }
        #endif
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
