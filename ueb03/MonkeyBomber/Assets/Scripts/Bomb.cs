using System;
using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    
    public enum BombType
    {
        BOMB_WATER,
        BOMB_FIRE,
        BOMB_ICE
    }
    
    private static readonly Vector3[] _dirsHorizontal = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };
    private static readonly Vector3[] _dirsVertical = { Vector3.up, Vector3.down };
    
    [SerializeField] private ParticlesSO _particles;
    [SerializeField] private float _fuseTime = 2f;
    [SerializeField] private int _range = 3;
    [SerializeField] private float _blockSize = 3f;
    [SerializeField] private float _stepDelay = 0.08f;
    
    private BombType _bombType;
    private bool _useHorizontalDirs = true;
    private readonly Collider[] _hitColliders = new Collider[10];
    
    public static event Action OnBombHitPlayer;
    public static event Action OnBombExploded;

    /**
     * Eine Art Konstruktor um itemabhängige Bomben zu erstellen.
     * @param range Anzahl an Blöcke in die gewählten Richtungen
     * @param bombType Art der Bombe
     * @param horizontalDirs ob horizontal oder vertikal explodiert werden soll
     */
    public void Init(int range, BombType bombType, bool horizontalDirs)
    {
        _range = range;
        _bombType = bombType;
        _useHorizontalDirs = horizontalDirs;
    }

    /**
     * Sobald die Bombe existiert geht der Timer herunter und danach explodiert sie.
     */
    private void Start()
    {
        Instantiate(_particles._ignition, transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity);
        Invoke(nameof(Explode), _fuseTime);
    }

    /**
     * Diese Bomb explodiert, dabei wird das Mesh versteckt und das Objekt, nachdem alles explodiert, ist gelöscht.
     * Die Explosionen gehen anhand von _useHorizontalDirs in die Richtungen und im Zentrum.
     */
    private void Explode()
    {
        foreach (var r in GetComponentsInChildren<Renderer>())
        {
            r.enabled = false;
        }
        
        OnBombExploded?.Invoke();
        Instantiate(_particles._explosions[(int) _bombType], transform.position, Quaternion.identity);

        if (CheckHitAtPos(transform.position + new Vector3(0, 1.5f, 0))) return;
        
        foreach (Vector3 dir in (_useHorizontalDirs ? _dirsHorizontal : _dirsVertical))
        {
            StartCoroutine(Propagate(dir));
        }
        
        float life = _range * _stepDelay + 0.5f;
        Destroy(gameObject, life);
    }
    
    /**
     * Coroutine die in die übergebene Richtung eine Explosion erzeugt.
     * @param dir Richtung in der von dieser Position eine Explosion erzeugt werden soll
     * @returns Wert für die Coroutine
     */
    private IEnumerator Propagate(Vector3 dir)
    {
        Vector3 origin = transform.position + new Vector3(0, 1.5f, 0);
        for (int step = 1; step <= _range; step++)
        {
            yield return new WaitForSeconds(_stepDelay);
            
            Vector3 cellPos = origin + dir * (_blockSize * step);
            if (CheckHitAtPos(cellPos)) break;
            Instantiate(_particles._spreadExplosions[(int) _bombType], cellPos, Quaternion.identity);
        }
    }

    /**
     * Überprüft an einer Position, ob die Explosion etwas trifft und nimmt entsprechende Maßnahmen ein.
     * @param pos die Position an der eine Teil-Explosion stattfinden soll
     */
    private bool CheckHitAtPos(Vector3 pos)
    {
        var hitCount = Physics.OverlapSphereNonAlloc(pos, _blockSize * 0.4f, _hitColliders);
        bool hitSomething = false;
        
        for (var i = 0; i < hitCount; i++)
        {
            var hit = _hitColliders[i];
            if (hit.CompareTag("Player"))
            {
                OnBombHitPlayer?.Invoke();
            }
            else if (hit.TryGetComponent(out Block block))
            {
                block.BombHit(_bombType);
                hitSomething = true;
            }
        }

        return hitSomething;
    }
    
}
