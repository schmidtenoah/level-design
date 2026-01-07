using Unity.VisualScripting;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    
    [SerializeField] private Transform _target;
    [SerializeField] private float _followDistance = 1.5f;
    [SerializeField] private float _followSpeed = 10f;
    [SerializeField] private float _rotationSpeed = 5f;

    /**
     * Verfolgt die Position des ausgewählten Targets.
     */
    private void Update()
    {
        if (_target.IsUnityNull())
            return;

        Vector3 offsetDir = (transform.position - _target.position).normalized;
        Vector3 targetPosition = _target.position + offsetDir * _followDistance;
        
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * _followSpeed);
        
        Vector3 moveDir = targetPosition - transform.position;
        if (moveDir.sqrMagnitude > 0.001f)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * _rotationSpeed);
        }
    }

    /**
     * Setzt das Target, welches verfolgt werden soll.
     * @param newTarget das (neue) Target, welches verfolgt werden soll
     */
    public void SetTarget(Transform newTarget) => _target = newTarget;
    
}
