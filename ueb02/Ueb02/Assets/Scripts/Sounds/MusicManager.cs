using UnityEngine;

public class MusicManager : MonoBehaviour
{

    [Header("Background-Music Settings")]
    [SerializeField] private float _defaultPitchBckg = 1f;
    [SerializeField] private float _defaultVolumeBckg = 0.5f;
    
    [Header("Ambient-Music Settings")]
    [SerializeField] private float _defaultPitchAmb = 1f;
    [SerializeField] private float _defaultVolumeAmb = 0.5f;
    
    [Header("General")] 
    [SerializeField] private float _fadeDuration = 2f;
    
    private AudioSource _backgroundMusic;
    private AudioSource _ambientMusic;
    
    /**
     * Nur die Ambient-Musik wird am Anfang abgespielt.
     * Setzt die Referenzen zur Audio.
     */
    private void Awake()
    {
        _backgroundMusic = transform.Find("BackgroundAudio").GetComponent<AudioSource>();
        _ambientMusic = transform.Find("AmbientAudio").GetComponent<AudioSource>();
        _backgroundMusic.volume = 0.0f;
    }

    /**
     * Abonniert alle Music-Events.
     */
    private void OnEnable()
    {
        MusicEvents.OnChangeMusicMood += SetMood;
        MusicEvents.OnResetMusicMood += ResetMood;
        InfectionZone.OnPlayerEnterInfectionZone += PlayBackgroundMusic;
        InfectionZone.OnPlayerExitInfectionZone += SwitchToAmbient;
    }

    /**
     * Spielt nur die Hintergrundmusik ab.
     * @param infectionIntensity wird nicht benötigt
     */
    private void PlayBackgroundMusic(float infectionIntensity)
    {
        SwitchToBackground();
    }
    
    /**
     * Deabonniert die Music-Events.
     */
    private void OnDisable()
    {
        MusicEvents.OnChangeMusicMood -= SetMood;
        MusicEvents.OnResetMusicMood -= ResetMood;
        InfectionZone.OnPlayerEnterInfectionZone -= PlayBackgroundMusic;
        InfectionZone.OnPlayerExitInfectionZone -= SwitchToAmbient;
    }
    
    /**
     * Verändert den pitch beider Audioquellen.
     * @param pitch neuer Pitch beider Audioquellen
     */
    private void SetMood(float pitch)
    {
        _ambientMusic.pitch = pitch;
        _backgroundMusic.pitch = pitch;
    }

    /**
     * Setzt den Pitch beider Audioquellen wieder auf Default.
     */
    private void ResetMood()
    {
        _backgroundMusic.pitch = _defaultPitchBckg;
        _ambientMusic.pitch = _defaultPitchAmb;
    }
    
    /**
     * Blendet die Ambient-Musik langsam ein.
     */
    private void SwitchToAmbient()
    {
        StopAllCoroutines();
        StartCoroutine(FadeMusic(_backgroundMusic, _defaultVolumeBckg, 0f));
        StartCoroutine(FadeMusic(_ambientMusic, 0f, _defaultVolumeAmb));
    }

    /**
     * Blendet die Hintergrund-Musik langsam ein.
     */
    private void SwitchToBackground()
    {
        StopAllCoroutines();
        StartCoroutine(FadeMusic(_backgroundMusic, 0f, _defaultVolumeBckg));
        StartCoroutine(FadeMusic(_ambientMusic, _defaultVolumeAmb, 0f));
    }

    /**
     * Coroutine die eine Audioquelle langsam einblendet.
     * @param source die Audioquelle die eingeblendet werdn soll
     * @param from von wo aus geblendet werden soll
     * @param to bis wohin geblendet werden soll
     */
    private System.Collections.IEnumerator FadeMusic(AudioSource source, float from, float to)
    {
        float time = 0f;
        while (time < _fadeDuration)
        {
            source.volume = Mathf.Lerp(from, to, time / _fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }
        source.volume = to;
    }
    
}
