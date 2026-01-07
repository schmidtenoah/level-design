using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    
    [SerializeField] private float _defaultPitch = 1f;
    [SerializeField] private float _defaultVolume = 1.0f;
    
    [SerializeField] private float _infectionPitch = 2f;
    [SerializeField] private float _infectionVolume = 1.0f;
    [SerializeField] private float _fadeDuration = 2f;

    private AudioSource _audio;
    private Coroutine _transitionCoroutine;
    
    /**
     * Setzt die Referenzen zur Audioquelle.
     */
    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        _audio.volume = _defaultVolume;
        _audio.pitch = _defaultPitch;
    }

    /**
     * Abonniert alle Music-Events.
     */
    private void OnEnable()
    {
        InfectionZone.OnPlayerEnterInfectionZone += PlayInfectedState;
        InfectionZone.OnPlayerExitInfectionZone += PlayNormalState;
    }
    
    /**
     * Deabonniert die Music-Events.
     */
    private void OnDisable()
    {
        InfectionZone.OnPlayerEnterInfectionZone -= PlayInfectedState;
        InfectionZone.OnPlayerExitInfectionZone -= PlayNormalState;
    }
    
    /** Spielt die Hintergrundmusik in normalem Pitch und Volume ab. */
    private void PlayNormalState() => StartTransition(_defaultPitch, _defaultVolume);

    /** Spielt die Hintergrundmusik im angepassten Pitch und Volume ab. */
    private void PlayInfectedState(float notUsed) => StartTransition(_infectionPitch, _infectionVolume);

    /**
     * Startet die Transition vom aktuellen Pitch/Volume zum angepassten.
     * @param targetPitch Pitch am Ende der Transition
     * @param targetVolume Volume am Ende der Transition
     */
    private void StartTransition(float targetPitch, float targetVolume)
    {
        if (_transitionCoroutine != null)
        {
            StopCoroutine(_transitionCoroutine);
        }

        _transitionCoroutine = StartCoroutine(TransitionToState(targetPitch, targetVolume));
    }

    /**
     * Coroutine zum Wechseln  vom aktuellen Pitch/Volume zum angepassten.
     * @param targetPitch Pitch am Ende der Transition
     * @param targetVolume Volume am Ende der Transition
     * @returns Wert für die Coroutine
     */
    private System.Collections.IEnumerator TransitionToState(float targetPitch, float targetVolume)
    {
        float startPitch = _audio.pitch;
        float startVolume = _audio.volume;

        float t = 0;
        while (t < _fadeDuration)
        {
            t += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(t / _fadeDuration);
            _audio.pitch = Mathf.Lerp(startPitch, targetPitch, normalizedTime);
            _audio.volume = Mathf.Lerp(startVolume, targetVolume, normalizedTime);
            yield return null;
        }

        _audio.pitch = targetPitch;
        _audio.volume = targetVolume;
    }
    
}
