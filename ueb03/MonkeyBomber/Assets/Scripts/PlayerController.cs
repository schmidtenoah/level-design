using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    private const float DEFAULT_SPEED_MODIFIER = 1f;
    private static float _speedModifier = 1f;
    
    [SerializeField] private float _speed = 10;
    [SerializeField] private float _rotSpeed = 10;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private float _playerHeight = 2.0f;
    [SerializeField] private float _playerRadius = 0.5f;
    
    private Vector2 _movement;
    private bool _inputLocked = false;

    public static event Action<Vector3> OnPlaceBombs;
    public static event Action<bool> OnSwitchBombType;

    /**
     * Setzt den Speed Modifier.
     * @param val der neue Speed Modifier
     */
    public static void SetSpeedModifier(float val) => _speedModifier = val;
    
    /**
     * Subscribed alle Listener zu beginn.
     */
    private void Start()
    {
        BlockElevator.OnElevatorStart += LockInput;
        BlockElevator.OnElevatorEnd += UnlockInput;
        Bomb.OnBombHitPlayer += LockInput;
        _speedModifier = DEFAULT_SPEED_MODIFIER;
    }

    /**
     * Unsubscribed alle Listener, wenn das Skript disabled wird.
     */
    private void OnDisable()
    {
        BlockElevator.OnElevatorStart -= LockInput;
        BlockElevator.OnElevatorEnd -= UnlockInput;
        Bomb.OnBombHitPlayer -= LockInput;
    }

    /**
     * Verhindert, dass sich der Spieler ab sofort bewegen kann.
     */
    private void LockInput() => _inputLocked = true;
    
    /**
    * Der Spieler kann sich ab sofort wieder Bewegen.
    */
    private void UnlockInput() => _inputLocked = false;

    /**
     * Methode für PlayerInput OnMove.
     * @param val Inhalt der Aktion
     */
    private void OnMove(InputValue val) => _movement = val.Get<Vector2>();

    /**
     * Updatet in jedem Frame die Spielerbewegung.
     */
    private void Update() => MovePlayer();
    
    /**
     * Überprüft, ob der Spieler sich bewegen kann (Block unter ihm und kein Block vor ihm).
     * @param moveDir die Richtung in der sich der Spieler Bewegen wird
     * @returns ob der Spieler sich bewegen darf/kann
     */
    private bool CanMove(Vector3 moveDir)
    {
        Vector3 currentPos = transform.position;
        Vector3 nextPos = currentPos + moveDir * (_speed * _speedModifier * Time.deltaTime);
        
        Vector3 bottom = currentPos - Vector3.up * (_playerHeight * 0.3f - _playerRadius);
        Vector3 top    = currentPos + Vector3.up * (_playerHeight * 0.3f - _playerRadius);
        bool frontFree = !Physics.CapsuleCast(bottom, top, _playerRadius, moveDir,
            _speed * _speedModifier * Time.deltaTime, _groundMask);

        bool walkableBlockUnderneath = false;
        if (Physics.Raycast(nextPos + Vector3.up * 0.1f, Vector3.down, out RaycastHit hit, 2f, _groundMask))
        {
            if (hit.collider.TryGetComponent(out Block block))
            {
                walkableBlockUnderneath = block.IsWalkable;
            }
        }

        return frontFree && walkableBlockUnderneath;
    }

    /**
     * Bewegt den Spieler anhand des Input-Action-Inputs.
     */
    private void MovePlayer()
    {
        if (_movement == Vector2.zero || _inputLocked) return;
        
        Vector3 movement = new Vector3(_movement.x, 0f, _movement.y).normalized;
        if (!CanMove(movement))
        {
            Vector3 moveDirX = new Vector3(movement.x, 0, 0).normalized;
            if (CanMove(moveDirX))
            {
                movement = moveDirX;
            }
            else
            {
                Vector3 moveDirZ = new Vector3(0, 0, movement.z).normalized;
                movement = CanMove(moveDirZ) ? moveDirZ : Vector3.zero;
            }
        }
        
        transform.position += movement * (_speed * _speedModifier * Time.deltaTime);
        transform.forward = Vector3.Slerp(transform.forward, movement, _rotSpeed * Time.deltaTime);
    }

    /**
    * Versucht, eine Bombe unter dem Spieler zu platzieren.
    * Führt eine Raycast-Abfrage nach unten durch, prüft, ob der Block Bomben erlaubt,
    * und ruft bei Erfolg das Platzierungs-Event mit der genauen Position auf.
    */
    private void OnPlaceBomb()
    {
        if (_inputLocked) 
            return;

        if (!Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2f, _groundMask))
            return;
        
        if (hit.collider.TryGetComponent(out Block block) && !block.BombPlaceable)
            return;

        Vector3 centerTop = hit.collider.bounds.center + Vector3.up * hit.collider.bounds.extents.y;
        OnPlaceBombs?.Invoke(centerTop);
    }

    /**
    * Wechselt den Bombentyp eine Position nach links (rückwärts im Enum).
    * Löst entsprechendes Event mit false aus.
    */
    private void OnSwitchBombTypeLeft() => OnSwitchBombType?.Invoke(false);
    
    /**
    * Wechselt den Bombentyp eine Position nach rechts (vorwärts im Enum).
    * Löst entsprechendes Event mit true aus.
    */
    private void OnSwitchBombTypeRight() => OnSwitchBombType?.Invoke(true);

    /**
     * Zeigt ein Gizmo der den Raycast wiederspiegelt.
     */
    private void OnDrawGizmosSelected()
    {
        Vector3 movement = new Vector3(_movement.x, 0f, _movement.y).normalized;
        Vector3 nextPos = transform.position + movement * (_speed * _speedModifier * Time.deltaTime);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(nextPos + Vector3.up * 0.1f, nextPos + Vector3.up * 0.1f + Vector3.down * 2f);
    }
    
}
