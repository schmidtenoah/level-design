using UnityEngine;

public class Target : MonoBehaviour
{
    
    [SerializeField] protected float _maxHealth = 5.0f;
    protected float _currHealth;

    /**
     * Initialisierung der Lebenspunkte.
     */
    private void Awake() => _currHealth = _maxHealth;

    /**
     * Wird aufgerufen, wenn das Target von der Waffe mit bestimmten Schadenspunkten getroffen wurde.
     * @param weapon die Art der Waffe die das Target getroffen hat
     * @param hitPoints die Schadenspunkte der Waffe.
     */
    public virtual void Got(WeaponController.WeaponType weapon, float hitPoints) => _currHealth -= hitPoints;
 
}
