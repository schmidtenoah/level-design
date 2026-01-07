using UnityEngine;

[CreateAssetMenu(fileName = "ParticlesSO", menuName = "SO/ParticlesSO", order = 1)]
public class ParticlesSO : ScriptableObject
{
    
    public GameObject _ignition;
    [Tooltip("Explosion der Bombe in Reihenfolge des Bombentyp-Enums!")]
    public GameObject[] _explosions = new GameObject[System.Enum.GetValues(typeof(Bomb.BombType)).Length];
    [Tooltip("Explosion der Verbreitung in Reihenfolge des Bombentyp-Enums!")]
    public GameObject[] _spreadExplosions = new GameObject[System.Enum.GetValues(typeof(Bomb.BombType)).Length];
    
}
