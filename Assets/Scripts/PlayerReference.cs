using UnityEngine;

public class PlayerReference : MonoBehaviour
{
	public static PlayerReference Instance;

	[Header("Reference to Player UI root")]
	public GameObject playerUI;

	private void Awake()
	{
		Instance = this;
	}
}
