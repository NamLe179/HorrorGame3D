using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCMovement : MonoBehaviour
{
    protected NavMeshAgent agent;
    protected Animator animator;
    [SerializeField] public ConversationTrigger conversationTrigger;
    [HideInInspector] private bool isPaused = false;

    [Header("Target to follow")]
    public Transform target;
    public NPCPointsManager pointsManager;
    public int currentPointIndex = 0;
    public bool isLastPoint = false;
    public float stoppingDistance = 0.5f;
    public bool move = false;

    [Header("Animator")]
    public string animationSpeedParameter = "Speed";
    public float animationDampTime = 0.1f;
    private float currentAnimationSpeed = 0f;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;
    [Header("Speed of the agent")]
    public float speed = 2f;



    protected void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent.speed = speed;
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component not found on " + gameObject.name);
            return;
        }

        if (animator == null)
        {
            Debug.LogError("Animator component not found on " + gameObject.name);
            return;
        }

        if (pointsManager == null)
        {
            Debug.LogError("NPCPointsManager component not found on " + gameObject.name);
            return;
        }

        agent.autoBraking = false;
    }

    protected virtual void Start()
    {
        PauseGameInput.Instance.OnGamePaused += OnPauseGame;
        PauseGameInput.Instance.OnGameResume += OnResumeGame;
        conversationTrigger.OnEnterTrigger += Moving;
    }

    protected virtual void Update()
    {
        if (isPaused) return;
        SetAnimationSpeed();
        
    }

    private void Moving()
    {
        move = true;
        conversationTrigger.OnEnterTrigger -= Moving;
    }

    protected void SetAnimationSpeed()
    {
        if (animator == null) return;

        float actualSpeed = agent.velocity.magnitude;
        float targetAnimationSpeed = actualSpeed;

        currentAnimationSpeed = Mathf.Lerp(currentAnimationSpeed, targetAnimationSpeed, Time.deltaTime);

        if (agent.velocity.magnitude < 0.1f)
        {
            currentAnimationSpeed = 0f;
        }
        animator.SetFloat(animationSpeedParameter, currentAnimationSpeed);
    }

    protected void GoToNextWaypoint()
    {
        if (pointsManager.waypoints.Count == 0) return;

        if (!isLastPoint && currentPointIndex < pointsManager.waypoints.Count)
        {
            SetTarget(pointsManager.waypoints[currentPointIndex]);
            FindPath();
            currentPointIndex++;
        }
        if (currentPointIndex >= pointsManager.waypoints.Count)
        {
            isLastPoint = true;

            return;
        }

    }

    private void FindPath()
    {
        agent.SetDestination(target.position);
    }



    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }


    protected void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = UnityEngine.Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.position, FootstepAudioVolume);
            }
        }
    }
    public void OnPauseGame()
    {
        isPaused = true;

        if (agent != null && agent.isOnNavMesh) 
        {
            agent.isStopped = true; 
            agent.velocity = Vector3.zero; 
        }

    }

    public void OnResumeGame()
    {
        isPaused = false;

        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = false; 
        }
    }
}
