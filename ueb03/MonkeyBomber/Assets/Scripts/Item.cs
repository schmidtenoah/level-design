using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Item : MonoBehaviour
{
    public enum ItemType
    {
        ITEM_INC_BOMB_RANGE,
        ITEM_VERTICAL_BOMBS,
        ITEM_MORE_SPEED,
        ITEM_MORE_BOMBS,
        ITEM_GAIN_TIME,
        ITEM_EXTRA_BOMB,
    }

    public static event Action OnItemCollected;
    [SerializeField] private ItemType _item;
    [SerializeField] private ItemPrefabsSO _itemPrefabs;
    
    [Header("More Bomb Range")]
    [SerializeField] private int _incBombRangeCnt = 1;
    
    [Header("More Bombs")]
    [SerializeField] private int _incBombCnt = 1;
    
    [Header("More Speed")]
    [SerializeField] private float _speedModifier = 1.5f;
    
    [Header("Extra Bomb")]
    [SerializeField] private Bomb.BombType _extraBombType;
    [SerializeField] private int _extraBombCnt = 2;

    [Header("Gain Time")] 
    [SerializeField] private float _moreTimeSec = 5.0f;

    /**
    * Instanziiert das passende 3D-Modell (Prefab) für das Item anhand des Item-Typs.
    * Bei Extra-Bomben wird der Offset durch den Bombentyp berechnet.
    */
    private void Start()
    {
        int prefabOffset = (_item.Equals(ItemType.ITEM_EXTRA_BOMB)) ? (int) _extraBombType : 0;
        GameObject prefab = _itemPrefabs._itemPrefabs[(int)_item + prefabOffset];
        Instantiate(prefab, transform.position + Vector3.up,
            prefab.transform.rotation, transform);
    }

    /**
    * Wird ausgelöst, wenn ein Collider (z.B. der Spieler) mit dem Item kollidiert.
    * Führt je nach Item-Typ die entsprechende Aktion aus:
    * z.B. mehr Bomben, höhere Geschwindigkeit oder zusätzliche Zeit.
    * Anschließend wird das Item zerstört und ein Sammel-Event ausgelöst.
    *
    * @param other Der Collider des Objekts, das das Item berührt hat
    */
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        switch (_item)
        {
            case ItemType.ITEM_INC_BOMB_RANGE:
            {   
                BombInventory.IncreaseBombRange(_incBombRangeCnt);
                break;
            }
            case ItemType.ITEM_VERTICAL_BOMBS:
            {
                BombInventory.UseVerticalBombs();
                break;
            }
            case ItemType.ITEM_MORE_BOMBS:
            {
                BombInventory.IncreaseMaxBombPlaceCnt(_incBombCnt);
                break;
            }
            case ItemType.ITEM_MORE_SPEED:
            {
                PlayerController.SetSpeedModifier(_speedModifier);
                break;
            }
            case ItemType.ITEM_EXTRA_BOMB:
            {
                BombInventory.AddBomb(_extraBombType, _extraBombCnt);
                break;
            }
            case ItemType.ITEM_GAIN_TIME:
            {
                GameManager.GainTime(_moreTimeSec);
                break;
            }
            default: break;
        }
        
        OnItemCollected.Invoke();
        Destroy(this.gameObject);
    }
    
    /**
    * Zeichnet im Editor eine magentafarbene Kugel oberhalb des Items,
    * um dessen Position visuell hervorzuheben.
    */
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(transform.position + Vector3.up, 0.5f);
    }
    
}
