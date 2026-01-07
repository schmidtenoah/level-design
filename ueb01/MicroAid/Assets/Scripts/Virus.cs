using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Virus : MonoBehaviour
{

    public delegate void VirusCaptureAction(Virus virus);

    public event VirusCaptureAction OnVirusCaptured;
    
    private void OnDisable()
    {
        OnVirusCaptured?.Invoke(this);
    }
}
