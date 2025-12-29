using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropZoneKey : DropZoneTrigger
{
    [SerializeField] private bool isCame = false;
    [SerializeField] private GameObject key;
    [SerializeField] private AudioClip dropSound;
    [SerializeField] private DialogueData dialogueData;
    
    private void OnTriggerExit(Collider other)
    {
        if (!isCame) isCame = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCame && other.CompareTag("Player") && key != null)
        {
            key.SetActive(true);
            Rigidbody itemRigidbody = key.GetComponent<Rigidbody>();
            AudioSource audioSource = key.GetComponent<AudioSource>();
            if (itemRigidbody != null && audioSource != null)
            {
                Debug.Log("[DropZoneKey] Have Rigidbody!");
                itemRigidbody.isKinematic = false;
                audioSource.PlayOneShot(dropSound);
            }
            if (dialogueData != null)
            {
                DialogueManager.Instance.SetTargetDialogueBox(key.transform);
                DialogueManager.Instance.ShowDialogueBox(dialogueData);

            }
            Destroy(this);
        }
    }
}
