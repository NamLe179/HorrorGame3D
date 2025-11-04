using UnityEngine;

namespace HorrorGame3D.Interaction
{
	public abstract class InteractableBase : MonoBehaviour, IInteractable
	{
		[SerializeField] protected KeyData requiredKey; 

		[SerializeField] protected bool isLocked = false;

		[SerializeField] protected bool isOpen = false;

		private const string INTERACTABLE_LAYER = "Interactable";
		public abstract void Interact(Transform player);
		public abstract string GetPromptMessage();

		// Gán layer khi object được khởi tạo
		protected virtual void Awake()
		{
			SetLayerRecursively(gameObject, INTERACTABLE_LAYER);
		}


		public virtual bool CanInteract()
		{
			// Nếu object bị khóa nhưng không có chìa → không thể tương tác
			if (isLocked && requiredKey == null)
				return false;

			// Nếu có chìa khóa trong inventory (có thể mở rộng sau)
			return true;
		}

		

		// 🔧 Hàm tiện ích: gán layer cho object và toàn bộ con của nó
		private void SetLayerRecursively(GameObject obj, string layerName)
		{
			int layer = LayerMask.NameToLayer(layerName);

			if (layer == -1)
			{
				Debug.LogWarning($"⚠️ Layer '{layerName}' Not Found! Pls add layer in Project Settings > Tags and Layers.");
				return;
			}

			obj.layer = layer;

			foreach (Transform child in obj.transform)
			{
				SetLayerRecursively(child.gameObject, layerName);
			}
		}

        public string GetInteractionText()
        {
            throw new System.NotImplementedException();
        }

        public void Interact(GameObject interactor)
        {
            throw new System.NotImplementedException();
        }
    }
}
