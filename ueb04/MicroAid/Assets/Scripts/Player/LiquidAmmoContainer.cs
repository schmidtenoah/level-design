using UnityEngine;

/*
 * https://assetstore.unity.com/packages/templates/tutorials/unity-learn-creator-kit-fps-urp-149310
 */

public class LiquidAmmoContainer : MonoBehaviour
{
    
    [SerializeField] private float _maxWobble = 0.03f;
    [SerializeField] private float _wobbleSpeed = 1f;
    [SerializeField] private float _recovery = 1f;
    
    private Renderer _renderer;
    private Vector3 _previousPosition;
    private Vector3 _velocity;
    private Vector3 _lastRotation;  
    private Vector3 _angularVelocity;
    private float _wobbleAmountX;
    private float _wobbleAmountZ;
    private float _wobbleAmountToAddX;
    private float _wobbleAmountToAddZ;
    private float _pulse;
    private float _time = 0.5f;

    private Material _material;

    private int _liquidRotationId;
    private int _fillAmountId;
    
    /**
     * Initalisierung.
     */
    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _material = _renderer.material;

        _liquidRotationId = Shader.PropertyToID("_LiquidRotation");
        _fillAmountId = Shader.PropertyToID("_FillAmount");
    }

    /**
     * Verändert den Füllstand dieses Containers.
     * @param liquidAmount neuer Füllstand
     */
    public void ChangeLiquidAmount(float liquidAmount) => _material.SetFloat(_fillAmountId, liquidAmount);
    
    /**
     * Flüssigkeitsdarstellung.
     */
    private void Update()
    {
        _time += Time.deltaTime;
        // decrease wobble over time
        _wobbleAmountToAddX = Mathf.Lerp(_wobbleAmountToAddX, 0, Time.deltaTime * (_recovery));
        _wobbleAmountToAddZ = Mathf.Lerp(_wobbleAmountToAddZ, 0, Time.deltaTime * (_recovery));

        // make a sine wave of the decreasing wobble
        _pulse = 2 * Mathf.PI * _wobbleSpeed;
        _wobbleAmountX = _wobbleAmountToAddX * Mathf.Sin(_pulse * _time);
        _wobbleAmountZ = _wobbleAmountToAddZ * Mathf.Sin(_pulse * _time);
        
        Matrix4x4 rotation = Matrix4x4.Rotate( Quaternion.AngleAxis(_wobbleAmountZ, Vector3.right) * Quaternion.AngleAxis(_wobbleAmountX, Vector3.forward));

        // send it to the shader
        _material.SetMatrix(_liquidRotationId, rotation);
        
        // velocity
        _velocity = (_previousPosition - transform.position) / Time.deltaTime;
        _angularVelocity = transform.rotation.eulerAngles - _lastRotation;
        
        // add clamped velocity to wobble
        _wobbleAmountToAddX += Mathf.Clamp((_velocity.x + (_angularVelocity.z * 0.2f)) * _maxWobble, -_maxWobble, _maxWobble);
        _wobbleAmountToAddZ += Mathf.Clamp((_velocity.z + (_angularVelocity.x * 0.2f)) * _maxWobble, -_maxWobble, _maxWobble);

        // keep last position
        _previousPosition = transform.position;
        _lastRotation = transform.rotation.eulerAngles;
    }
    
}
