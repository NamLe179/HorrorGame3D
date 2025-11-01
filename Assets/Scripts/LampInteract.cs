using System.Collections;
using UnityEngine;

namespace HorrorGame3D.Interaction
{
	public class LampInteract : InteractableLightBase
	{
		[SerializeField] private Light[] lightSources; 

		protected override void Awake()
		{
			base.Awake();

			// Nếu chưa gán thủ công thì tự động lấy tất cả Light con
			if (lightSources == null || lightSources.Length == 0)
			{
				lightSources = GetComponentsInChildren<Light>(true);
			}
		}

		public override void Interact(Transform player)
		{
			if (!CanInteract()) return;

			ToggleLightState();

			StartCoroutine(SwitchLightWithDelay(0.1f));

			Debug.Log($"💡 Lamp {gameObject.name} turned {(isOpen ? "ON" : "OFF")}");
		}

		public override string GetPromptMessage()
		{
			if (isLocked) return "⚙️ Power is cut off...";
			return isOpen ? "Press E to turn off lamp" : "Press E to turn on lamp";
		}

		private IEnumerator SwitchLightWithDelay(float delay)
		{
			foreach (var light in lightSources)
			{
				if (light != null)
				{
					light.enabled = isOpen;
					yield return new WaitForSeconds(delay);
				}
			}
		}
	}
}
