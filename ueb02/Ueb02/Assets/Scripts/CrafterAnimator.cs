using UnityEngine;

public class CrafterAnimator : MonoBehaviour
{
    
    private const string IS_WAVING = "IsWaving";
    private static Animator _animator;
    
    /**
     * Holt sich Referenz auf den Animator.
     */
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    
    /**
     * Wenn der Spieler interagieren kann, soll der Crafter die Interaktions-Animation abspielen.
     */
    private void Update()
    {
        _animator.SetBool(IS_WAVING, PlayerInteract._canInteract);
    }
    
}
