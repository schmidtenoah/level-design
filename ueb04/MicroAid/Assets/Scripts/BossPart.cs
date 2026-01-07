using System;
using UnityEngine;

public class BossPart : Target
{
    public static event Action OnBossPartShot;
    public static event Action OnAnyBossPartDefeated; 
    public event Action<BossPart> OnBossPartDefeated;
    
    [SerializeField] private Material _hurtMaterial;
    private Material _startMat;
    private Renderer _renderer;
    
    [SerializeField] private ParticleSystem _hitParticle;

    [SerializeField] private GameObject _targetVisualization;
    private bool _isDamageable;
    
    /**
     * Defaultwerte initialisieren.
     */
    private void Awake()
    {
        _isDamageable = false;
        _renderer = GetComponentInChildren<Renderer>();
        _startMat = _renderer.material;
        _currHealth = _maxHealth;
        _targetVisualization?.SetActive(false);
    }

    /**
     * Initialisiert bossteil-spezifische Eigenschaften.
     * @param maxHealth die HP des Bossteils
     * @param enableHit ob der Boss angreifbar ist
     */
    public void Init(float maxHealth, bool enableHit)
    {
        _maxHealth = maxHealth;
        _currHealth = _maxHealth;
        EnableHit(enableHit);
    }

    /**
     * Macht diesen Bossteil angreifbar oder nicht.
     * @param enable ob der Boss angreifbar sein soll
     */
    public void EnableHit(bool enable)
    {
        _targetVisualization.SetActive(enable);
        _isDamageable = enable;
    }

    /**
     * Getter für die aktuellen HP.
     * @returns die aktuellen HP
     */
    public float GetCurrHealth() => _currHealth;
    
    /**
     * Getter für die max. HP.
     * @returns die max. HP
     */
    public float GetMaxHealth() => _maxHealth;
    
    /**
     * Getter ob dieser Bossteil verletzbar ist.
     * @returns ob dieser Bossteil verletzbar
     */
    public bool IsDamageable() => _isDamageable;
    
    public override void Got(WeaponController.WeaponType weapon, float hitPoints)
    {
        if (!_isDamageable || !weapon.Equals(WeaponController.WeaponType.WEAPON_DAMAGE))
            return;

        _currHealth -= hitPoints;
        Instantiate(_hitParticle, transform.position + Vector3.up * 3f, Quaternion.identity);
        ChangeMatColor();
        
        if (_currHealth <= 0)
        {
            OnAnyBossPartDefeated?.Invoke();
            OnBossPartDefeated?.Invoke(this);
        }
        
        OnBossPartShot?.Invoke();
    }
    
    /**
     * Visualisiert, dass der Boss Schaden bekommen hat.
     */
    private void ChangeMatColor() => _renderer.material.Lerp(_startMat, _hurtMaterial, _currHealth / _maxHealth);
    
}
