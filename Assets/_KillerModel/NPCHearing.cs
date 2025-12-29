using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCHearing : MonoBehaviour
{
    [Header("Hearing")]
    public float hearingThreshold = 0.1f;
    public Transform player;
    public AudioSource playerFootstepAudio;

    [Header("Chase")]
    public float chaseRange = 25f;
    private NavMeshAgent agent;
    private bool isChasing;
    private Vector3 lastHeardPos;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (player == null) player = GameObject.FindWithTag("Player").transform;
        playerFootstepAudio = player.GetComponentInChildren<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        float dist = Vector3.Distance(transform.position, player.position);
        float volumeAtNPC = GetVolumeAtDist(dist, playerFootstepAudio);
        bool canHear = volumeAtNPC > hearingThreshold && dist <= chaseRange;

        if (canHear && !isChasing) StartChase(player.position);
        else if (isChasing && dist > chaseRange * 1.2f) StopChase();

        if (isChasing) agent.SetDestination(lastHeardPos);
    }

    float GetVolumeAtDist(float dist, AudioSource source)
    {
        if (source.volume <= 0 || source.maxDistance <= 0) return 0f;
        float falloff = 1f - (dist / source.maxDistance);
        return source.volume * falloff * falloff;  // Realistic drop-off
    }

    void StartChase(Vector3 pos) 
    { 
        isChasing = true; 
        lastHeardPos = pos; 
        Debug.Log("FootStep! Chase!"); 
    }

    void StopChase() 
    { 
        isChasing = false; 
        agent.ResetPath(); 
        Debug.Log("Not Found"); 
    }
}
