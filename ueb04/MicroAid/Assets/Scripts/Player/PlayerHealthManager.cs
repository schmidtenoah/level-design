using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerHealthManager : MonoBehaviour
{

    // <currHP, maxHP>
    public static event Action<float, float> OnHPChange;
    public static event Action OnPlayerDead;
    
    [FormerlySerializedAs("infectionTime")]
    [Header("Infection")]
    [SerializeField] private float _infectionTime = 0.01f;
    [SerializeField] private float _playerResistancePoints = 20f;
    [SerializeField] private float _regenPoints = 0.03f;
    
    private float _infectionTimer;
    private bool _isInfected;
    private float _infectionDmg;
    private float _resPoints;
    private bool _insideInfectionZone;
    private float _infectionZoneDmg;
    
    /**
     * Setzt alle Eventlistener.
     */
    private void Awake()
    {
        PlayerHurtbox.OnVirusHitPlayer += TakeVirusDamage;
        PlayerHurtbox.OnVirusLeavesPlayer += VirusExit;
        InfectionZone.OnPlayerExitInfectionZone += ExitInfectionZone;
        InfectionZone.OnPlayerEnterInfectionZone += EnterInfectionZone;
    }

    /**
     * Wenn der Spieler nicht mehr vom Virus angegriffen wird.
     */
    private void VirusExit()
    {
        if (_insideInfectionZone)
        {
            _infectionDmg = _infectionZoneDmg;
        }
        else
        {
            NoMoreInfected();
        }
    }

    /**
     * Wenn der Spieler eine infizierte Zone betritt.
     * @param dmg Infizierschaden der Zone
     */
    private void EnterInfectionZone(float dmg)
    {
        _insideInfectionZone = true;
        _infectionZoneDmg = dmg;
        Infect(dmg);
    }

    /**
     * Wenn der Spieler eine infizierte Zone verlässt.
     */
    private void ExitInfectionZone()
    {
        _insideInfectionZone = false;
        NoMoreInfected();
    }

    /**
     * Eventlistener abbestellen.
     */
    private void OnDisable()
    {
        PlayerHurtbox.OnVirusHitPlayer -= TakeVirusDamage;
        PlayerHurtbox.OnVirusLeavesPlayer -= VirusExit;
        InfectionZone.OnPlayerExitInfectionZone -= ExitInfectionZone;
        InfectionZone.OnPlayerEnterInfectionZone -= EnterInfectionZone;
    }
    
    /**
     * Gibt dem Spieler seine Lebenspunkte wieder, bzw. nimmt sie weg,
     * wenn dieser Infiziert ist,
     */
    private void Update()
    {
        if (_infectionTimer < _infectionTime)
        {
            _infectionTimer += Time.deltaTime;
            return;
        }

        _infectionTimer = 0.0f;
        if (_isInfected)
        {
            TakeInfectionDamage();
        }
        else
        {
            RegainInfectionDamage();
        }
    }

    /**
     * Der Spieler ist nicht mehr infiziert und erhält somit keinen Schaden mehr.
     */
    private void NoMoreInfected() => _isInfected = false;

    /**
     * Der Spieler ist infiziert und erhält ab jetzt den übergebenen Schaden.
     * @param infectionDamage der Schaden der von dem Spieler ab jetzt abgezogen wird
     */
    private void Infect(float infectionDamage)
    {
        _isInfected = true;
        _infectionDmg = infectionDamage;
    }
    
    /**
     * Wenn der Spieler von einem Virus angegriffen wird.
     * @param virus der Virus der angreift
     */
    private void TakeVirusDamage(Virus virus) => Infect(virus.GetInfectionDmg);

    /**
     * Zieht dem Spieler die derzeitigen Schadenspunkte ab und lässt ihn sterben, sobald
     * er keine resistance points mehr hat.
     */
    private void TakeInfectionDamage()
    {
        _resPoints -= _infectionDmg;
        OnHPChange?.Invoke(_resPoints, _playerResistancePoints);

        if (_resPoints <= 0)
        {
            OnPlayerDead?.Invoke();
        }
    }

    /**
     * Gibt dem Spieler die definierten Heilungspunkte.
     */
    private void RegainInfectionDamage()
    {
        if (_resPoints < _playerResistancePoints)
        {
            _resPoints += _regenPoints;
            OnHPChange?.Invoke(_resPoints, _playerResistancePoints);
        }
    }
    
    /**
     * Initialisierung mit Default-Werten.
     */
    private void Start()
    {
        _insideInfectionZone = false;
        _infectionTimer = 0.0f;
        _infectionDmg = 0.0f;
        _isInfected = false;
        _resPoints = _playerResistancePoints;
        OnHPChange?.Invoke(_resPoints, _playerResistancePoints);
    }
    
}
