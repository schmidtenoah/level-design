using UnityEngine;

public class LightMovement : MonoBehaviour
{
    
    [SerializeField] Vector2 _cycleDurationXZ = new Vector2(20f, 20f);
    [SerializeField] AnimationCurve _movementPathX;
    [SerializeField] AnimationCurve _movementPathZ;
    [SerializeField] Vector2 _movementMagnitudeXZ = new Vector2(1, 1);
    [SerializeField] Vector2 _movementTimeOffsetXZ;

    private Vector3 _initialPosition;
    
    /**
    *  Speichert die Startposition des Objekts, um spätere Bewegungen relativ zu dieser zu berechnen.
    */
    private void Awake()
    {
        _initialPosition = transform.position;
    }

    /**
     * Aktualisiert die Position des Objekts in X- und Z-Richtung basierend auf zwei Animationskurven.
    * Die Bewegung verläuft zyklisch und wird durch Magnitude und Zeit-Offsets angepasst.
    */
    private void Update()
    {
        float timeX = Time.time % _cycleDurationXZ.x;
        timeX /= _cycleDurationXZ.x;
        float timeZ = Time.time % _cycleDurationXZ.y;
        timeZ /= _cycleDurationXZ.y;
        
        float newx = _movementPathX.Evaluate(timeX + _movementTimeOffsetXZ.x) * _movementMagnitudeXZ.x;
        float newZ = _movementPathZ.Evaluate(timeZ + _movementTimeOffsetXZ.y) * _movementMagnitudeXZ.y;
        transform.position = _initialPosition + new Vector3(newx, 0, newZ);
    }
    
}
