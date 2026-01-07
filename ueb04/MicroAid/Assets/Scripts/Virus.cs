using System;
using UnityEngine;

public class Virus : Target
{
    
    public static event Action OnVirusHealed;
    public static event Action OnVirusShot;
    
    [SerializeField] private ParticleSystem _destroyedEffect;
    [SerializeField] private ParticleSystem _healEffect;
    [SerializeField] private float _infectionDamage;
    [SerializeField] private BloodCell _healedVirusPrefab;
    [SerializeField] private int _healedVirusAmmoGain = 30;

    [SerializeField] private Material _hurtMaterial;
    private Material _startMat;
    private Renderer _renderer;

    /**
     * Getter für den Infektionsschaden dieses Viruses.
     * @returns Infektionsschaden dieses Viruses
     */
    public float GetInfectionDmg => _infectionDamage;

    /**
     * Initialisiert individuelle Eigenschaften eines Virus.
     * @param health HP
     * @param dmg Infektionsschaden
     */
    public void Init(float health, float dmg)
    {
        _maxHealth = health;
        _currHealth = health;
        _infectionDamage = dmg;
    }
    
    /**
     * Werte initialisieren.
     */
    private void Start()
    {
        _renderer = GetComponentInChildren<Renderer>();
        _startMat = _renderer.material;
        _currHealth = _maxHealth;
    }
    
    public override void Got(WeaponController.WeaponType weapon, float hitPoints)
    {
        if (!weapon.Equals(WeaponController.WeaponType.WEAPON_HEAL))
            return;
        
        OnVirusShot?.Invoke();
        Instantiate(_healEffect, transform.position, Quaternion.identity);
        _currHealth -= hitPoints;
        ChangeMatColor();
        
        if (_currHealth <= 0)
        {
            OnVirusHealed?.Invoke();
            Instantiate(_destroyedEffect, transform.position, Quaternion.identity);
            
            var spawnPosition = transform.position;
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 10f, ~(1 << 9)))
            {
                spawnPosition = hit.point;
            }

            var bloodcell = Instantiate(_healedVirusPrefab, spawnPosition, Quaternion.identity);
            bloodcell.Init(_healedVirusAmmoGain);
            Destroy(gameObject);
        }

    }

    /**
     * Visualisiert, dass der Boss Schaden bekommen hat.
     */
    private void ChangeMatColor() => _renderer.material.Lerp(_startMat, _hurtMaterial, _currHealth / _maxHealth);
    
}
