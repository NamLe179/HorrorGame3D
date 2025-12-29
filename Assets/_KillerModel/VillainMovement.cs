using HorrorGame3D.Interaction;
using SojaExiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class VillainMovement : SelfMonoBehaviour
{
	[SerializeField] protected Transform target;
	[SerializeField] protected Transform lastTarget;
	[SerializeField] protected NavMeshAgent navMeshAgent;
	[SerializeField] public State currentState = State.IDLE;

	[Header("Detection")]
    [SerializeField] private float sightRange = 15f;
    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private float hearRange = 20f;
    [SerializeField] private LayerMask sightLayerMask;
    [SerializeField] private Transform eyeTransform;

	[Header("Chase")]
	public float chaseSpeed = 4f;
	public float pathfindingInterval = 0.2f;
	public float stopDistance = 3f;

	[Header("Patrol")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float patrolRange = 12f;
    [SerializeField] private float minDistanceBetweenPoints = 3f;
    [SerializeField] private Vector3 currentPatrolTarget;
	public PatrolPointsManager patrolPointsManager;
	public PatrolPointsManager patrolPointsManagerfloor1;
	public PatrolPointsManager patrolPointsManagerfloor2;
	public PatrolPointsManager patrolPointsManagerfloor3;
	int pointCurrent = 0;

	[SerializeField] public bool isPaused = false;

	private float pathUpdateDeadline;
	private FieldOfView fov;

	[Header("=== Auto Open Doors ===")]
	[SerializeField] float doorCheckDistance = 2f;
	[SerializeField] LayerMask doorLayer = ~0;
	private float nextDoorCheckTime = 0f;
	private const float DOOR_CHECK_INTERVAL = 0.2f;
    [Range(10f, 90f)]
    public float maxViewAngle = 45f;

	[Header("Animation")]
	public Animator animator;
	[Header("Audio")]
    public AudioSource audioSource;
    public AudioSource audioBackgroundSource;

    public AudioClip idleSound;
    public AudioClip walkingSound;
    public AudioClip chasingSound;
	public AudioClip backgroundSound;
    public bool isAttacking = false;

	public enum State
	{
		IDLE,
		CHASING,
		PATROL
	}

	protected void Start()
	{
		navMeshAgent = GetComponent<NavMeshAgent>();
		animator = GetComponent<Animator>();
		fov = GetComponent<FieldOfView>();
        patrolPointsManagerfloor1 = GameObject.Find("PatrolPointManagerFloor1").GetComponent<PatrolPointsManager>();
		patrolPointsManagerfloor2 = GameObject.Find("PatrolPointManagerFloor2").GetComponent<PatrolPointsManager>();
		patrolPointsManagerfloor3 = GameObject.Find("PatrolPointManagerFloor3").GetComponent<PatrolPointsManager>();

		PauseGameInput.Instance.OnGamePaused += OnPauseGame;
		PauseGameInput.Instance.OnGameResume += OnResumeGame;
		PauseGameInput.OnLoadMinigame += OnPauseGame;
		PauseGameInput.OnUnloadMinigame += OnResumeGame;
        if (navMeshAgent == null)
			Debug.LogError("NavMeshAgent not found!");
		if (fov == null)
			Debug.LogWarning("FieldOfView not found!");

		navMeshAgent.speed = patrolSpeed;
		navMeshAgent.stoppingDistance = this.stopDistance;

		currentState = State.PATROL;
		FindNewPatrolPoint();
	}

	protected void FixedUpdate()
	{
		if (isPaused) return;
        PlayBackgroundSound(backgroundSound);
        switch (currentState)
		{
			case State.IDLE:
				Idle();
				PlaySound(idleSound);
                if (DetectPlayer()) SwitchToChase();
				break;

			case State.CHASING:
				Chase();
				PlaySound(chasingSound);
                if (!DetectPlayer()) SwitchToPatrol();
				break;

			case State.PATROL:
				Patroling();
				PlaySound(walkingSound);
                if (DetectPlayer()) SwitchToChase();
				break;
		}
		navMeshAgent.stoppingDistance = stopDistance;

		UpdateAnimation();
	}

	#region State Transitions
	private void SwitchToChase()
	{
		currentState = State.CHASING;
		navMeshAgent.speed = chaseSpeed;
		navMeshAgent.stoppingDistance = stopDistance;
		pathUpdateDeadline = 0f;
	}

	private void SwitchToPatrol()
	{
		isAttacking = false;
		currentState = State.PATROL;
		navMeshAgent.speed = patrolSpeed;
		navMeshAgent.stoppingDistance = 0;
		this.lastTarget = target;
		SetTarget(null);

		FindNewPatrolPoint();
	}
	#endregion

	#region Detection
	private bool DetectPlayer()
	{
		bool canSee = CheckSight();

		if (canSee)
		{
			SetTarget(fov.playerRef.transform);
			return true;
		}
		return false;
	}

	private bool CheckSight()
	{
		if (fov == null) return false;
		return fov.canSeePlayer;
	}

	#endregion

	#region States
	void Idle()
	{
		navMeshAgent.isStopped = true;
	}

	void Chase()
	{
		if (target == null || navMeshAgent == null) return;

		navMeshAgent.isStopped = false;

		if (Time.time >= pathUpdateDeadline)
		{
			navMeshAgent.SetDestination(target.position);
			pathUpdateDeadline = Time.time + pathfindingInterval;
			if (Time.time >= nextDoorCheckTime)
			{
				TryOpenDoorAhead();
				nextDoorCheckTime = Time.time + DOOR_CHECK_INTERVAL;
			}
		}

		float distanceToTarget = Vector3.Distance(transform.position, target.position);
		if (distanceToTarget <= stopDistance)
		{
			Debug.Log("stop and attack");
			navMeshAgent.isStopped = true;
			LookAtTarget();
			isAttacking = true;
		}
		else
		{
			Debug.Log("[Chase] Chasing...");
			navMeshAgent.isStopped = false;
			LookAtTarget();
			isAttacking = false;
		}
	}
	void Patroling()
	{
		if (navMeshAgent == null) return;

		navMeshAgent.isStopped = false;

		// Nếu chưa có điểm hoặc đã đến gần điểm hiện tại
		if (!navMeshAgent.hasPath || navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance + 0.5f)
		{
			FindNewPatrolPoint();
		}
	}

	private void FindNewPatrolPoint()
	{
		if (lastTarget != null)
		{
			navMeshAgent.SetDestination(lastTarget.position);
			lastTarget = null;
			return;
		}

		//Vector3 randomPoint;
		//int maxAttempts = 15;
		//int attempts = 0;

		//do
		//{
		//	randomPoint = transform.position + Random.insideUnitSphere * patrolRange;
		//	randomPoint.y = transform.position.y;
		//	attempts++;
		//}
		//while (attempts < maxAttempts &&
		//	   Vector3.Distance(transform.position, randomPoint) < minDistanceBetweenPoints);



		//NavMeshHit hit;
		//if (NavMesh.SamplePosition(randomPoint, out hit, patrolRange, NavMesh.AllAreas))
		//{
		//	currentPatrolTarget = hit.position;
		//	navMeshAgent.SetDestination(currentPatrolTarget);
		//}

		if (patrolPointsManager != FindFloorToPatrol())
		{
			patrolPointsManager = FindFloorToPatrol();
			pointCurrent = -1;
        }
		;
        //patrolPointsManager = FindAnyObjectByType<PatrolPointsManager>();
		if (patrolPointsManager == null)
		{
			currentState = State.IDLE;
			return;
        }
        if (patrolPointsManager.patrolPoints.Count != 0)
		{
			pointCurrent++;
			if (pointCurrent >= patrolPointsManager.patrolPoints.Count)
			{
				pointCurrent = 0;
            }
			SetTarget(patrolPointsManager.patrolPoints[pointCurrent]);
            navMeshAgent.SetDestination(patrolPointsManager.patrolPoints[pointCurrent].position);
        }
		else
		{
			Debug.LogWarning("No patrol points found in PatrolPointsManager.");
        }

		if (Time.time >= nextDoorCheckTime)
		{
			TryOpenDoorAhead();
			nextDoorCheckTime = Time.time + DOOR_CHECK_INTERVAL;
		}
	}

	PatrolPointsManager FindFloorToPatrol()
	{
		if (fov.playerRef != null)
		{
			float playerY = fov.playerRef.transform.position.y;
			if (playerY <= 8f)
			{
				return patrolPointsManagerfloor1;
			}
			else if (playerY <= 12f)
			{
				return patrolPointsManagerfloor2;
			}
			else
			{
				return patrolPointsManagerfloor3;
			}
        }
		return FindAnyObjectByType<PatrolPointsManager>();
    }
	#endregion

	#region Utilities
	private void PlayBackgroundSound(AudioClip clip)
	{
		if (audioBackgroundSource == null) return;
        if (audioBackgroundSource.clip == clip && audioBackgroundSource.isPlaying)
		{
			return;
		}
		audioBackgroundSource.clip = clip;
		audioBackgroundSource.loop = true;
		audioBackgroundSource.Play();
    }

	private void PlaySound(AudioClip clip)
	{
		if (audioSource == null) return;
        if (audioSource.clip == clip && audioSource.isPlaying)
		{
			return;
		}
		audioSource.clip = clip;
		audioSource.Play();
	}

	public virtual void SetTarget(Transform newTarget)
	{
		this.target = newTarget;
	}

	private void LookAtTarget()
	{
		if (target == null) return;

		Vector3 lookPos = target.position - transform.position;
		lookPos.y = 0;
		if (lookPos != Vector3.zero)
		{
			Quaternion rotation = Quaternion.LookRotation(lookPos);
			transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 10f);
		}
	}

	private void UpdateAnimation()
	{
		if (animator == null) return;
		animator.SetBool("isRunning", currentState == State.CHASING);
		animator.SetBool("isWalking", currentState == State.PATROL);
		animator.SetBool("Attacking", isAttacking);
	}

	private bool IsGoingThroughDoor(Transform transform)
	{
		Vector3 npcToDoor = transform.position - this.transform.position;
		Vector3 destToDoor = transform.position - target.position;

		npcToDoor.y = 0;
		destToDoor.y = 0;

		float dotProduct = Vector3.Dot(npcToDoor.normalized, destToDoor.normalized);

		return dotProduct < 0.1;
	}

	//private void TryOpenDoorsOnPath()
	//{
	//	if (!navMeshAgent.hasPath) return;

	//	foreach (Vector3 corner in navMeshAgent.path.corners)
	//	{
	//		foreach (Collider col in Physics.OverlapSphere(corner, doorCheckDistance, doorLayer))
	//		{
	//			if (col.TryGetComponent<InteractableDoorBase>(out var door))
	//			{
	//				if (!door.IsOpen() && door.CanInteract())
	//				{
	//					if (IsGoingThroughDoor(door.transform))
	//					{
	//						door.OpenDoor();
	//					}
	//				}
	//			}
	//		}
	//	}
	//}
	private void Display()
	{
		this.gameObject.SetActive(false);
	}

	private void Show()
	{
		this.gameObject.SetActive(true);
	}
	private void TryOpenDoorAhead()
	{
        if (navMeshAgent.velocity.sqrMagnitude < 0.1f) return;

        Vector3 origin = transform.position + Vector3.up * 1f; 
        Vector3 dir = target.position - transform.position;

        if (Physics.Raycast(origin, dir, out RaycastHit hit, doorCheckDistance, doorLayer))
        {
            Debug.Log("Door detected ahead: " + hit.collider.name);
            Vector3 directionToDoor = (hit.point - origin).normalized;

			float angleToDoor = Vector3.Angle(transform.forward, directionToDoor);

			if (angleToDoor > this.maxViewAngle)
			{
				return;
			}

			var door = hit.collider.GetComponent<InteractableDoorBase>();
            if (door == null)
            {
                door = hit.collider.GetComponentInChildren<InteractableDoorBase>();
            }
            Debug.Log("Door detected ahead: " + hit.collider.name);
            if (door != null)
            {

                Debug.Log("Door detected InteractableDoorBase Object");
                if (!door.IsOpen() && door.CanInteract())
                {
                    door.OpenDoor();
                }
            }
        }
    }

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			Debug.Log("Be Catched!!");
			CutSceneLoader.endingID = 0;
			SceneManager.LoadScene("CutScenePlayer");
		}
	}

    #endregion

    #region Gizmos
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;

    //    Vector3 origin = transform.position + Vector3.up * 1f;

    //    Vector3 dir = transform.forward;

    //    Gizmos.DrawRay(origin, dir * doorCheckDistance);

    //    Gizmos.color = Color.yellow;

    //    Vector3 leftRay = Quaternion.Euler(0, -maxViewAngle, 0) * transform.forward;
    //    Vector3 rightRay = Quaternion.Euler(0, maxViewAngle, 0) * transform.forward;

    //    Gizmos.DrawRay(origin, leftRay * doorCheckDistance);
    //    Gizmos.DrawRay(origin, rightRay * doorCheckDistance);
    //}

    #endregion

    public void OnPauseGame()
    {
		Debug.Log("Pause");
		isPaused = true;
        // 1. Dừng âm thanh ngay lập tức
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Pause();
        }

        if (audioBackgroundSource != null && audioBackgroundSource.isPlaying)
        {
            audioBackgroundSource.Pause();
        }

        // 2. Đóng băng NavMeshAgent để tránh lỗi trượt
        if (navMeshAgent != null && navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.velocity = Vector3.zero;
        }
		
        if (animator != null)
        {
			Debug.Log(animator.speed);
            animator.speed = 0f;
        }

    }

    // Hàm này gọi khi Resume game
    public void OnResumeGame()
    {
		Debug.Log("UnPause");
		isPaused = false;
        if (audioSource != null)
        {
            audioSource.UnPause();
        }

        if (audioBackgroundSource != null)
        {
            audioBackgroundSource.UnPause();
        }

        if (navMeshAgent != null && navMeshAgent.isOnNavMesh)
        {
            if (currentState != State.IDLE)
            {
                navMeshAgent.isStopped = false;
            }
        }
        if (animator != null)
        {
            animator.speed = 1f;
        }
    }

}