using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCPointsManager : MonoBehaviour
{
    public List<Transform> waypoints = new List<Transform>();

    void Start()
    {
        LoadComponent();
    }

    void LoadComponent()
    {
        foreach (Transform child in transform)
        {
            waypoints.Add(child);
        }
    }
}
