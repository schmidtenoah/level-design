using UnityEngine;

public class SFXManager : MonoBehaviour
{
    
    [SerializeField] private AudioClipRefsSO _audioClips;
    private static AudioSource _src;
    
    /**
    * Fügt dem GameObject eine AudioSource-Komponente hinzu und
     * abonniert alle Events die einen Soundeffekt abspielen sollen.
    */
    private void Awake()
    {
        _src = gameObject.AddComponent<AudioSource>();
        _src.spatialBlend = 0f;

        PlayerHurtbox.OnVirusHitPlayer += InsideBeamSFX;
        WeaponController.OnWeaponShot += ShotWeaponSFX;
        BreakableWall.OnWallBreak += WallBreakSFX;
        PlayerHurtbox.OnBloodCellCollect += ItemCollectSFX;
        PlayerHurtbox.OnPillCollect += ItemCollectSFX;
        PlayerHealthManager.OnPlayerDead += LostSFX;
        Heart.OnHeartHealed += WonSFX;
        WeaponController.OnWeaponChange += WeaponChangeSFX;
        Virus.OnVirusHealed += VirusHealSFX;
        Virus.OnVirusShot += VirusShotSFX;
        Boss.OnVirusSpawn += VirusSpawnSFX;
        BossPart.OnAnyBossPartDefeated += DefeatedBossPartSFX;
        PlayerHurtbox.OnLockCollect += LockSFX;
    }

    /**
     * Events abbestellen.
     */
    private void OnDisable()
    {
        PlayerHurtbox.OnVirusHitPlayer -= InsideBeamSFX;
        WeaponController.OnWeaponShot -= ShotWeaponSFX;
        BreakableWall.OnWallBreak -= WallBreakSFX;
        PlayerHurtbox.OnBloodCellCollect -= ItemCollectSFX;
        PlayerHurtbox.OnPillCollect -= ItemCollectSFX;
        PlayerHealthManager.OnPlayerDead -= LostSFX;
        Heart.OnHeartHealed -= WonSFX;
        WeaponController.OnWeaponChange -= WeaponChangeSFX;
        Virus.OnVirusHealed -= VirusHealSFX;
        Virus.OnVirusShot -= VirusShotSFX;
        Boss.OnVirusSpawn -= VirusSpawnSFX;
        BossPart.OnAnyBossPartDefeated -= DefeatedBossPartSFX;
        PlayerHurtbox.OnLockCollect -= LockSFX;
    }

    /** Spielt passenden Soundeffekt ab. */
    private void InsideBeamSFX(Virus virus) => PlaySound(_audioClips._insideBeam);

    /** Spielt passenden Soundeffekt ab. */
    private void WallBreakSFX() => PlaySound(_audioClips._wallBreak);

    /** Spielt passenden Soundeffekt ab. */
    private void ItemCollectSFX(int cnt) => PlaySound(_audioClips._itemCollected);

    /** Spielt passenden Soundeffekt ab. */
    private void DefeatedBossPartSFX() => PlaySound(_audioClips._bossPartDefeat);

    /** Spielt passenden Soundeffekt ab. */
    private void LostSFX() => PlaySound(_audioClips._lost);
    
    /** Spielt passenden Soundeffekt ab. */
    private void LockSFX() => PlaySound(_audioClips._lockCollected);
    
    /** Spielt passenden Soundeffekt ab. */
    private void WonSFX() => PlaySound(_audioClips._won);
    
    /** Spielt passenden Soundeffekt ab. */
    private void VirusShotSFX() => PlaySound(_audioClips._virusShot);
    
    /** Spielt passenden Soundeffekt ab. */
    private void VirusHealSFX() => PlaySound(_audioClips._virusHealed);
    
    /** Spielt passenden Soundeffekt ab. */
    private void WeaponChangeSFX(WeaponController.WeaponType type) => PlaySound(_audioClips._weaponChange);
    
    /** Spielt passenden Soundeffekt ab. */
    private void VirusSpawnSFX() => PlaySound(_audioClips._virusSpwan);

    /** Spielt passenden Soundeffekt ab. */
    private void ShotWeaponSFX(WeaponController.WeaponType type)
    {
        switch (type)
        {
            case WeaponController.WeaponType.WEAPON_HEAL:
                PlaySound(_audioClips._healShot);
                break;
            case WeaponController.WeaponType.WEAPON_DAMAGE:
                PlaySound(_audioClips._damageShot);
                break;
            case WeaponController.WeaponType.WEAPON_NONE:
            default:
                break;
        }
    }

    /**
    * Spielt den übergebenen Audio-Clip an Position und mit Lautstärke ab.
    * @param audioClip die Audio die abgespielt werden soll
    * @param volume die Lautstärke des Sounds
    */
    private static void PlaySound(AudioClip clip, float volume = 0.7f) => _src.PlayOneShot(clip, Mathf.Clamp01(volume));
    
}
