using UnityEngine;

public class SoundManager : MonoBehaviour
{
    
    [SerializeField] private AudioClipRefsSO _audioClipRefsSO;
    
    /**
     * Abonniert Events um an passenden Stellen Sounds abzuspielen.
     */
    private void Start()
    {
        BugnetMovement.OnCaptureSidePressed += OnCaptureSound;
        BugnetMovement.OnCaptureFrontPressed += OnCaptureSound;
        PlayerHurtbox.OnHitPlayer += OnVirusHitPlayerSound;
        NetCapture.OnNetCapture += OnCaptureSuccessSound;
        PlayerHurtbox.OnCollectSuccessfull += OnCaptureSuccessSound;
    }

    /**
     * Spielt einen Sound beim Schwingen des Netzes.
     * @param senderGameObject das Object an dem der Sound abgespielt werden soll
     */
    private void OnCaptureSound(GameObject senderGameObject)
    {
       PlaySound(_audioClipRefsSO._capture, senderGameObject.transform.position, 0.4f);
    }

    /**
     * Spielt einen Sound beim Treffer von Viren.
     * @param senderGameObject das Object an dem der Sound abgespielt werden soll
     */
    private void OnVirusHitPlayerSound(GameObject senderGameObject)
    {
        PlaySound(_audioClipRefsSO._hitByVirus, senderGameObject.transform.position, 8f);
    }

    /**
     * Spielt einen Sound beim Winfangen von Viren.
     * @param senderGameObject das Object an dem der Sound abgespielt werden soll
     */
    private void OnCaptureSuccessSound(GameObject senderGameObject)
    {
        PlaySound(_audioClipRefsSO._captureVirus, senderGameObject.transform.position, 0.2f);
    }

    /**
     * Spielt den übergebenen Audio-Clip an Position und mit Lautstärke ab.
     * @param audioClip die Audio die abgespielt werden soll
     * @param pos die Position von dem der Sound kommen soll
     * @param volume die Lautstärke des Sounds
     */
    private static void PlaySound(AudioClip audioClip, Vector3 pos, float volume = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip, pos, volume);
    }
    
}
