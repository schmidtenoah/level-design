using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BlockGoal : MonoBehaviour
{
    
    public static event Action OnReachedGoal;

    /**
     * Lößt das OnReachedGoal-Event aus, wenn der Spieler diesen Collider betritt.
     * @param other der Collider des anderen Objektes
     */
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnReachedGoal?.Invoke();
        }
    }
    
}
