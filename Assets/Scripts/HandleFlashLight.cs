using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleFlashLight : MonoBehaviour
{
    [SerializeField] protected Camera mainCamera;
    [SerializeField] protected float rotationSpeed = 5.0f;
    [SerializeField] protected float maxDistance = 100f;

    public Vector3 aimOffset = new Vector3(-90, -90, -90);
    // Start is called before the first frame update
    void Start()
    {
        if (mainCamera == null)
        {
            Camera camera = transform.root.GetComponentInChildren<Camera>();
            if (camera != null)
            {
                mainCamera = camera;
                Debug.Log("[HandleFlashLight] Main Camera assigned from parent hierarchy.");
            }
            else
            {
                Debug.LogWarning("[HandleFlashLight] Main Camera not assigned and not found in parent hierarchy.");
            }
        }
    }

    private void LateUpdate()
    {
        UpdateLightDirection();
    }

    void UpdateLightDirection()
    {
        if (mainCamera == null) return;

        Vector3 targetPoint;

        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(maxDistance);
        }

        //Vector3 directionToTarget = targetPoint - transform.position;

        //Quaternion baseLookRotation = Quaternion.LookRotation(directionToTarget);

        //Quaternion targetRotation = baseLookRotation * Quaternion.Euler(aimOffset);

        Vector3 directionToTarget = targetPoint - transform.position;

        Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, directionToTarget);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        Debug.DrawLine(mainCamera.transform.position, targetPoint, Color.green);
        Debug.DrawRay(transform.position, transform.forward * 10f, Color.red);
        Debug.DrawRay(transform.position, transform.up * 5f, Color.yellow);
    }
}
