using StarterAssets;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BugnetAnimator : MonoBehaviour
{
    private const string IS_WALKING = "IsWalking";
    private const string IS_CAPTURING_SIDE = "IsCapturingSide";
    private const string IS_CAPTURING_FRONT = "IsCapturingFront";
    private static Animator _animator;

    [SerializeField] private ThirdPersonController _personController;

    /**
     * Wird aufgerufen, wenn eine Skriptinstanz geladen wird.
     * Ordnet die Referenzen den Komponenten zu.
     */
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    /**
     * Updated jeden Frame die Bewegungsanimation.
     */
    private void Update()
    {
        _animator.SetBool(IS_WALKING, _personController.isMoving);
    }

    /**
     * Führt die Animation für eine Einfangen-Animation von Vorne aus.
     */
    public static void TriggerCapturingAnimFront()
    {
        _animator.SetTrigger(IS_CAPTURING_FRONT);
    }
    
    /**
     * Führt die Animation für eine Einfangen-Animation von der Seite aus.
     */
    public static void TriggerCapturingAnimSide()
    {
        _animator.SetTrigger(IS_CAPTURING_SIDE);
    }
    
}
