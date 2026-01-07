using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(PlayerInput))]
public class BugnetMovement : MonoBehaviour
{
    [SerializeField] private Collider _bugnetCollider;
    [SerializeField] private float _captureCooldown = 1f;
    public static event Action<GameObject> OnCaptureSidePressed;
    public static event Action<GameObject> OnCaptureFrontPressed;
    private bool _canCapture = true;

    /**
     * Wird vor Update aufgerufen, wenn ein Skript aktiviert wird.
     * Schaltet den Collider vom Netz aus.
     */
    private void Start()
    {
        _bugnetCollider.enabled = false;
    }
    
    /**
     * Methode die vom PlayerInput aufgerufen wird.
     * Führt einen Fang von Vorne aus, wenn dies gerade möglich ist.
     * @param val wird nicht benötigt 
     */
    private void OnCaptureFront(InputValue val)
    {
        if (!_canCapture) return;
        
        _canCapture = false;
        _bugnetCollider.enabled = true;
        OnCaptureFrontPressed?.Invoke(this.gameObject);
        BugnetAnimator.TriggerCapturingAnimFront();
        Invoke(nameof(ResetCaptureCollider), _captureCooldown);
    }
    
    /**
     * Methode die vom PlayerInput aufgerufen wird.
     * Führt einen Fang von der Seite aus, wenn dies gerade möglich ist.
     * @param val wird nicht benötigt
     */
    private void OnCaptureSide(InputValue val)
    {
        if (!_canCapture) return;
        
        _canCapture = false;
        _bugnetCollider.enabled = true;
        OnCaptureSidePressed?.Invoke(this.gameObject);
        BugnetAnimator.TriggerCapturingAnimSide();
        Invoke(nameof(ResetCaptureCollider), _captureCooldown);
    }
    
    /**
     * Schaltet den Netz-Collider aus und der Spieler kann wieder Fangen.
     */
    private void ResetCaptureCollider()
    {
        _bugnetCollider.enabled = false;
        _canCapture = true;
    }
    
}
