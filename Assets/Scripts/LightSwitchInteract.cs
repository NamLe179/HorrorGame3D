using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HorrorGame3D.Interaction
{
	public class LightSwitchInteract : ToggleInteractableBase
	{
		[SerializeField] private List<GameObject> connectedLampObjects = new List<GameObject>();

		private List<Light> cachedLights = new List<Light>();

		protected override void Awake()
		{
			base.Awake();
			RefreshCachedLights();
			ApplyLightState(isOpen);
		}

		public void RefreshCachedLights()
		{
			cachedLights.Clear();

			if (connectedLampObjects == null || connectedLampObjects.Count == 0)
			{
				Debug.Log($"💡 Lamps Not Found!!!");
			}
			else
			{
				foreach (var go in connectedLampObjects)
				{
					if (go == null) continue;
					var lights = go.GetComponentsInChildren<Light>(true);
					foreach (var l in lights) if (!cachedLights.Contains(l)) cachedLights.Add(l);
				}
			}
		}

		public override void Interact(Transform player)
		{
			if (!CanInteract()) return;

			if (cachedLights.Count == 0) RefreshCachedLights();

			// áp dụng trạng thái
			if (isOpen)
			{
				StartCoroutine(Close());
			}
			else
			{
				StartCoroutine(Open());
			}
		}

		private void ApplyLightState(bool state)
		{
			foreach (var l in cachedLights)
			{
				if (l == null) continue;
				// đảm bảo GameObject active — nếu object bị SetActive(false), không thể bật component
				if (!l.gameObject.activeInHierarchy)
				{
					// nếu muốn có hành vi bật cả gameobject, uncomment dòng bên dưới:
					// l.gameObject.SetActive(true);
				}

				l.enabled = state;
				// nếu bạn dùng emission trên vật liệu, bạn có thể xử lý ở đây (không được trong scope hiện tại)
			}

			return;
		}

		public override IEnumerator Open()
		{
			//animator.Play("open");
			ApplyLightState(true);
			if (sound) sound.Play();
			isOpen = true;
			yield return new WaitForSeconds(0.5f);
		}

		public override IEnumerator Close()
		{
			//animator.Play("close");	
			ApplyLightState(false);
			if (sound) sound.Play();
			isOpen = false;
			yield return new WaitForSeconds(0.5f);
		}
	}
}
