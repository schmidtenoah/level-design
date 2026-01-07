using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Collider))]
public class Block : MonoBehaviour
{
    private enum BlockType
    {
        BLOCK_NORMAL,
        BLOCK_WATER,
        BLOCK_ICE,
        BLOCK_FIRE,
    }

    [Header("Design Properties")]
    [Tooltip("Materialien für die Elemente der Blöcke, Anzahl und Reihenfolge anhand des Blocktyp-Enums!")]
    [SerializeField] private Material[] _blockMats = new Material[Enum.GetValues(typeof(BlockType)).Length];
    [Tooltip("Ob das Material ausgehend von _blockMats bestimmt werden soll")]
    [SerializeField] private bool _useElementMats = false;
    [SerializeField] private GameObject _destroyParticles;
    
    [FormerlySerializedAs("_bombPlacable")]
    [Header("Behaviour Properties")]
    [SerializeField] private bool bombPlaceable = true;
    [SerializeField] private bool _destructible = false;
    [SerializeField] private bool _walkable = true;
    [SerializeField] private BlockType _blockType = BlockType.BLOCK_NORMAL;
    
    private Renderer _renderer;

    /**
     * Holt sich den Renderer, um das Material zu ändern und setzt das Anfangsmaterial anhand vom _blockType, wenn
     * _useElementMats aktiviert ist.
     */
    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        ApplyMaterial();
    }

    /**
     * Dieser Block wurde von dem übergebenen Blocktyp getroffen.
     * Anhand des Typen der Bombe und dieses Blockes werden Eigenschaften verändert.
     * @param bombType der Typ der Bombe die diesen Block getroffen hat
     */
    public void BombHit(Bomb.BombType bombType)
    {
        if (_blockType.Equals(BlockType.BLOCK_NORMAL) && _destructible)
        {
            Destroy(gameObject);
            SpawnParticles();
        }
        else if (_blockType.Equals(BlockType.BLOCK_WATER) && bombType.Equals(Bomb.BombType.BOMB_ICE))
        {
            ChangeBlockType(BlockType.BLOCK_ICE);
            _walkable = true;
            bombPlaceable = false;
        }
        else if (_blockType.Equals(BlockType.BLOCK_ICE) && bombType.Equals(Bomb.BombType.BOMB_FIRE))
        {
            SpawnParticles();
            Destroy(gameObject);
        }
        else if (_blockType.Equals(BlockType.BLOCK_FIRE) && bombType.Equals(Bomb.BombType.BOMB_WATER))
        {
            SpawnParticles();
            Destroy(gameObject);
        }
    }

    /**
     * Gibt zurück, ob dieser Block begehbar ist.
     * @returns ob dieser Block begehbar ist
     */
    public bool IsWalkable => _walkable;
    
    /**
     * Gibt zurück, ob auf diesem Block Bomben platziert werden dürfen.
     * @returns ob auf diesem Block Bomben platziert werden dürfen
     */
    public bool BombPlaceable => bombPlaceable;

    /**
     * Ändert den Blocktyp dieses Blockes und verändert anhand dessen ggf. das Material.
     * @param toType der neue Typ des Blockes
     */
    private void ChangeBlockType(BlockType toType)
    {
        _blockType = toType;
        ApplyMaterial();
    }

    /**
     * Setzt das Material anhand des aktuellen _blockType, aber nur wenn _useElementMats aktiviert ist.
     */
    private void ApplyMaterial()
    {
        if ((int)_blockType >= _blockMats.Length || !_useElementMats) 
            return;
        
        _renderer.sharedMaterial = _blockMats[(int)_blockType];
    }

    /**
     * Erzeugt an der derzeitigen Position die _destroyParticles.
     */
    private void SpawnParticles()
    {
        if (!_destroyParticles.IsUnityNull())
        {
            Instantiate(_destroyParticles, transform.position, Quaternion.identity);
        }
    }
    
}
