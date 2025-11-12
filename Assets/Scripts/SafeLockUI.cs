using TMPro;
using UnityEngine;

namespace HorrorGame3D.Interaction
{
	public class SafeLockUI : MonoBehaviour
	{
		public GameObject uiPanel;
		public TMP_Text[] numberBoxes;
		public TMP_Text instructionText;

		void Start()
		{
			uiPanel.SetActive(false);
		}

		public void ShowUI()
		{
			uiPanel.SetActive(true);
			ClearBoxes();
		}

		public void HideUI()
		{
			uiPanel.SetActive(false);
		}

		public void UpdateStep(int step, int value)
		{
			if (step >= 0 && step < numberBoxes.Length)
				numberBoxes[step].text = value.ToString();
		}

		private void ClearBoxes()
		{
			foreach (var box in numberBoxes)
				box.text = "";
		}
	}
}
