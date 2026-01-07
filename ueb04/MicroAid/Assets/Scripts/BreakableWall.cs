using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BreakableWall : Target
{

    public static event Action OnWallBreak;
    [SerializeField] private GameObject _destroyParticles;
    [SerializeField] private ParticleSystem _hitParticles;
    [SerializeField] private DecalProjector _projector;

    /**
     * Defaultmaterial setzen.
     */
    private void Start() => ChangeMat();
    
    public override void Got(WeaponController.WeaponType weapon, float hitPoints)
    {
        if (!weapon.Equals(WeaponController.WeaponType.WEAPON_DAMAGE))
            return;

        _currHealth -= hitPoints;
        Instantiate(_hitParticles, transform.position + Vector3.up, Quaternion.identity);
        ChangeMat();

        if (_currHealth <= 0)
        {
            OnWallBreak?.Invoke();
            Instantiate(_destroyParticles, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    /**
     * Visualisiert, dass das Target Schaden bekommen hat.
     */
    private void ChangeMat() => _projector.fadeFactor = Mathf.Lerp(1f, 0f, _currHealth / _maxHealth);

}
