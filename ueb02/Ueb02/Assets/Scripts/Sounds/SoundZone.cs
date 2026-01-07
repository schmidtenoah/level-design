using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
public class SoundZone : MonoBehaviour
{
    
    [Header("Audio")]
    [SerializeField] private float _minDelay = 2f;
    [SerializeField] private float _maxDelay = 10f;
    [SerializeField] private Vector2 _pitchRange = new (1.2f, 2.0f);
    private AudioSource _audioSource;
    
    [Header("Music Mood")]
    [SerializeField] private float _minMusicPitch = 1.5f;
    [SerializeField] private float _maxMusicPitch = 1.0f;
    
    private Collider _collider;
    private bool _playerInside;
    private Coroutine _soundRoutine;

    /**
     * Referenzen setzen.
     */
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _collider = GetComponent<Collider>();
    }

    /**
     * Tritt der Spieler in die Sound-Zone ein, startet eine Coroutine die
     * den Sound in den definierten Parametern abspielt.
     * @param other anderes Kollisionsobjekt 
     */
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && _soundRoutine == null)
        {
            _playerInside = true;
            _soundRoutine = StartCoroutine(PlayRandomSounds());
        }
    }
    
    /**
     * Solange der Spieler innerhalb der Zone ist, wird jedes Mal der Abstand zum Zone-Mittelpunkt
     * berechnet und anhand dessen der Sound angepasst.
     * @param other anderes Kollisionsobjekt 
     */
    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        float distance = Vector3.Distance(other.transform.position, _collider.bounds.center);
        float t = Mathf.InverseLerp(0f, _collider.bounds.extents.magnitude, distance);
        float pitch = Mathf.Lerp(_minMusicPitch, _maxMusicPitch, t);

        MusicEvents.OnChangeMusicMood(pitch);
    }

    /**
     * Verlässt der Spieler die Zone, wird die normale Musik wiederhergestellt und die Coroutine gestoppt.
     * @param other anderes Kollisionsobjekt 
     */
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MusicEvents.OnResetMusicMood();
            _playerInside = false;
            if (_soundRoutine != null)
            {
                StopCoroutine(_soundRoutine);
                _soundRoutine = null;
            }
        }
    }

    /**
     * Spielt in einem zufällig gewähltem Pitch und Abstand den Sound ab.
     */
    private IEnumerator PlayRandomSounds()
    {
        while (_playerInside)
        {
            float delay = Random.Range(_minDelay, _maxDelay);
            yield return new WaitForSeconds(delay);
            
            _audioSource.pitch = Random.Range(_pitchRange.x, _pitchRange.y);
            _audioSource.Play();
        }
    }
    
}
