using System.Collections;
using UnityEngine;

public class CamShake : MonoBehaviour
{
    private Vector3 _originalPosition;
    private Coroutine _shakeCoroutine;

    /**
     * Events bestellen.
     */
    void Awake()
    {
        _originalPosition = transform.localPosition;
        PlayerHurtbox.OnVirusHitPlayer += VirusHitShake;
        WeaponController.OnWeaponShot += WeaponShotShake;
        BreakableWall.OnWallBreak += WallBreakShake;
        BossPart.OnAnyBossPartDefeated += BossPartShake;
    }

    /**
     * Events abbestellen.
     */
    private void OnDisable()
    {
        PlayerHurtbox.OnVirusHitPlayer -= VirusHitShake;
        WeaponController.OnWeaponShot -= WeaponShotShake;
        BreakableWall.OnWallBreak -= WallBreakShake;
        BossPart.OnAnyBossPartDefeated -= BossPartShake;
    }

    /** Shake bei einem Virustreffer. @param virus der Virus */
    private void VirusHitShake(Virus virus) => Shake(0.2f, 0.03f);

    /** Shake bei einem Bossteiltreffer. */
    private void BossPartShake() => Shake(0.2f, 0.03f);
    
    /** Shake beim Schuss einer Waffe. @param type der Waffentyp */
    private void WeaponShotShake(WeaponController.WeaponType type)
    {
        if (type.Equals(WeaponController.WeaponType.WEAPON_HEAL))
        {
            Shake(0.2f, 0.02f);
        }
        else
        {
            Shake(0.2f, 0.01f);
        }
    }

    /** Shake beim Zerstören einer Mauer. */
    private void WallBreakShake() => Shake(1f, 0.06f);

    /**
     * Startet ein Schütteln der Kamera.
     * @param time wie lange das Schütteln andauern soll
     * @param strength die Stärke des Schüttelns
     */
    private void Shake(float time, float strength)
    {
        if (_shakeCoroutine != null)
        {
            StopCoroutine(_shakeCoroutine);
        }
        _shakeCoroutine = StartCoroutine(DoShake(time, strength));
    }

    /**
     * Führt das Schütteln der Kamera aus.
     * @param duration wie lange das Schütteln andauern soll
     * @param strength die Stärke des Schüttelns
     */
    private IEnumerator DoShake(float duration, float strength)
    {
        float remainingTime = duration;
        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            Vector3 randomDir = Random.insideUnitSphere;
            transform.localPosition = _originalPosition + randomDir * strength;
            yield return null;
        }
        transform.localPosition = _originalPosition;
        _shakeCoroutine = null;
    }

}
