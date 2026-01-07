using UnityEngine;

public class BloodCell : MonoBehaviour
{
    
    [SerializeField] private int _damageAmmoGain;
    
    /**
     * Getter für die Munition dieser Blutzelle.
     */
    public int GetAmmo() => _damageAmmoGain;

    /**
     * Setzt die Munition dieser Blutzelle.
     * @param ammoGain
     */
    public void Init(int ammoGain) => _damageAmmoGain = ammoGain;

}
