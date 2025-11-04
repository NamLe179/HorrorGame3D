using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HorrorGame3D.Interaction
{
	public class LightSwitchInteract : InteractableLightBase
	{
		[SerializeField] private List<GameObject> connectedLampObjects = new List<GameObject>();

		[Tooltip("Nếu true thì khi bật switch sẽ bật tất cả lights cùng lúc. Nếu false sẽ bật/tắt từng light theo thứ tự với delay.")]
		[SerializeField] private bool toggleAllAtOnce = true;

		[Tooltip("Delay giữa mỗi light khi toggle nếu toggleAllAtOnce = false.")]
		[SerializeField] private float perLightDelay = 0.05f;

		private List<Light> cachedLights = new List<Light>();

		protected override void Awake()
		{
			base.Awake();
			RefreshCachedLights();
			ApplyLightState(isOpen, immediate: true);
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

			isOpen = !isOpen; 

			if (cachedLights.Count == 0) RefreshCachedLights();

			// áp dụng trạng thái
			if (toggleAllAtOnce)
			{
				ApplyLightState(isOpen, immediate: true);
			}
			else
			{
				StopAllCoroutines();
				StartCoroutine(ApplyLightStateSequential(isOpen));
			}

			Debug.Log($"[SwitchInteract] '{name}' set lights {(isOpen ? "ON" : "OFF")} (found {cachedLights.Count} lights).");
		}

		public override string GetPromptMessage()
		{
			if (isLocked) return "⚙️ Power is cut off...";
			return isOpen ? "Press E to turn off lights" : "Press E to turn on lights";
		}

		private void ApplyLightState(bool state, bool immediate = false)
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

			if (immediate) return;
		}

		private IEnumerator ApplyLightStateSequential(bool state)
		{
			foreach (var l in cachedLights)
			{
				if (l == null) continue;
				l.enabled = state;
				yield return new WaitForSeconds(perLightDelay);
			}
		}
	}
}
