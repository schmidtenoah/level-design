using StarterAssets;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerAnimator : MonoBehaviour
{
    private const string IS_WALKING = "IsWalking";
    private const string IS_JUMPING = "IsJumping";
    private const string IS_SPRINTING = "IsSprinting";
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
     * Updated jeden Frame die Bewegungs-/Sprint-Animation.
     */
    private void Update()
    {
        _animator.SetBool(IS_WALKING, _personController.isMoving);
        _animator.SetBool(IS_SPRINTING, _personController.isSprinting);
    }

    /**
     * Führt die Animation für einen Sprung aus.
     */
    public static void TriggerJumpingAnim()
    {
        _animator.SetTrigger(IS_JUMPING);
    }
    
}
