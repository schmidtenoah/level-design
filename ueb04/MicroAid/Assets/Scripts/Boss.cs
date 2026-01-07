using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using WaypointsFree;
using Random = UnityEngine.Random;

public class Boss : MonoBehaviour
{
    
    //<partCount, lastBossPart>
    public static event Action<int, BossPart> OnBossHealthChange;
    public static event Action OnBossDefeated;
    public static event Action OnVirusSpawn;
    public static event Action<bool> OnPlayerNearby;
    
    [Header("Waypoints")]
    [SerializeField] private WaypointsGroup[] _waypointsGrp;
    [SerializeField] private GameObject[] _locks;
    [SerializeField] private bool _loopWaypoints;
    private List<BossPart> _bossParts;
    private int _currWaypointIdx;
    private int _currLockIdx;

    [Header("Bossparts")]
    [SerializeField] private int _partCnt = 1;
    [SerializeField] private BossPart _bossPart;
    [SerializeField] private BossPart _frontPart;
    [FormerlySerializedAs("_incFactor")]
    [Tooltip("Increases Speed when Bosspart is defeated")]
    [SerializeField] private float _speedIncFactor = 1.2f;
    [Tooltip("Increased Health for each Part")]
    [SerializeField] private float _healthInc = 50.0f;
    private WaypointsTraveler _frontPartWaypointsTraveler;

    [Header("Virus Spawn")]
    [SerializeField] private Virus _virus;
    [SerializeField] private int _maxVirusCount = 10;
    [SerializeField] private float _virusHealth = 1f;
    [SerializeField] private float _virusDmg = 1f;
    [SerializeField] private float _minInterval = 1f;
    [SerializeField] private float _maxInterval = 5f;
    [SerializeField] private float _maxMoveSpeed = 10f;
    [SerializeField] private float _minScale = 0.2f;
    [SerializeField] private float _maxScale = 2f;
    [SerializeField] private float _playerDetectRadius = 15f;
    private List<Virus> _spawnedViruses;
    private bool _spawnIsRunning = true;

    [Header("Other")]
    [SerializeField] private GameObject _potionPrefab;
    [SerializeField] private Transform _playerTransf;
    private bool _playerNearby;

    /**
     * Events abonnieren und Werte initialisieren.
     */
    private void Awake()
    {
        _spawnIsRunning = true;
        _bossParts = new List<BossPart>();
        _spawnedViruses = new List<Virus>();
        _frontPartWaypointsTraveler = _frontPart.GetComponent<WaypointsTraveler>();
        _currWaypointIdx = 0;
        _currLockIdx = 0;
        _frontPartWaypointsTraveler.Waypoints = _waypointsGrp[_currWaypointIdx];
        BossPart.OnBossPartShot += BossPartShot;
        BossPart.OnAnyBossPartDefeated += BossPartShot;
        PlayerHurtbox.OnLockCollect += LockCollected;
    }

    /**
     * Macht den hintersten Bossteil verwundbar.
     */
    private void LockCollected()
    {
        _bossParts[^1].EnableHit(true);
        OnBossHealthChange?.Invoke(_bossParts.Count, _bossParts[^1]);
    }

    /**
     * Aktualisiert die Lebensanzeige wenn ein Bossteil angegriffen wurde.
     */
    private void BossPartShot()
    {
        if (_bossParts.Count <= 0) 
            return;
        OnBossHealthChange?.Invoke(_bossParts.Count, _bossParts[^1]);
    }

    /**
     * Erstellt alle Teile des Bosses.
     */
    private void Start()
    {
        StartCoroutine(SpawnLoop());
        _bossParts.Add(_frontPart);
        _frontPart.Init(_partCnt * _healthInc * 2f, false);
        _frontPart.OnBossPartDefeated += PartDefeated;
        
        for (var i = 0; i < _partCnt; ++i)
        {
            var spawnPos = _bossParts[^1].transform.position - 
                               _bossParts[^1].transform.forward * 2.0f;
            var newPart = Instantiate(_bossPart, spawnPos, Quaternion.identity);
            newPart.Init((_partCnt - i) * _healthInc, false);
            if (newPart.TryGetComponent(out FollowTarget target))
            {
                target.SetTarget(_bossParts[^1].gameObject.transform);
            }
            _bossParts.Add(newPart);
            newPart.OnBossPartDefeated += PartDefeated;
        }
        
        foreach (var lockObj in _locks)
        {
            lockObj.SetActive(false);
        }
        OnBossHealthChange?.Invoke(_bossParts.Count, _bossParts[^1]);
        ActivateNextLock();
    }
    
    /**
     * Instanziiert zufällig Viren auf der aktuellen WaypointGroup mit zufälligen Eigenschaften.
     * @returns Wert für Coroutine
     */
    private IEnumerator SpawnLoop()
    {
        while (_spawnIsRunning)
        {
            float waitTime = Random.Range(_minInterval, _maxInterval);
            yield return new WaitForSeconds(waitTime);

            if ((_spawnedViruses.Count <= _maxVirusCount) && _playerNearby && _spawnIsRunning)
            {
                int waypointIdx = Random.Range(0, _waypointsGrp[_currWaypointIdx].waypoints.Count);
                var virus = Instantiate(_virus, 
                    _waypointsGrp[_currWaypointIdx].waypoints[waypointIdx].position + 
                    Vector3.up * Random.Range(0.5f, 1.5f), Quaternion.identity
                );
                var scale = Random.Range(_minScale, _maxScale);
                virus.transform.localScale = new Vector3(scale, scale, scale);
                virus.Init(_virusHealth, _virusDmg);
                if (virus.TryGetComponent(out WaypointsTraveler trav))
                {
                    trav.ResetWaypointList(_waypointsGrp[_currWaypointIdx]);
                    trav.MoveSpeed = Random.Range(1, _maxMoveSpeed);
                    var values = Enum.GetValues(typeof(TravelDirection));
                    trav.StartTravelDirection = (TravelDirection) values.GetValue(Random.Range(0, values.Length));
                }
                
                OnVirusSpawn?.Invoke();
                _spawnedViruses.Add(virus);
            }
        }
    }
    
    /**
     * Überprüft ob der Spieler in der Nähe ist und ruft das Event auf, wenn sich der Status geändert hat.
     */
    private void Update()
    {
        if (_frontPart.IsUnityNull()) 
            return;
        
        var newPlayerNearby = Vector3.Distance(_playerTransf.position, _frontPart.transform.position) <= _playerDetectRadius;
        if (newPlayerNearby != _playerNearby)
        {
            _playerNearby = newPlayerNearby;
            OnPlayerNearby?.Invoke(_playerNearby);
        }
        _spawnedViruses.RemoveAll(virus => virus.IsUnityNull());
    }
    

    /**
     * Alle Events abbestellen.
     */
    private void OnDisable()
    {
        BossPart.OnAnyBossPartDefeated -= BossPartShot;
        BossPart.OnBossPartShot -= BossPartShot;
        PlayerHurtbox.OnLockCollect -= LockCollected;
        foreach (var part in _bossParts)
        {
            part.OnBossPartDefeated -= PartDefeated;
        }
    }

    /**
     * Wird ein Bossteil besiegt, so wird das nächste Schloss aktiviert und der Boss stärker.
     */
    private void PartDefeated(BossPart part)
    {
        _bossParts.Remove(part);
        part.OnBossPartDefeated -= PartDefeated;
        var frontPos = _frontPart.transform.position;
        Destroy(part.gameObject);

        if (_bossParts.Count <= 0)
        {
            _spawnIsRunning = false;
            BossPartShot();
            OnBossDefeated?.Invoke();
            Instantiate(_potionPrefab, frontPos + Vector3.up * 2f, Quaternion.identity);
            return;
        }
        
        Destroy(part.gameObject);
        _frontPartWaypointsTraveler.ResetWaypointList(GetNextWaypoint());
        _frontPartWaypointsTraveler.MoveSpeed *= _speedIncFactor;
        _frontPartWaypointsTraveler.LookAtSpeed *= _speedIncFactor;
        ActivateNextLock();
    }
    
    /**
     * Aktiviert das nächste Schloss.
     */
    private void ActivateNextLock() => _locks[_currLockIdx++].SetActive(true);

    /**
     * Gibt die nächste WaypointGroup anhand des gewählten Modus zurück.
     * @param die nächste WaypointGroup anhand des gewählten Modus
     */
    private WaypointsGroup GetNextWaypoint()
    {
        if (_loopWaypoints)
        {
            _currWaypointIdx = (_currWaypointIdx + 1) % _waypointsGrp.Length;
        }
        else if (_currWaypointIdx < _waypointsGrp.Length - 1)
        {
            _currWaypointIdx++;
        }

        return _waypointsGrp[_currWaypointIdx];
    }
    
    /**
     * Zeichnet den Radius der Spieleierkennung ausgehend vom ersten Bossteil.
     */
    private void OnDrawGizmosSelected()
    {
        if (_frontPart == null)
            return;
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_frontPart.transform.position, _playerDetectRadius);
    }
    
}
