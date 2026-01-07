using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(PlayerHealthUI))]
public class PlayerHealthManager : MonoBehaviour
{
    private PlayerHealthUI _playerHealthUI;
    
    [FormerlySerializedAs("infectionTime")]
    [Header("Infection")]
    [SerializeField] private float _infectionTime = 0.01f;
    [SerializeField] private float _playerResistancePoints = 20f;
    [SerializeField] private float _regenPoints = 0.03f;
    public PlayerStatus _status { get; private set; } = PlayerStatus.PLAYER_PLAYING;
    private float _infectionTimer;
    private bool _isInfected;
    private float _infectionDmg;
    private float _resPoints;
    
    public enum PlayerStatus
    {
        PLAYER_DEAD,
        PLAYER_WON,
        PLAYER_PLAYING
    }
    
    /**
     * Setzt alle Eventlistener und initialisiert das UI.
     */
    private void OnEnable()
    {
        _playerHealthUI = GetComponent<PlayerHealthUI>();
        PlayerHurtbox.OnVirusHitPlayer += TakeVirusDamage;
        InfectionZone.OnPlayerExitInfectionZone += NoMoreInfected;
        InfectionZone.OnPlayerEnterInfectionZone += Infect;
        PlayerInventory.OnWinRecipeCrafted += OnWinRecipeCrafted;
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
     * Eventlistener abbestellen.
     */
    private void OnDisable()
    {
        PlayerHurtbox.OnVirusHitPlayer -= TakeVirusDamage;
        InfectionZone.OnPlayerExitInfectionZone -= NoMoreInfected;
        InfectionZone.OnPlayerEnterInfectionZone -= Infect;
        PlayerInventory.OnWinRecipeCrafted -= OnWinRecipeCrafted;
    }

    /**
     * Der Spieler ist nicht mehr infiziert und erhält somit keinen Schaden mehr.
     */
    private void NoMoreInfected()
    {
        _isInfected = false;
        _playerHealthUI.NoMoreInfectedUI();
    }

    /**
     * Der Spieler ist infiziert und erhält ab jetzt den übergebenen Schaden.
     * @param infectionDamage der Schaden der von dem Spieler ab jetzt abgezogen wird
     */
    private void Infect(float infectionDamage)
    {
        _isInfected = true;
        _infectionDmg = infectionDamage;
        _playerHealthUI.InfectUI();
    }

    /**
     * Der Spieler gewinnt, weil er das Win-Rezept erstellt hat.
     * Das Spiel wird pausiert.
     */
    private void OnWinRecipeCrafted()
    {
        _status = PlayerStatus.PLAYER_WON;
        Time.timeScale = 0.0f;
        _playerHealthUI.WinRecipeCraftedUI();
    }

    /**
    * Der Spieler verliert, weil er von einem Virus infiziert wurde.
    * Das Spiel wird pausiert.
    */
    private void TakeVirusDamage()
    {
        _status = PlayerStatus.PLAYER_DEAD;
        _playerHealthUI.ShowDeathUI();
        Time.timeScale = 0.0f;
    }

    /**
     * Zieht dem Spieler die derzeitigen Schadenspunkte ab und lässt ihn sterben, sobald
     * er keine resistance points mehr hat.
     */
    private void TakeInfectionDamage()
    {
        _resPoints -= _infectionDmg;
        _playerHealthUI.UpdateHealthText(_resPoints, _playerResistancePoints);
        
        if (_resPoints <= 0)
        {
            _status = PlayerStatus.PLAYER_DEAD;
            _playerHealthUI.ShowDeathUI();
            Time.timeScale = 0.0f;
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
            _playerHealthUI.UpdateHealthText(_resPoints, _playerResistancePoints);
        }
    }
    
    /**
     * Einmaliges darstellen der UI am Anfang und Initialisierung mit Default-Werten.
     */
    private void Start()
    {
        _infectionTimer = 0.0f;
        _infectionDmg = 0.0f;
        _isInfected = false;
        _resPoints = _playerResistancePoints;
        _playerHealthUI.UpdateHealthText(_resPoints, _playerResistancePoints);
    }
    
}
