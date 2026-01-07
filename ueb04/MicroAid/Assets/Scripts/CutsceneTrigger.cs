using System;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
[RequireComponent(typeof(Collider))]
public class CutsceneTrigger : MonoBehaviour
{

    public static event Action OnCutsceneStarted;
    public static event Action OnCutsceneEnded;
    
    private PlayableDirector _director;
    private Collider _collider;
    [SerializeField] private GameObject[] _objInEndDestroy;
    [SerializeField] private bool _oneTimeTrigger = true;
    [Tooltip("For Debugging or Testplaying")]
    [SerializeField] private bool _doNotPlay = false;
    
    /**
     * Events abbonieren.
     */
    private void Awake()
    {
        _director = GetComponent<PlayableDirector>();
        _director.stopped += HandleCutsceneEnded;
        
        _collider = GetComponent<Collider>();
        _collider.isTrigger = true;
    }

    /**
     * Event abbstellen.
     */
    private void OnDisable()
    {
        _director.stopped -= HandleCutsceneEnded;
    }

    /**
     * Spielt die Cutscene ab, sobald der Spieler den Trigger betritt.
     * @param other Collider des triggernden Objektes
     */
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || _doNotPlay) 
            return;
        
        OnCutsceneStarted?.Invoke();
        _director.Play();

        _collider.enabled = !_oneTimeTrigger;
    }
    
    /**
     * Ruft das Event auf und zerstört die Cutscene, wenn dies angestellt ist.
     * Zerstört zudem alle Objekte die nach der Cutscene nicht mehr benötigt werden.
     * @param obj director der Cutscene die beendet wurde
     */
    private void HandleCutsceneEnded(PlayableDirector obj)
    {
        OnCutsceneEnded?.Invoke();

        foreach (var obje in _objInEndDestroy)
        {
            if (obje != null)
                Destroy(obje);
        }
        
        if (_oneTimeTrigger)
        {
            Destroy(gameObject);
        }
    }
    
}
