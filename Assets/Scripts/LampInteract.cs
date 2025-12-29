using System.Collections;
using UnityEngine;

namespace HorrorGame3D.Interaction
{
	public class LampInteract : ToggleInteractableBase
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

		public override IEnumerator Open()
		{
			//animator.Play("openpullopp");
			foreach (var light in lightSources)
			{
				if (light != null)
				{
					light.enabled = true;
				}
			}
			if (sound) sound.Play();
			isOpen = true;
			yield return new WaitForSeconds(0.5f);
		}

		public override IEnumerator Close()
		{
			//animator.Play("closepushopp");
			foreach (var light in lightSources)
			{
				if (light != null)
				{
					light.enabled = false;
				}
			}
			if (sound) sound.Play();
			isOpen = false;
			yield return new WaitForSeconds(0.5f);
		}
	}
}
