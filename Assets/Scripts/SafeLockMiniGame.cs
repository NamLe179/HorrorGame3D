using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HorrorGame3D.Interaction
{
	public class SafeLockMiniGame : MonoBehaviour
	{
		[Header("UI")]
		[SerializeField] private SafeLockUI ui;

		[Header("References")]
		public Transform dial; // object thật trong scene
		public Transform focusPoint; // gán FocusPoint (position+rotation)
		
		[SerializeField] private Transform player;
		[SerializeField] private Camera playerCamera;
		[SerializeField] private Camera safeCamera;
		private List<MonoBehaviour> playerControlScripts = new List<MonoBehaviour>(); // các script di chuyển / xoay

		[Header("Audio")]
		public AudioSource rotateSound;
		public AudioSource clickSound;

		[Header("Lock Settings")]
		public int[] combination = { 42, 17, 88 };
		public float rotationSpeed = 25f;	// số per second -> 100 nghĩa 1 vòng/1s
		public float correctThreshold = 0.5f;   // cho dễ ăn số
		public float unlockDelay = 1.2f;       // đợi sau số cuối rồi open

		private int step = 0;
		private bool isActive = false;

		public System.Action OnUnlock;

		void Start()
		{
			if (player == null)
			{
				player = GameObject.FindGameObjectWithTag("Player")?.transform;
			}

			// ✅ Thu thập tất cả script điều khiển 
			CachePlayerControlScripts();

			if (dial == null) Debug.LogWarning("SafeLockMiniGame: dial not assigned!");
			if (focusPoint == null) Debug.LogWarning("SafeLockMiniGame: focusPoint not assigned!");
		}

		void Update()
		{
			if (!isActive) return;

			if (Input.GetKeyDown(KeyCode.Q))
			{
				ExitMiniGame();
				return;
			}

			float input = Input.GetAxis("Horizontal");
			if (Mathf.Abs(input) > 0.01f)
			{
				dial.Rotate(Vector3.forward, input * rotationSpeed * Time.deltaTime);
				if (rotateSound && !rotateSound.isPlaying)
					rotateSound.Play();
			}

			if(step < combination.Length)
			{
				float difference = Mathf.DeltaAngle(dial.localEulerAngles.z, (100 - combination[step]) * 3.6f);
				if (Mathf.Abs(difference) <= correctThreshold * 3.6f)
				{
					if (clickSound) clickSound.Play();
					if (ui) ui.UpdateStep(step, combination[step]);
					step++;

					if (step >= combination.Length)
						StartCoroutine(UnlockDelay());
				}
			}
			
		}

		private void CachePlayerControlScripts()
		{
			playerControlScripts.Clear();
			if (player == null) return;

			// Lấy các script ở Player
			foreach (var script in player.GetComponents<MonoBehaviour>())
			{
				if (script == this) continue;
				string name = script.GetType().Name.ToLower();
				if (name.Contains("move") || name.Contains("interact") || name.Contains("controller"))
					playerControlScripts.Add(script);
			}

			// Lấy script ở Camera con
			var cam = player.GetComponentInChildren<Camera>();
			if (cam != null)
			{
				foreach (var script in cam.GetComponents<MonoBehaviour>())
				{
					string name = script.GetType().Name.ToLower();
					if (name.Contains("look") || name.Contains("camera"))
						playerControlScripts.Add(script);
				}
			}
		}

		private void TogglePlayerControls(bool enable)
		{
			if (playerControlScripts != null)
			{
				foreach (var s in playerControlScripts)
				{
					if (s != null) s.enabled = enable;
				}
			}

			// nếu Player có Rigidbody: stop velocity when disabling
			// (nếu enable==false) set velocity = zero

			if(enable == false)
			{
				var rb = player.GetComponent<Rigidbody>();
				if (rb != null)
					rb.velocity = Vector3.zero;
			}
		}

		public void StartMiniGame()
		{
			if(ui) ui.ShowUI();
			isActive = true;
			step = 0;

			StartCoroutine(Focus());
		}

		private IEnumerator Focus()
		{
			if (!playerCamera || !safeCamera) yield break;

			if (playerCamera) playerCamera.enabled = false;
			if (safeCamera) safeCamera.enabled = true;

			TogglePlayerControls(false);

			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}

		private IEnumerator Restore()
		{
			if (!playerCamera) yield break;

			if (safeCamera) safeCamera.enabled = false;
			if (playerCamera) playerCamera.enabled = true;

			TogglePlayerControls(true);

			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		private IEnumerator UnlockDelay()
		{
			yield return new WaitForSeconds(unlockDelay);
			OnUnlock?.Invoke();
			ExitMiniGame();
		}

		private void ExitMiniGame()
		{
			if (ui) ui.HideUI();
			isActive = false;
			StartCoroutine(Restore());
		}

	}
}
