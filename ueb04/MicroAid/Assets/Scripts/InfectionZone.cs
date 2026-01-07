using System;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Plane))]
public class InfectionZone : MonoBehaviour
{
    
    public static event Action<float> OnPlayerEnterInfectionZone;
    public static event Action OnPlayerExitInfectionZone;
    
    [FormerlySerializedAs("infectionDamage")] 
    [SerializeField] private float _infectionDamage = 0.01f;
    private bool _isInside;
    private Vector3 _lastPlayerPosition;

    /**
     * Betritt der Spieler die Zone, wird seine Position abgespeichert, um zu berechnen,
     * ob er herein- oder herausgehen wird.
     * @param other das andere Kollisionsobjekt
     */
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) 
            return;

        _lastPlayerPosition = other.transform.position;
    }
    
    /**
     * Innerhalb der Zone wird konstant die neue Spielerposition abgespeichert und berechnet in welche Richtung er
     * sich bewegt i.e. ob er herein- oder herausgehen wird.
     * @param other das andere Kollisionsobjekt
     */
    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) 
            return;

        Vector3 currentPos = other.transform.position;
        Vector3 movementDir = (currentPos - _lastPlayerPosition).normalized;
        _lastPlayerPosition = currentPos;
        float dot = Vector3.Dot(transform.forward, movementDir);

        _isInside = dot < 0;
    }

    /**
     * Geht der Spieler aus der Zone heraus, so wird das passende Event aufgerufen,
     * je nachdem in welche Richtung er gegangen ist.
     * @param other das andere Kollisionsobjekt
     */
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) 
            return;
        
        
        if (_isInside)
        {
            OnPlayerEnterInfectionZone?.Invoke(_infectionDamage);
        }
        else
        {
            OnPlayerExitInfectionZone?.Invoke();
        }
    }
    
    /**
     * Zeichnet ein Pfeil in die Richtung, in dem sich der infizierte Bereich befindet.
     */
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Vector3 start = transform.position;
        Vector3 direction = -transform.forward;
        float arrowLength = 2f;

        Gizmos.DrawRay(start, direction * arrowLength);
        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 150, 0) * Vector3.forward;
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -150, 0) * Vector3.forward;
        Gizmos.DrawRay(start + direction * arrowLength, right * 0.5f);
        Gizmos.DrawRay(start + direction * arrowLength, left * 0.5f);
    }
    
}
