using UnityEngine;

public class DoorInteractable1 : InteractableBase
{
	private Quaternion closedRot;
	private Quaternion openRot;

	private void Start()
	{
		closedRot = transform.rotation;
		openRot = Quaternion.Euler(transform.eulerAngles + new Vector3(0, 90, 0));
	}

	public override void Interact(GameObject interactor)
	{
		isActive = !isActive; // toggle
		transform.rotation = isActive ? openRot : closedRot;
		Debug.Log(isActive ? "Door opened" : "Door closed");
	}
}
