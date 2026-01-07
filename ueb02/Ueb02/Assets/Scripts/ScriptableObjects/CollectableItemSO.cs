using UnityEngine;

[CreateAssetMenu(fileName = "NewCollectableItem", menuName = "ScriptableObjects/CollectableItemSO")]
public class CollectableItemSO : ScriptableObject
{
    
    public string _itemName;
    public Sprite _icon;
    public GameObject _prefab; 
    
}
