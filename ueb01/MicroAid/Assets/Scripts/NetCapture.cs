using UnityEngine;

public class NetCapture : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Virus"))
        {
            Debug.Log("Capture Detected: " + other.gameObject.name + gameObject.name);
            Destroy(other.gameObject); 
        }
    }
}
