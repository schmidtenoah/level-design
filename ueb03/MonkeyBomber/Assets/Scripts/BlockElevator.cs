using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))] 
public class BlockElevator : MonoBehaviour
{
    
    [Tooltip("Toggle to go up or down the first time")]
    [SerializeField] private bool _goUp = true;

    [Tooltip("Ausgehend von der y-Größe des Colliders")]
    [SerializeField] private int _travelBlocks = 1;

    [SerializeField] private float _travelDuration = 2.0f;
    private bool _isMoving = false;

    public static event Action OnElevatorStart;
    public static event Action OnElevatorEnd;
    
    /**
     * Wird der Fahrstuhl vom Spieler betreten, startet eine Animation die den Parent (Block)
     * je nachdem wo er gerade war, nach oben oder unten bewegt.
     * @param other der Collider des anderen Objektes
     */
    private void OnTriggerEnter(Collider other)
    {
        if (_isMoving || !other.CompareTag("Player")) return;

        Transform block = transform.parent;
        float blockHeight = block.GetComponent<Collider>().bounds.size.y;
        
        float distance = (_goUp ? 1f : -1f) * _travelBlocks * blockHeight;
        Vector3 startPos = block.position;
        Vector3 endPos   = startPos + Vector3.up * distance;
        
        StartCoroutine(MoveBlockWithPlayer(other.transform, block, startPos, endPos));
        _goUp = !_goUp;
    }
    
    /**
     * Coroutine, die die Animation und Bewegung des Blockes und Spielers übernimmt.
     * @param player der Spieler der mitbewegt wird
     * @param block der Block der bewegt werden soll
     * @param from Startposition
     * @param to Endposition
     */
    private IEnumerator MoveBlockWithPlayer(Transform player, Transform block, Vector3 from, Vector3 to)
    {
        // passt t an, damit Lerp cooler aussieht
        float EaseInOutQuad(float x) => (x < 0.5f) ? 2f * x * x : (1f - Mathf.Pow(-2f * x + 2f, 2f) / 2f);
        
        OnElevatorStart?.Invoke();
        _isMoving = true;
        float t = 0f;
        player.SetParent(block, true);

        while (t < 1f)
        {
            t += Time.deltaTime / _travelDuration;
            block.position = Vector3.Lerp(from, to, EaseInOutQuad(t));
            yield return null;
        }
        
        OnElevatorEnd?.Invoke();
        block.position = to;
        player.SetParent(block, false);
        _isMoving = false;
    }
    
    /**
    * Zeichnet im Editor ein magentafarbenes Gizmo des Zielortes, 
    * zu dem sich der Fahrstuhl bewegen kann.
    */
    private void OnDrawGizmos()
    {
        Transform block = transform.parent;
        Collider col = block.GetComponent<Collider>();
        Gizmos.color = Color.magenta;
        Vector3 size   = col.bounds.size;
        Vector3 center = col.bounds.center;

        float distance = (_goUp ? 1f : -1f) * _travelBlocks * size.y;
        center += Vector3.up * distance;
        Gizmos.DrawWireCube(center, size);
    }
    
}
