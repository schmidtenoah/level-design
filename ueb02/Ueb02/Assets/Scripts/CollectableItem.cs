using UnityEngine;
using UnityEngine.Serialization;

public class CollectableItem : MonoBehaviour
{
    
    [FormerlySerializedAs("itemData")] 
    [SerializeField] private CollectableItemSO _itemData;

    /**
     * Lädt das Modell des Item.
     */
    private void Start()
    {
        Instantiate(_itemData._prefab, transform);
    }

    /**
     * Zeichnet ein Gizmo dort wo das Item-Modell geladen wird.
     */
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(transform.position + Vector3.up, 0.65f);
    }

    /**
     * Getter für die Daten des Items.
     * @returns das Item des Collectables
     */
    public CollectableItemSO GetItemData()
    {
        return _itemData;
    }
    
}
