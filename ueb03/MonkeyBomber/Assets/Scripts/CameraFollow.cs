using Unity.VisualScripting;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    [SerializeField] private Transform _target;
    [SerializeField] private float _smoothTime = 0.3f;
    [SerializeField] private Vector3 _offset;
    private Vector3 _velocity = Vector3.zero;
    
    /**
     * Folgt dem Target mit gewissem smoothing.
     */
    private void Update()
    {
        if (_target.IsUnityNull()) return;

        Vector3 targetPos = _target.position + _offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref _velocity, _smoothTime);
    }
    
}
