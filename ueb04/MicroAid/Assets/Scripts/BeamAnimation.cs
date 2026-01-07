using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class BeamAnimation : MonoBehaviour
{

    [SerializeField] private Vector2 _scrollSpeed = new (0f, 8.0f);
    private Renderer _renderer;
    private Vector2 _offset;

    /**
     * Holt sich die Referenz auf den Renderer, um die Textur zu verschieben.
     */
    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    /**
     * Verschiebt die Textur zeitabhängig in die definierte Richtung.
     */
    private void Update()
    {
        _offset += _scrollSpeed * Time.deltaTime;
        _renderer.material.SetTextureOffset("_MainTex", _offset);
    }
    
}
