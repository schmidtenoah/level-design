using System;
using UnityEngine;

public class Heart : Target
{
    
    public static event Action OnHeartHealed;
    
    [SerializeField] private GameObject _mouthSad;
    [SerializeField] private GameObject _mouthHappy;
    private Quaternion _startRot;
    private Vector3 _startPos;
    private bool _canHealHeart;
    private bool _virusLeft;

    /**
     * Werte initialisieren.
     */
    private void Start()
    {
        _mouthSad.SetActive(true);
        _mouthHappy.SetActive(false);
        _canHealHeart = false;
        _virusLeft = true;
        _startRot = _mouthSad.transform.rotation;
        _startPos = _mouthSad.transform.position;
    }

    /**
     * Events bestellen.
     */
    private void Awake()
    {
        PlayerHurtbox.OnPotionCollect += CanHealHeart;
        ScoreManager.OnScoreChange += NoVirusLeft;
        _currHealth = _maxHealth;
    }

    /**
     * Events abbestellen.
     */
    private void OnDisable()
    {
        PlayerHurtbox.OnPotionCollect -= CanHealHeart;
        ScoreManager.OnScoreChange -= NoVirusLeft;
    }

    /** Setzt, ob noch Viren übrig sind. */
    private void NoVirusLeft(int score) => _virusLeft = (score != 0);

    /** Setzt, dass das Herz geheilt werden kann. */
    private void CanHealHeart() => _canHealHeart = true;
    
    public override void Got(WeaponController.WeaponType weapon, float hitPoints)
    {
        if (_virusLeft || !_canHealHeart || !weapon.Equals(WeaponController.WeaponType.WEAPON_HEAL))
            return;
        
        _currHealth -= hitPoints;
        float t = 1f - (_currHealth / _maxHealth);
        _mouthSad.transform.rotation = Quaternion.Lerp(_startRot, _mouthHappy.transform.rotation, t);
        _mouthSad.transform.position = Vector3.Lerp(_startPos, _mouthHappy.transform.position, t);
        
        if (_currHealth <= 0)
        {
            OnHeartHealed?.Invoke();
        }
    }
    
}
