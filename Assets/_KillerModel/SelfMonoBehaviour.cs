using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfMonoBehaviour : MonoBehaviour
{
    // Author: Quy
    // Start is called before the first frame update
    protected void Awake()
    {
        this.LoadComponents();
    }

    protected void Reset()
    {
        this.LoadComponents();
    }
    /// <summary>
    /// For override in derived classes to load components.
    /// </summary>
    protected virtual void LoadComponents()
    {
        
    }
}
