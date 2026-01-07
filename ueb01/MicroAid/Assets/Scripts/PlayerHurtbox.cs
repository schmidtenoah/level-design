using System;
using UnityEngine;

public class PlayerHurtbox : MonoBehaviour
{
    
    public static event Action OnVirusHitPlayer;
    
    [SerializeField] private Rigidbody playerRigidbody; 
    [SerializeField] private float knockbackForce = 5f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Virus"))
        {
            OnVirusHitPlayer?.Invoke();
            Vector3 knockbackDirection = (transform.position - other.transform.position).normalized;
            knockbackDirection.y = 0f;

            Debug.Log(knockbackDirection);
            playerRigidbody.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
        }
    }
    
}
