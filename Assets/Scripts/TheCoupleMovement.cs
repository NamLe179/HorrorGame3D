using EazyCamera.Legacy;
using HorrorGame3D.Interaction;
using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering.UI;

[RequireComponent(typeof(NavMeshAgent))]
public class TheCoupleMovement : NPCMovement
{
    

    protected override void Update()
    {

        SetAnimationSpeed();
        if (!move) return;
        if (!isLastPoint && !agent.pathPending && agent.remainingDistance < stoppingDistance)
        {
            GoToNextWaypoint();
        }
        if (isLastPoint && !agent.pathPending && agent.remainingDistance < stoppingDistance)
        {
            animator.SetFloat(animationSpeedParameter, 0f);
            agent.isStopped = true;
            Disappear();
        }

    }
    

    protected void Disappear()
    {
        agent.enabled = false;
        Debug.Log($"NPC {gameObject.name} complete waypoints and dissappear!");
        Destroy(gameObject);
    }
    
}
