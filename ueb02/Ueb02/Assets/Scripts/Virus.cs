using System;
using UnityEngine;

public class Virus : MonoBehaviour
{
    
    public event Action<Virus> OnVirusCaptured;
    
    /**
     * Löst das OnVirusCaptured-Event aus, sobald dieses Objekt Disabled wird.
     */
    private void OnDisable()
    {
        OnVirusCaptured?.Invoke(this);
    }
    
}
