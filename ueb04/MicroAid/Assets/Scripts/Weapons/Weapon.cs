using System.Collections.Generic;
using UnityEngine;


public class Weapon : MonoBehaviour
{
    
    [SerializeField] private WeaponController.WeaponType _weaponType;
    [SerializeField] private float _damage = 1.0f;
    [SerializeField] private float _maxShootDistance = 100.0f;
    [SerializeField] private float _fireRate = 0.5f;
    [SerializeField] private Camera _cam;
    [SerializeField] private float _spreadAngle = 1.0f;
    
    [SerializeField] private Transform _endPoint;
    private float _shotTimer;
    
    [SerializeField] private LineRenderer _prefabRayTrail;
    [SerializeField] private GameObject _impactParticles;
    
    private class ActiveTrail
    {
        public LineRenderer renderer;
        public Vector3 direction;
        public float remainingTime;
    }
    
    private readonly List<ActiveTrail> _activeTrails = new();
    private readonly Vector3[] _trailPositions = new Vector3[2];
    
    /**
     * Shottimer initialisieren.
     */
    private void Awake() => _shotTimer = -1.0f;
    
    /**
     * Handhabung der abgeschossenen Projektile.
     */
    private void Update()
    {
        if (_shotTimer > 0)
        {
            _shotTimer -= Time.deltaTime;
        }
        
        for (int i = 0; i < _activeTrails.Count; ++i)
        {
            var activeTrail = _activeTrails[i];
            
            activeTrail.renderer.GetPositions(_trailPositions);
            activeTrail.remainingTime -= Time.deltaTime;

            _trailPositions[0] += activeTrail.direction * (50.0f * Time.deltaTime);
            _trailPositions[1] += activeTrail.direction * (50.0f * Time.deltaTime);
            
            _activeTrails[i].renderer.SetPositions(_trailPositions);
            
            if (_activeTrails[i].remainingTime <= 0.0f)
            {
                _activeTrails[i].renderer.gameObject.SetActive(false);
                Destroy(_activeTrails[i].renderer.gameObject);
                _activeTrails.RemoveAt(i);
                --i;
            }
        }
    }
    
    /**
     * Schiesst ein Projektil anhand der Richtung und Position der Kamera.
     * @returns ob der Schuss möglich war
     */
    public bool Fire()
    {
        if (_shotTimer > 0) 
            return false;
        
        _shotTimer = _fireRate;
        float spreadRatio = _spreadAngle / _cam.fieldOfView;
        Vector2 spread = spreadRatio * Random.insideUnitCircle;
        
        Ray r = _cam.ViewportPointToRay(Vector3.one * 0.5f + (Vector3)spread);
        Vector3 hitPosition = r.origin + r.direction * _maxShootDistance;
    
        if (Physics.Raycast(r, out RaycastHit hit, _maxShootDistance, ~(1 << 9), QueryTriggerInteraction.Ignore))
        {
            if (hit.distance > 5.0f)
                hitPosition = hit.point;

            var particles = Instantiate(_impactParticles);
            particles.transform.position = hit.point;
            
            if (hit.collider.gameObject.TryGetComponent(out Target target))
            {
                target.Got(_weaponType, _damage);
            }
        }


        if (_prefabRayTrail != null)
        {
            var pos = new Vector3[] { _endPoint.transform.position, hitPosition };
            var trail = Instantiate(_prefabRayTrail);
            trail.gameObject.SetActive(true);
            trail.SetPositions(pos);
            _activeTrails.Add(new ActiveTrail()
            {
                remainingTime = 0.3f,
                direction = (pos[1] - pos[0]).normalized,
                renderer = trail
            });

        }

        return true;
    }
    
}
