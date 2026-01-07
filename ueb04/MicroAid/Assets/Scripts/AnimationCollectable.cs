using UnityEngine;

public class AnimationCollectable : MonoBehaviour
{
    
    [SerializeField] private float _rotSpeed = 50f;
    [SerializeField] private float _wiggleSpeed = 4f;
    [SerializeField] private float _wiggleHeight = 0.3f;
    [SerializeField] private float _posOffsetY = 0.4f;
    private Vector3 _startPos;
    
    /**
     * Speichert die Startposition ab, damit der wiggle einen Anker hat.
     */
    private void Start()
    {
        _startPos = transform.position + new Vector3(0, _posOffsetY, 0);
    }
    
    /**
     * Animiert das Objekt, indem es sich zeitabhängig dreht und sch nach oben und unten bewegt.
     */
    private void Update()
    {
        transform.Rotate(Vector3.up * (_rotSpeed * Time.deltaTime));
        
        float height = Mathf.Sin(Time.time * _wiggleSpeed) * _wiggleHeight;
        transform.position = _startPos + new Vector3(0, height, 0);
    }
    
}
