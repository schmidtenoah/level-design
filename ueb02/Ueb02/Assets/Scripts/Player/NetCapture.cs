using System;
using UnityEngine;

public class NetCapture : MonoBehaviour
{
    
    public static event Action<GameObject> OnNetCapture;
    
    /**
     * Wenn ein Virus in den aktivierten Collider eintritt, wird das OnNetCapture-Event
     * ausgelöst und der Virus zerstört.
     * @param other der Collider des anderen Objekt des Eintritts
     */ 
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Virus"))
        {
            OnNetCapture?.Invoke(other.gameObject);
            Destroy(other.gameObject); 
        }
    }
    
}
