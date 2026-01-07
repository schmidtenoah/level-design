using System;
using UnityEngine;

public class WeaponController : MonoBehaviour
{

    public static event Action<WeaponType> OnWeaponShot;
    public static event Action<WeaponType> OnWeaponChange;
    public enum WeaponType { WEAPON_NONE,  WEAPON_HEAL,  WEAPON_DAMAGE }
    
    [Header("Heal Weapon")]
    [SerializeField] private Weapon _healWeapon;
    [SerializeField] private int _healAmmoStart;
    [SerializeField] private LiquidAmmoDisplay _liquidHealAmmoDisplay;
    private int _currHealAmmo;
    
    [Header("Damage Weapon")]
    [SerializeField] private Weapon _damageWeapon;
    [SerializeField] private int _damageAmmoStart;
    [SerializeField] private LiquidAmmoDisplay _liquidDamageAmmoDisplay;
    private int _currDamageAmmo;

    [Header("General")]
    [SerializeField] private WeaponType _starterWeapon;
    [SerializeField] private float _switchWeaponCooldown = 0.5f;
    [SerializeField] private int _minGainAmmo = 100;
    private WeaponType _currWeapon;
    private float _currSwitchWeaponCooldown;
    
    /**
     * Defaultwerte initialisieren.
     */
    private void Start()
    {
        _currHealAmmo = _healAmmoStart;
        _currSwitchWeaponCooldown = 0.0f;
        _currDamageAmmo = _damageAmmoStart;
        SwitchWeapon(_starterWeapon);
        _liquidHealAmmoDisplay.UpdateAmount(_currHealAmmo, _healAmmoStart);
        _liquidDamageAmmoDisplay.UpdateAmount(_currDamageAmmo, _damageAmmoStart);
    }

    /**
     * Relevante Events abonnieren.
     */
    private void Awake()
    {
        PlayerHurtbox.OnBloodCellCollect += GainDamageAmmo;
        PlayerHurtbox.OnPillCollect += GainHealAmmo;
    }

    /**
     * Alle Events wieder abbestellen.
     */
    private void OnDisable()
    {
        PlayerHurtbox.OnBloodCellCollect -= GainDamageAmmo;
        PlayerHurtbox.OnPillCollect -= GainHealAmmo;
    }

    /**
     * Die Damage-Waffe erhält mehr Munition.
     * @param ammoCnt ganzer Prozentwert
     */
    private void GainDamageAmmo(int ammoCnt)
    {
        if (_currDamageAmmo <= 0)
        {
            _damageAmmoStart = _minGainAmmo;
        }
        
        _currDamageAmmo += (int) Math.Abs((ammoCnt / 100f) * _damageAmmoStart);
        if (_currDamageAmmo >= _damageAmmoStart)
        {
            _damageAmmoStart = _currDamageAmmo;
        }
        _liquidDamageAmmoDisplay.UpdateAmount(_currDamageAmmo, _damageAmmoStart);
    }
    
    /**
     * Die Heal-Waffe erhält mehr Munition.
     * @param ammoCnt ganzer Prozentwert
     */
    private void GainHealAmmo(int ammoCnt)
    {
        if (_currHealAmmo <= 0)
        {
            _healAmmoStart = _minGainAmmo;
        }
        
        _currHealAmmo += (int) Math.Abs((ammoCnt / 100f) * _healAmmoStart);
        if (_currHealAmmo >= _healAmmoStart)
        {
            _healAmmoStart = _currHealAmmo;
        }
        _liquidHealAmmoDisplay.UpdateAmount(_currHealAmmo, _healAmmoStart);
    }

    /**
     * Cooldown aktualisieren.
     */
    private void Update() => _currSwitchWeaponCooldown += Time.deltaTime;

    /**
     * InputAction-Event zum Wechseln der aktuellen Waffe.
     */
    private void OnSwitchWeapon()
    {
        if (_currSwitchWeaponCooldown < _switchWeaponCooldown) 
            return;

        _currSwitchWeaponCooldown = 0.0f;
        SwitchWeapon(_currWeapon.Equals(WeaponType.WEAPON_DAMAGE) ? WeaponType.WEAPON_HEAL : WeaponType.WEAPON_DAMAGE);
    }

    /**
     * Wechselt die aktive Waffe.
     * @param toType die Art der Waffe zu der gewechselt werden soll
     */
    private void SwitchWeapon(WeaponType toType)
    {
        _currWeapon = toType;
        _damageWeapon.gameObject.SetActive(_currWeapon.Equals(WeaponType.WEAPON_DAMAGE));
        _healWeapon.gameObject.SetActive(_currWeapon.Equals(WeaponType.WEAPON_HEAL));
        OnWeaponChange?.Invoke(_currWeapon);
    }
    
    /**
     * InputAction-Event zum Feuern der aktuellen Waffe.
     */
    private void OnFire()
    {
        switch (_currWeapon)
        {
            case WeaponType.WEAPON_HEAL:
            {
                if (_currHealAmmo <= 0) 
                    return;
                
                if (_healWeapon.Fire() && _currHealAmmo > 0)
                {
                    OnWeaponShot?.Invoke(_currWeapon);
                    --_currHealAmmo;
                    _liquidHealAmmoDisplay.UpdateAmount(_currHealAmmo, _healAmmoStart);
                }
                break;
            }
            case WeaponType.WEAPON_DAMAGE:
            {
                if (_currDamageAmmo <= 0)
                    return; 
                
                if (_damageWeapon.Fire() && _currDamageAmmo > 0)
                {
                    OnWeaponShot?.Invoke(_currWeapon);
                    --_currDamageAmmo;
                    _liquidDamageAmmoDisplay.UpdateAmount(_currDamageAmmo, _damageAmmoStart);
                }
                break;
            }
            case WeaponType.WEAPON_NONE:
            default: break;
        }
    }
    
}
