using System;
using UnityEngine;

public class CraftInteractible : MonoBehaviour
{
    
    [SerializeField] private RecipeSO _recipe;
    
    public static event Action<RecipeSO> OnCraftInteraction;
    
    /**
     * Ruft das Interaktionsevent auf.
     */
    public void Interact()
    {
        OnCraftInteraction?.Invoke(_recipe);
    }
    
}
