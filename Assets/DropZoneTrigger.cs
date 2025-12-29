using HorrorGame3D.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropZoneTrigger : MonoBehaviour
{
    [SerializeField] protected GameObject itemToDrop;
    [SerializeField] private DoorWithKeyObject doorWithKeyObject;
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        Rigidbody itemRigidbody = itemToDrop.GetComponent<Rigidbody>();
    //        if (itemRigidbody != null)
    //        {
    //            Debug.Log("Have Rigidbody!");
    //            itemRigidbody.isKinematic = false;
    //        }
    //        Destroy(this);
    //    }
    //}
    private void Start()
    {
        doorWithKeyObject.OnInteract += Drop;
    }
    private void Drop()
    {
        if (itemToDrop != null)
        {
            if (itemToDrop == null) return;
            Rigidbody itemRigidbody = itemToDrop.GetComponent<Rigidbody>();
            if (itemRigidbody != null)
            {
                Debug.Log("[Drop] Have Rigidbody!");
                itemRigidbody.isKinematic = false;
            }
        }
        doorWithKeyObject.OnInteract -= Drop;
    }
}