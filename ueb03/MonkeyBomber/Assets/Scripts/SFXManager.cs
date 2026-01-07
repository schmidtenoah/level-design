using UnityEngine;

public class SFXManager : MonoBehaviour
{
    
    [SerializeField] private AudioClipsSO _audioClips;
    [SerializeField] private float _alarmStartThreshold = 0.25f;
    
    private bool _hasAlarmPlayed = false;
    private static AudioSource _src;

    /**
    * Fügt dem GameObject einen AudioSource-Komponente hinzu 
    * und konfiguriert sie für nicht-räumlichen (2D) Sound.
    */
    private void Awake()
    {
        _src = gameObject.AddComponent<AudioSource>();
        _src.spatialBlend = 0f;
    }

    /**
    * Registriert Event-Handler für alle relevanten Spielereignisse, 
    * um passende Soundeffekte auszulösen.
    */
    private void Start()
    {
        Bomb.OnBombExploded += OnBombExploded;
        BlockElevator.OnElevatorStart += OnEnterElevator;
        Item.OnItemCollected += OnItemCollected;
        BombInventory.OnPlaceBombSuccess += OnBombPlaced;
        GameManager.OnGameLost += OnGameLost;
        GameManager.OnGameWon += OnGameWon;
        GameManager.OnTimeChange += OnTimeChange;
    }

    /**
    * Hebt alle Event-Registrierungen beim Deaktivieren der Komponente auf.
    */
    private void OnDisable()
    {
        Bomb.OnBombExploded -= OnBombExploded;
        BlockElevator.OnElevatorStart -= OnEnterElevator;
        Item.OnItemCollected -= OnItemCollected;
        BombInventory.OnPlaceBombSuccess -= OnBombPlaced;
        GameManager.OnGameLost -= OnGameLost;
        GameManager.OnGameWon -= OnGameWon;
        GameManager.OnTimeChange -= OnTimeChange;
    }

    /**
    * Spielt den Soundeffekte ab.
    */
    private void OnBombExploded() => PlaySound(_audioClips._bombExplosion);
    
    /**
    * Spielt den Soundeffekte ab.
    */ 
    private void OnEnterElevator() => PlaySound(_audioClips._elevatorTravel);
    
    /**
    * Spielt den Soundeffekte ab.
    */
    private void OnItemCollected() => PlaySound(_audioClips._collectedItem);
    
    /**
    * Spielt den Soundeffekte ab.
    */
    private void OnBombPlaced() => PlaySound(_audioClips._placeBomb);
    
    /**
    * Spielt den Soundeffekte ab.
    */
    private void OnGameLost() => PlaySound(_audioClips._lost);
    
    /**
    * Spielt den Soundeffekte ab.
    */
    private void OnGameWon() => PlaySound(_audioClips._won);

    /**
    * Überwacht den verbleibenden Zeitanteil und spielt 
    * einmalig einen Alarmton, wenn ein kritischer Wert unterschritten wird.
    *
    * @param t Der verbleibende Zeitanteil
    */
    private void OnTimeChange(float t)
    {
        if (t >= _alarmStartThreshold)
        {
            _hasAlarmPlayed = false;
        }
        
        if (_hasAlarmPlayed || t >= _alarmStartThreshold) 
            return;
        
        PlaySound(_audioClips._alarm);
        _hasAlarmPlayed = true;
    }
    
    /**
     * Spielt den übergebenen Audio-Clip an Position und mit Lautstärke ab.
     * @param audioClip die Audio die abgespielt werden soll
     * @param volume die Lautstärke des Sounds
     */
   private static void PlaySound(AudioClip clip, float volume = 0.7f) => _src.PlayOneShot(clip, Mathf.Clamp01(volume));
    
}
