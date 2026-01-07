using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInteract : MonoBehaviour
{
    
    [SerializeField] private float _interactionRange = 2.0f;
    [SerializeField] private GameObject _containerUI;
    public static bool _canInteract { get; private set; }
    private readonly Collider[] _hitColliders = new Collider[10];

    /**
     * Versteckt die InteraktionsUI.
     */
    private void Start()
    {
        _containerUI.SetActive(false);
    }

    /**
     * PlayerInput Methode die bei Input des Spielers überprüft, ob Objekte in der Nähe sind die
     * Interagierbar sind und interagiert mit ihnen, wenn ja.
     * @param val wird nicht benötigt
     */
    private void OnInteract(InputValue val)
    {
        var hitCount = Physics.OverlapSphereNonAlloc(transform.position, _interactionRange, _hitColliders);
        for (var i = 0; i < hitCount; i++)
        {
            if (_hitColliders[i].TryGetComponent(out CraftInteractible interactible))
            {
                interactible.Interact();
            }
        }
    }
    
    /**
     * Damit ein Interaktionsbutton auf der UI erscheinen kann, wird im Umkreis des Spielers nach Objekten gesucht,
     * mit denen er Interagieren kann, woraufhin die UI erscheint.
     */
    private void Update()
    {
        bool isInteracting = false;
        var hitCount = Physics.OverlapSphereNonAlloc(transform.position, _interactionRange, _hitColliders);
        for (var i = 0; i < hitCount && !isInteracting; i++)
        {
            if (_hitColliders[i].TryGetComponent(out CraftInteractible interactible))
            {
                isInteracting = true;
            }
        }

        _canInteract = isInteracting;
        _containerUI.SetActive(_canInteract);
    }
    
}
