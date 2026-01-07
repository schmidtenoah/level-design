using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHurtbox : MonoBehaviour
{
    
    public static event Action<Virus> OnVirusHitPlayer;
    public static event Action OnVirusLeavesPlayer;
    public static event Action<int> OnBloodCellCollect;
    public static event Action<int> OnPillCollect;
    public static event Action OnPotionCollect;
    public static event Action OnLockCollect;
    
    private readonly List<Virus> _virusInTrigger = new();
    
    /**
     * Achtet auf Kollisionen mit dem Strahl der Viren und allen Items.
     * @param other Collider des anderen Game-Objektes
     */
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Beam"))
        {
            var virus = other.GetComponentInParent<Virus>();
            OnVirusHitPlayer?.Invoke(virus);
            _virusInTrigger.Add(virus);
        }
        else if (other.TryGetComponent(out BloodCell bloodCell))
        {
            OnBloodCellCollect?.Invoke(bloodCell.GetAmmo());
            Destroy(bloodCell.gameObject);
        }
        else if (other.TryGetComponent(out Pill pill))
        {
            OnPillCollect?.Invoke(pill.GetAmmo());
            Destroy(pill.gameObject);
        }
        else if (other.CompareTag("Potion"))
        {
           OnPotionCollect?.Invoke();
           Destroy(other.gameObject);
        }
        else if (other.CompareTag("Lock"))
        {
            OnLockCollect?.Invoke();
            Destroy(other.gameObject);
        }
    }

    /**
     * Wenn ein Virus der den Spieler gerade angreift nicht mehr existiert (Spieler töten ihn),
     * dann soll er nicht mehr angegriffen werden.
     * Muss gesondert abgefangen werden, da OnTriggerExit nicht aufgerufen wird.
     */
    private void Update()
    {
        for (var i = 0; i < _virusInTrigger.Count; ++i)
        {
            if (_virusInTrigger[i].IsUnityNull())
            {
                _virusInTrigger.RemoveAt(i);
                OnVirusLeavesPlayer?.Invoke();
            }
        }
    }

    /**
     * Virus greift den Spieler nicht mehr an, wenn er den Beam verlässt.
     * @param other Collider des anderen Game-Objektes
     */
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Beam"))
        {
            OnVirusLeavesPlayer?.Invoke();
            _virusInTrigger.Remove(other.GetComponentInParent<Virus>());
        }
    }
    
}
