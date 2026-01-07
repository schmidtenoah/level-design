using System;
using UnityEngine;

public class PlayerHurtbox : MonoBehaviour
{
    
    public static event Action OnVirusHitPlayer;
    public static event Action<GameObject> OnHitPlayer;
    public static event Action<CollectableItemSO> OnCollect;
    public static event Action<GameObject> OnCollectSuccessfull;
    
    /**
     * Achtet auf Kollisionen mit dem Strahl der Viren und allen Items.
     * @param other Collider des anderen Game-Objektes
     */
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Beam"))
        {
            HandleBeamCollision(other.gameObject);
        }
        else if (other.TryGetComponent(out CollectableItem collectable))
        {
            HandleCollectable(collectable);
        }
    }

    /**
     * Löst die Events für ein aufgesammeltes Item aus und löscht dieses danach.
     * @param collectable das Item das aufgesammelt wurde
     */
    private void HandleCollectable(CollectableItem collectable)
    {
        OnCollect?.Invoke(collectable.GetItemData());
        OnCollectSuccessfull?.Invoke(collectable.gameObject);
        Destroy(collectable.gameObject); 
    }

    /**
     * Löst die Events für einen Treffer mit dem Strahl der Viren aus.
     * @param virusObj der Virus der getroffen wurde
     */
    private void HandleBeamCollision(GameObject virusObj)
    {
        OnVirusHitPlayer?.Invoke();
        OnHitPlayer?.Invoke(virusObj);
    }
    
}
