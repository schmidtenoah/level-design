using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private readonly Dictionary<CollectableItemSO, int> _items = new();
    [SerializeField] private GameObject _craftUI;
    [SerializeField] private InventoryUI _inventoryUI;
    [SerializeField] private RecipeSO _winRecipe;
    
    private const string NOT_CRAFTABLE = "Not enough resources!";
    private const string VIRUSES_LEFT = "There are still Viruses alive!";
    private const float SHOW_MSG_TIME = 1.5f;
    
    public static event Action OnWinRecipeCrafted;

    /**
     * Versteckt das Crafting-UI.
     */
    private void HideCraftUI()
    {
        _craftUI.SetActive(false);
    }
    
    /**
     * Abonniert Events und versteckt die Crafting-UI.
     */
    private void OnEnable()
    {
        PlayerHurtbox.OnCollect += AddItem;
        CraftInteractible.OnCraftInteraction += Craft;
        HideCraftUI();
    }

    /**
     * Deabboniert alle Events.
     */
    private void OnDisable()
    {
        PlayerHurtbox.OnCollect -= AddItem;
        CraftInteractible.OnCraftInteraction -= Craft;
    }

    /**
     * Zeigt die übergebene Nachricht auf dem UI an.
     * @param msg die Nachricht die angezeigt werden soll
     */
    private void ShowMessage(string msg)
    {
        _craftUI.GetComponentInChildren<TextMeshProUGUI>().text = msg;
        _craftUI.SetActive(true);
        Invoke(nameof(HideCraftUI), SHOW_MSG_TIME);
    }
    
    /**
     * Fügt dem Inventar das übergebene Item hinzu.
     * @param item das Item das dem Inventar hinzugefügt werden soll
     */
    private void AddItem(CollectableItemSO item)
    {
        if (!_items.TryAdd(item, 1))
        {
            _items[item] += 1;
        }
        
        _inventoryUI.ShowItems(_items);
    }

    /**
     * Überprüft, ob das Inventar genug Items für das übergebene Rezept aufweist.
     * @param recipe das Rezept
     * @returns ob im Inventar genug Items für das Rezept sind
     */
    private bool HasEnoughItems(RecipeSO recipe)
    {
        foreach (var ingredient in recipe._ingredients)
        {
            if (!_items.ContainsKey(ingredient._item) || _items[ingredient._item] < ingredient._quantity)
                return false;
        }
        return true;
    }

    /**
     * Erstellt das Item aus dem Rezept, wenn genug Items vorhanden sind.
     * @param recipe das Rezept das erstellt werden soll
     */
    private void Craft(RecipeSO recipe)
    {
        if (!HasEnoughItems(recipe))
        {
            ShowMessage(NOT_CRAFTABLE);
            return;
        }

        if (_winRecipe.name.Equals(recipe.name))
        { 
            if (ScoreManager._score != 0)
            {
                ShowMessage(VIRUSES_LEFT);
            }
            else
            {
                OnWinRecipeCrafted?.Invoke();   
            }
            
            return;
        }

        foreach (var ingredient in recipe._ingredients)
        {
            _items[ingredient._item] -= ingredient._quantity;
            if (_items[ingredient._item] <= 0)
            {
                _items.Remove(ingredient._item);
            }
        }
        
        AddItem(recipe._resultItem);
        ShowMessage($"Crafted {recipe._resultItem._itemName}!");
    }
    
}
