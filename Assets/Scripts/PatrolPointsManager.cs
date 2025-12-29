using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPointsManager : MonoBehaviour
{
    public List<Transform> patrolPoints = new List<Transform>();
    // Start is called before the first frame update
    void Start()
    {
        LoadComponent();
    }

    void LoadComponent()
    {
        foreach(Transform child in transform)
        {
            patrolPoints.Add(child);
        }
    }

}
