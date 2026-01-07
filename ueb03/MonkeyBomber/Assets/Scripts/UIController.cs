using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class UIController : MonoBehaviour
{
    
    [Header("Clock")]
    [SerializeField] private Image _circleImage;
    [SerializeField] private RectTransform _shakeTarget;
    [SerializeField] private float _shakeThreshold = 0.2f;
    [SerializeField] private float _shakeIntensity = 10f;
    [SerializeField] private float _shakeStepTime = 0.04f;
    private Coroutine _shakeRoutine;
    private Vector3 _origAnchoredPos;
    
    [Header("Bomb")]
    [SerializeField] private TextMeshProUGUI[] _rangeTxts;
    [SerializeField] private TextMeshProUGUI _bombCntTxt;
    [SerializeField] private GameObject _horizontalArrows;
    [SerializeField] private GameObject _verticalArrows;
    [Tooltip("Same order as BombType Array")]
    [SerializeField] private Sprite[] _bombImgs = new Sprite[Enum.GetValues(typeof(Bomb.BombType)).Length];
    [SerializeField] private Image _bombImg;
    
    [Header("Message")]
    [SerializeField] private GameObject _messageContainer;
    [SerializeField] private TextMeshProUGUI _messageTxt;
    [SerializeField] private float _displayTimeSec = 5f;
    [SerializeField] private float _fadeDuration = 1f;
    [SerializeField] private string _onGameLostTxt;
    [SerializeField] private string _onGameWonTxt;
    private Vector3 _originalScale;
    private Coroutine _hideMessageUIRoutine;
    
    /**
     * Abonniert alle Events und setzt Grundeinstellungen.
     */
    private void Awake()
    {
        _messageContainer.SetActive(false);
        _originalScale = _messageContainer.transform.localScale;
        GameManager.OnTimeChange += UpdateTime;
        BombInventory.OnNewActiveBombType += SwitchBombType;
        BombInventory.OnUseVerticalBombChange += UseVerticalBombs;
        BombInventory.OnBombRangeChange += ResetBombRange;
        MessageTrigger.OnDisplayMessage += DisplayUIMessage;
        GameManager.OnGameLost += DisplayGameLostMsg;
        GameManager.OnGameWon += DisplayGameWonMsg;
    }

    /**
     * Deabonniert alle Events.
     */
    private void OnDestroy()
    {
        GameManager.OnTimeChange -= UpdateTime;
        BombInventory.OnNewActiveBombType -= SwitchBombType;
        BombInventory.OnUseVerticalBombChange -= UseVerticalBombs;
        BombInventory.OnBombRangeChange -= ResetBombRange;
        MessageTrigger.OnDisplayMessage -= DisplayUIMessage;
        GameManager.OnGameLost -= DisplayGameLostMsg;
        GameManager.OnGameWon -= DisplayGameWonMsg;
    }

    /**
    * Zeigt den Game-Over/Win--Text fürs Spielende an.
    */
    private void DisplayGameLostMsg() => DisplayUIMessage(_onGameLostTxt);
    private void DisplayGameWonMsg() => DisplayUIMessage(_onGameWonTxt);

    /**
     * Erneuert die UI Anzeige für die Zeit.
     * @param timeAspect Zahl zwischen 0 und 1 
     */
    private void UpdateTime(float timeAspect)
    {
        _circleImage.fillAmount = timeAspect;
        _circleImage.color = Color.Lerp(Color.red, Color.green, timeAspect);
        
        if (timeAspect <= _shakeThreshold)
        {
            _shakeRoutine ??= StartCoroutine(ShakeLoop());
        }
        else
        {
            if (_shakeRoutine != null) StopShake();
        }
    }
    
    /**
    * Coroutine, die das UI-Element für die Zeitwarnung dauerhaft leicht zittern lässt.
    * Wird gestartet, wenn die Zeit unter den kritischen Schwellenwert fällt.
    *
    * @return IEnumerator für die Coroutine
    */
    private IEnumerator ShakeLoop()
    {
        while (true)
        {
            Vector2 offset = Random.insideUnitCircle * _shakeIntensity;
            _shakeTarget.anchoredPosition = _origAnchoredPos + (Vector3)offset;
            yield return new WaitForSecondsRealtime(_shakeStepTime);
        }
    }

    /**
    * Stoppt die laufende Shake-Coroutine und setzt das UI-Element auf seine Ausgangsposition zurück.
    */
    private void StopShake()
    {
        StopCoroutine(_shakeRoutine);
        _shakeRoutine = null;
        _shakeTarget.anchoredPosition = _origAnchoredPos;
    }

    /**
     * Aktualisiert das UI für den aktuell aktiven Bombentyp und dessen verbleibende Anzahl.
    *
    * @param bombType Der aktuell gewählte Bombentyp
    * @param count    Anzahl der verfügbaren Bomben dieses Typs
    */
    private void SwitchBombType(Bomb.BombType bombType, int count)
    {
        _bombImg.sprite = _bombImgs[(int)bombType];
        _bombCntTxt.text = count.ToString();
    }

    /**
     * Schaltet die visuelle Anzeige zwischen horizontalen und vertikalen Bombenrichtungen um.
    *
    * @param useVert true = vertikale Ausrichtung
    */
    private void UseVerticalBombs(bool useVert)
    {
        _verticalArrows.SetActive(useVert);
        _horizontalArrows.SetActive(!useVert);
    }
    
    /**
    * Aktualisiert die Anzeige der Bombenreichweite für alle betroffenen UI-Textelemente.
    *
    * @param range Die neue Reichweite der Bombe
    */
    private void ResetBombRange(int range)
    {
        foreach (var txt in _rangeTxts)
        {
            txt.text = range.ToString();
        }
    }
    
    /**
     * Zeigt eine neue Nachricht auf dem Bildschirm an, die nach der festgelegten Zeit wieder ausgeblendet wird.
     * Bereits angezeigte Nachrichten werden entfernt.
     * @param txt die Nachricht die angezeigt werden soll
     */
    private void DisplayUIMessage(string txt)
    {
        _messageContainer.SetActive(true);
        _messageTxt.text = txt;
        _messageContainer.transform.localScale = _originalScale;
        
        if (_hideMessageUIRoutine != null)
        {
            StopCoroutine(_hideMessageUIRoutine);
        }
        _hideMessageUIRoutine = StartCoroutine(HideDisplayUIAfter(_displayTimeSec));

    }

    /**
     * Coroutine, die die aktuelle Nachricht für die übergebene Zeit ausblendet.
     * @param seconds Anzahl an Sekunden nachdem das UI ausgeblendet werden soll
     * @returns Wert für die Coroutine
     */
    private IEnumerator HideDisplayUIAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        float elapsed = 0f;
        Vector3 startScale = _originalScale;

        while (elapsed < _fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _fadeDuration;
            _messageContainer.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            yield return null;
        }
        
        _messageContainer.SetActive(false);
    }
    
}
