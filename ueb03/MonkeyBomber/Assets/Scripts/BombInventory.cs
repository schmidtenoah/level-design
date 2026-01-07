using System;
using System.Collections.Generic;
using UnityEngine;

public class BombInventory : MonoBehaviour
{

    private const int MAX_PLACEABLE_BOMB_CNT = 1;

    public static event Action<int> OnBombRangeChange;
    public static event Action<bool> OnUseVerticalBombChange;
    public static event Action<Bomb.BombType, int> OnNewActiveBombType;
    public static event Action<int> OnPlacableBombChange;
    public static event Action OnPlaceBombSuccess;
    
    [SerializeField] private GameObject[] _bombPrefabs = new GameObject[Enum.GetValues(typeof(Bomb.BombType)).Length];
    [SerializeField] private int _defaultBombRange = 1;
    
    private static int _maxBombPlaceCnt = MAX_PLACEABLE_BOMB_CNT;
    private static bool _useHorizontalDirs = true;
    private static int _currBombRange;
    private static int _currPlacedBombCnt = 0;
    private static Dictionary<Bomb.BombType, int> _bombs;
    private static Bomb.BombType _currBombType = Bomb.BombType.BOMB_WATER;

    /**
    * Fügt dem Inventar Bomben eines bestimmten Typs hinzu.
    * Wenn der Typ bereits im Inventar vorhanden ist, wird die Anzahl erhöht.
    *
    * @param type Der Typ der hinzuzufügenden Bombe
    * @param cnt  Die Anzahl an Bomben, die hinzugefügt werden sollen
    */
    public static void AddBomb(Bomb.BombType type, int cnt)
    {
        if (!_bombs.TryAdd(type, cnt))
        {
            _bombs[type] += cnt;
        }
        
        OnNewActiveBombType?.Invoke(_currBombType, _bombs[_currBombType]);
    }

    /**
    * Erhöht die maximale Anzahl an gleichzeitig platzierbaren Bomben.
    * Aktualisiert den verfügbaren Platz per Event.
    *
    * @param cnt Anzahl, um die die maximale Platziermenge erhöht werden soll
    */
    public static void IncreaseMaxBombPlaceCnt(int cnt)
    {
        _maxBombPlaceCnt += cnt;
        OnPlacableBombChange?.Invoke(_maxBombPlaceCnt - _currPlacedBombCnt);
    }
    
    /**
    * Aktiviert die vertikale Bombenplatzierung.
    * Löst Event zur Anzeigeänderung aus.
    */
    public static void UseVerticalBombs()
    {
        _useHorizontalDirs = false;
        OnUseVerticalBombChange?.Invoke(true);
    }

    /**
    * Erhöht die aktuelle Reichweite der Bombe um den angegebenen Wert.
    * Gibt den neuen Wert per Event weiter.
    *
    * @param cnt Anzahl an Reichweitenpunkten, um die erhöht werden soll
    */
    public static void IncreaseBombRange(int cnt)
    {
        _currBombRange += cnt;
        OnBombRangeChange?.Invoke(_currBombRange);
    }

    /**
    * Initialisiert das Bomben-Inventar zu Spielbeginn.
    * Setzt Standardwerte und abonniert relevante Events.
    */
    private void Start()
    {
        _bombs = new Dictionary<Bomb.BombType, int>();
        _currBombRange = _defaultBombRange;
        _maxBombPlaceCnt = MAX_PLACEABLE_BOMB_CNT;
        _currPlacedBombCnt = 0;
        _bombs[Bomb.BombType.BOMB_WATER] = 0;
        _bombs[Bomb.BombType.BOMB_ICE] = 0;
        _bombs[Bomb.BombType.BOMB_FIRE] = 0;
        
        PlayerController.OnPlaceBombs += PlaceBomb;
        PlayerController.OnSwitchBombType += SwitchCurrBombType;
        Bomb.OnBombExploded += BombExploded;
        
        OnPlacableBombChange?.Invoke(_maxBombPlaceCnt);
        OnBombRangeChange?.Invoke(_currBombRange);
        OnNewActiveBombType?.Invoke(_currBombType, _bombs[_currBombType]);
        OnUseVerticalBombChange?.Invoke(!_useHorizontalDirs);
    }

    /**
    * Hebt Event-Abonnements beim Deaktivieren des GameObjects wieder auf.
    */
    private void OnDisable()
    {
        PlayerController.OnPlaceBombs -= PlaceBomb;
        PlayerController.OnSwitchBombType -= SwitchCurrBombType;
        Bomb.OnBombExploded -= BombExploded;
    }

    /**
    * Wird aufgerufen, wenn eine Bombe explodiert.
    * Verringert den Zähler der aktuell platzierten Bomben und löst das Event zur Anzeige aus.
    */
    private void BombExploded()
    {
        --_currPlacedBombCnt;
        OnPlacableBombChange?.Invoke(_maxBombPlaceCnt - _currPlacedBombCnt);
    }

    /**
    * Platziert eine Bombe am übergebenen Ort, sofern noch Bomben verfügbar sind 
    * und die maximale Platzieranzahl nicht überschritten wurde.
    * Reduziert den Bombenzähler, setzt Werte zurück und löst mehrere Events aus.
    *
    * @param pos Die Weltposition, an der die Bombe platziert werden soll
    */
    private void PlaceBomb(Vector3 pos)
    {
        if (_bombs[_currBombType] <= 0 || _currPlacedBombCnt >= _maxBombPlaceCnt) return;
        OnPlaceBombSuccess?.Invoke();
        
        GameObject go = Instantiate(_bombPrefabs[(int) _currBombType], pos, Quaternion.identity);
        Bomb bomb = go.GetComponent<Bomb>();
        bomb.Init(_currBombRange, _currBombType, _useHorizontalDirs);
        --_bombs[_currBombType];
        ++_currPlacedBombCnt;
        OnPlacableBombChange?.Invoke(_maxBombPlaceCnt - _currPlacedBombCnt);
        _currBombRange = _defaultBombRange;
        _useHorizontalDirs = true;
        OnBombRangeChange?.Invoke(_currBombRange);
        OnUseVerticalBombChange?.Invoke(false);
        OnNewActiveBombType?.Invoke(_currBombType, _bombs[_currBombType]);
    }

    /**
    * Wechselt den aktuell aktiven Bombentyp – je nach Richtung zyklisch vor oder zurück.
    * Aktualisiert die Anzeige per Event.
    *
    * @param toRight true, wenn nach rechts gewechselt werden soll, false für links
    */
    private void SwitchCurrBombType(bool toRight)
    {
        int bombTypeCnt = Enum.GetValues(typeof(Bomb.BombType)).Length;
        int curr = (int)_currBombType;
        curr = toRight ? (curr + 1) % bombTypeCnt : (curr - 1 + bombTypeCnt) % bombTypeCnt;
        _currBombType = (Bomb.BombType)curr;
        OnNewActiveBombType?.Invoke(_currBombType, _bombs[_currBombType]);
    }
    
}
