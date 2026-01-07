using UnityEngine;

public class Pill : MonoBehaviour
{
    
    [SerializeField] private int _healAmmoGain;
    
    /**
     * Getter für die Heal-Munition dieser Pille.
     * @returns die Heal-Munition dieser Pille
     */
    public int GetAmmo() => _healAmmoGain;
    
}
