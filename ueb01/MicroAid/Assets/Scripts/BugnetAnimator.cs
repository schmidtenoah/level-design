using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BugnetAnimator : MonoBehaviour
{
    private const string IS_WALKING = "IsWalking";
    private const string IS_CAPTURING = "IsCapturing";
    private static Animator _animator;

    [SerializeField] private PlayerMovement plyr;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        _animator.SetBool(IS_WALKING, plyr.IsWalking);
    }

    public static void TriggerCapturingAnim()
    {
        _animator.SetTrigger(IS_CAPTURING);
    }
}
