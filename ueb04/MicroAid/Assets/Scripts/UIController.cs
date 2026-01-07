using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    [SerializeField] private GameObject _healCrosshair;
    [SerializeField] private GameObject _damageCrosshair;
    
    [SerializeField] private Image _infectionBar;
    [SerializeField] private Image _skullImg;
    [SerializeField] private GameObject _infectionShadow;
    
    [SerializeField] private GameObject _winScreen;
    [SerializeField] private GameObject _deathScreen;

    [SerializeField] private TextMeshProUGUI _messageTxt;
    [SerializeField] private GameObject _messageContainer;
    [SerializeField] private float _fadeDuration = 1f;
    [SerializeField] private float _displayTimeSec = 2f;
    private Vector3 _originalScale;
    private Coroutine _hideMessageUIRoutine;
    private readonly Queue<string> _messageQueue = new();
    private bool _isDisplayingMessage;
    
    [SerializeField] private TextMeshProUGUI _virusCounterTxt;
    [SerializeField] private string _healHeartTxt = "Healing the Heart is Possible now";
    
    [SerializeField] private GameObject _bossBarContainer;
    [SerializeField] private Transform _verticalLayoutGrpParent;
    [SerializeField] private GameObject _bossBarPrefab;


    /**
     * UI-Elemente ausblenden und alle Events abonnieren.
     */
    private void Awake()
    {
        _bossBarContainer.SetActive(false);
        _infectionShadow.SetActive(false);
        _messageContainer.SetActive(false);
        _winScreen.SetActive(false);
        _deathScreen.SetActive(false);
        _isDisplayingMessage = false;
        _originalScale = _messageContainer.transform.localScale;
        
        WeaponController.OnWeaponChange += SwitchCrosshair;
        PlayerHealthManager.OnHPChange += UpdateHealthText;
        PlayerHurtbox.OnBloodCellCollect += DisplayDamageAmmoTxt;
        PlayerHurtbox.OnPillCollect += DisplayHealthAmmoTxt;
        InfectionZone.OnPlayerEnterInfectionZone += EnterInfectionZone;
        InfectionZone.OnPlayerExitInfectionZone += ExitInfectionZone;
        PlayerHealthManager.OnPlayerDead += ShowDeathScreen;
        Heart.OnHeartHealed += ShowWinScreen;
        ScoreManager.OnScoreChange += ChangeScore;
        Boss.OnBossDefeated += HideBossBar;
        Boss.OnBossHealthChange += UpdateBossBars;
        Boss.OnPlayerNearby += SetBossBarActive;
        PlayerHurtbox.OnPotionCollect += DisplayPotionCollectedTxt;
        MessageTrigger.OnMessageTrigger += DisplayUIMessage;
    }
    
    /**
     * Events abbestellen.
     */
    private void OnDisable()
    {
        WeaponController.OnWeaponChange -= SwitchCrosshair;
        PlayerHealthManager.OnHPChange -= UpdateHealthText;
        PlayerHurtbox.OnBloodCellCollect -= DisplayDamageAmmoTxt;
        PlayerHurtbox.OnPillCollect -= DisplayHealthAmmoTxt;
        InfectionZone.OnPlayerEnterInfectionZone -= EnterInfectionZone;
        InfectionZone.OnPlayerExitInfectionZone -= ExitInfectionZone;
        PlayerHealthManager.OnPlayerDead -= ShowDeathScreen;
        Heart.OnHeartHealed -= ShowWinScreen;
        ScoreManager.OnScoreChange -= ChangeScore;
        Boss.OnBossDefeated -= HideBossBar;
        Boss.OnBossHealthChange -= UpdateBossBars;
        Boss.OnPlayerNearby -= SetBossBarActive;
        PlayerHurtbox.OnPotionCollect -= DisplayPotionCollectedTxt;
        MessageTrigger.OnMessageTrigger -= DisplayUIMessage;
    }

    /** Zeigt WinScreen im UI an. */
    private void ShowWinScreen() => _winScreen.SetActive(true);
    
    /** Zeigt DeathScreen im UI an. */
    private void ShowDeathScreen() => _deathScreen.SetActive(true);
    
    /** Zeigt roten Schatten im UI an. */
    private void EnterInfectionZone(float dmg) => _infectionShadow.SetActive(true);
    
    /** Versteckt roten Schatten im UI an. */
    private void ExitInfectionZone() => _infectionShadow.SetActive(false);
    
    /** Wechselt den Crosshair passend zur Waffe. */
    private void SwitchCrosshair(WeaponController.WeaponType toType)
    {
        _healCrosshair.SetActive(toType.Equals(WeaponController.WeaponType.WEAPON_HEAL));
        _damageCrosshair.SetActive(toType.Equals(WeaponController.WeaponType.WEAPON_DAMAGE));
    }

    /** Zeigt neuen Score im UI an. */
    private void ChangeScore(int score) => _virusCounterTxt.text = score.ToString();

    /** Zeigt BossBar im UI an/aus. @param active anzeigen/auszeigen */
    private void SetBossBarActive(bool active)
    {
        _bossBarContainer.SetActive(active);
        _verticalLayoutGrpParent.gameObject.SetActive(active);
        foreach (Transform child in _verticalLayoutGrpParent)
        {
            child.gameObject.SetActive(active);
        }
    }
    
    /** Versteckt BossBar im UI. */
    private void HideBossBar() => SetBossBarActive(false);
    
    /**
     * Updatet die Health-Bar und färbt dabei den Balken und den Totenkopf rötlicher je nachdem wie wenig
     * Leben vorhanden sind.
     * @param resPoints die derzeitigen resistance Punkte des Spielers
     * @param playerResistancePoints die Punkte die ein Spieler insgesamt haben kann
     */
    private void UpdateHealthText(float currPoints, float maxPoints)
    {
        float rawFill = Mathf.Clamp01(currPoints / maxPoints);
        float visualFill = Mathf.Lerp(0.18f, 0.82f, rawFill);
        _infectionBar.fillAmount = visualFill;

        float colorValue = Mathf.Clamp01(rawFill);
        Color targetColor = new Color(1f, colorValue, colorValue);
        _skullImg.color = Color.Lerp(_skullImg.color, targetColor, 5f * Time.deltaTime);
        _infectionBar.color = Color.Lerp(_infectionBar.color, targetColor, 5f * Time.deltaTime);
    }

    /**
     * Aktualisiert alle BossBars anhand der übergebenen Werte.
     * @param partCnt Anzahl an Bossteil insgesamt
     * @param bossPart der hinterste Bossteil
     */
    private void UpdateBossBars(int partCnt, BossPart bossPart)
    {
        foreach (Transform child in _verticalLayoutGrpParent)
        {
            Destroy(child.gameObject);
        }
        
        for (var i = 0; i < partCnt; ++i)
        {
            var slot = Instantiate(_bossBarPrefab, _verticalLayoutGrpParent);
            var bar = slot.transform.Find("Bar").GetComponent<Image>();
            var lockImg = slot.transform.Find("Lock").GetComponent<Image>();
            if (i == partCnt - 1)
            {
                var t = bossPart.GetCurrHealth() / bossPart.GetMaxHealth();
                bar.fillAmount = t;
                bar.color = Color.Lerp(Color.red, Color.green, t);
                lockImg.gameObject.SetActive(!bossPart.IsDamageable());
            }
            else
            {
                lockImg.gameObject.SetActive(true);
                bar.color = Color.green;
            }
        }
    }

    /**
     * Zeigt erhaltene Damage-Munition im UI an.
     * @param cnt Anzahl an erhaltener Munition
     */
    private void DisplayDamageAmmoTxt(int cnt)
    {
        _messageTxt.color = Color.red;
        DisplayUIMessage($"+{cnt}% Damage Ammo");
    }
    
    /**
     * Zeigt erhaltene Health-Munition im UI an.
     * @param cnt Anzahl an erhaltener Munition
     */
    private void DisplayHealthAmmoTxt(int cnt)
    {
        _messageTxt.color = Color.green;
        DisplayUIMessage($"+{cnt}% Heal Ammo");
    }

    /**
     * Zeigt erhaltene Potion im UI an.
     */
    private void DisplayPotionCollectedTxt()
    {
        _messageTxt.color = Color.black;
        DisplayUIMessage(_healHeartTxt);
    }
    
    /**
     * Zeigt eine Nachricht im UI an.
     * @param txt Text der Nachricht
     */
    private void DisplayUIMessage(string txt)
    {
        _messageQueue.Enqueue(txt);
    
        if (!_isDisplayingMessage)
        {
            StartCoroutine(ProcessMessageQueue());
        }
    }
    
    /**
     * Coroutine, die die aktuelle Nachricht für die übergebene Zeit ausblendet.
     * @param seconds Anzahl an Sekunden nachdem das UI ausgeblendet werden soll
     * @returns Wert für die Coroutine
     */
    private IEnumerator ProcessMessageQueue()
    {
        _isDisplayingMessage = true;

        while (_messageQueue.Count > 0)
        {
            string msg = _messageQueue.Dequeue();
            _messageContainer.SetActive(true);
            _messageTxt.text = msg;
            _messageContainer.transform.localScale = _originalScale;
            yield return new WaitForSeconds(_displayTimeSec);
            
            float elapsed = 0f;
            Vector3 startScale = _originalScale;
            while (elapsed < _fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _fadeDuration;
                _messageContainer.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
                yield return null;
            }

            _messageTxt.color = Color.white;
            _messageContainer.SetActive(false);
        }

        _isDisplayingMessage = false;
    }
    
}
