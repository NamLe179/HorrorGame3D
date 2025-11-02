using UnityEngine;

namespace HorrorGame3D.Interaction
{
	[CreateAssetMenu(fileName = "NewKey", menuName = "HorrorGame3D/KeyData")]
	public class KeyData : ScriptableObject
	{
		[Header("Key Info")]
		public string keyID;
		public string displayName;
		public string description;
	}
}
