using UnityEngine;

[CreateAssetMenu(fileName = "ItemPrefabsSO", menuName = "SO/ItemPrefabsSO")]
public class ItemPrefabsSO : ScriptableObject
{
    
    [Tooltip("Reihenfolge MUSS exakt der ItemType-Reihenfolge entsprechen!\n" +
             "Für ITEM_EXTRA_BOMB muss es so viele Prefabs geben wie es BombenTypen gibt!")]
    public GameObject[] _itemPrefabs = new GameObject[
        System.Enum.GetValues(typeof(Item.ItemType)).Length + System.Enum.GetValues(typeof(Bomb.BombType)).Length - 1
    ];
    
}
