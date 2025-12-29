using EazyCamera.Legacy;
using HorrorGame3D.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LandlordMovement : NPCMovement
{

    public float turnSpeed = 5f;
    [SerializeField] private bool found = false;

    override protected void Update()
    {
        base.Update();
        if (!move) return;
        if (!isLastPoint && !agent.pathPending && agent.remainingDistance < stoppingDistance)
        {
            GoToNextWaypoint();
        }


        if (isLastPoint && !agent.pathPending && agent.remainingDistance < stoppingDistance)
        {
            animator.SetFloat(animationSpeedParameter, 0f);
            agent.isStopped = true;
        }
        
    }

}
