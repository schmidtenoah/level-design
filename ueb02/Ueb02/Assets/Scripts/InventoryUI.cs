using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    
    [SerializeField] private GameObject _itemSlotPrefab;
    [SerializeField] private Transform _itemSlotParent;
    
    /**
     * Zeigt die übergebene Liste an Items in der UI an.
     * @param items die anzuzeigenden Items mit ihrer jeweiligen Anzahl
     */
    public void ShowItems(Dictionary<CollectableItemSO, int> items)
    {
        foreach (Transform child in _itemSlotParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in items)
        {
            GameObject slot = Instantiate(_itemSlotPrefab, _itemSlotParent);
            Image icon = slot.transform.Find("ItemSprite").GetComponent<Image>();
            icon.sprite = item.Key._icon;
            TextMeshProUGUI countText = slot.GetComponentInChildren<TextMeshProUGUI>();
            countText.text = $"{item.Value}x";
        }
    }

}
