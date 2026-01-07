using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject _deathScreenUI;
    [SerializeField] private GameObject _winScreenUI;
    [SerializeField] private GameObject _shadowBorder;
    [SerializeField] private Image _infectionBar;
    [SerializeField] private Image _skullImg;
    
    /**
     * Versteckt UI-Komponenten die nicht immer angezeigt werden.
     */
    private void Start()
    {
        _shadowBorder.SetActive(false);
        _deathScreenUI.SetActive(false);
        _winScreenUI.SetActive(false);
    }
    
    /**
     * Versteckt den roten Schimmer.
     */
    public void NoMoreInfectedUI()
    {
        _shadowBorder.SetActive(false);
    }
    
    /**
     * Zeigt den roten Schimmer.
     */
    public void InfectUI()
    {
        _shadowBorder.SetActive(true);
    }
    
    /**
     * Zeigt die Gewinnerausgabe.
     */
    public void WinRecipeCraftedUI()
    {
        _winScreenUI.SetActive(true);
    }
    
    /**
     * Zeigt die Todesausgabe.
     */
    public void ShowDeathUI()
    {
        _deathScreenUI.SetActive(true);
    }
    
    /**
     * Updatet die Health-Bar und färbt dabei den Balken und den Totenkopf rötlicher je nachdem wie wenig
     * Leben vorhanden sind.
     * @param resPoints die derzeitigen resistance Punkte des Spielers
     * @param playerResistancePoints die Punkte die ein Spieler insgesamt haben kann
     */
    public void UpdateHealthText(float resPoints, float playerResistancePoints)
    {
        float rawFill = Mathf.Clamp01(resPoints / playerResistancePoints);
        float visualFill = Mathf.Lerp(0.18f, 0.82f, rawFill);
        _infectionBar.fillAmount = visualFill;

        float colorValue = Mathf.Clamp01(rawFill);
        Color targetColor = new Color(1f, colorValue, colorValue);
        _skullImg.color = Color.Lerp(_skullImg.color, targetColor, 5f * Time.deltaTime);
        _infectionBar.color = Color.Lerp(_infectionBar.color, targetColor, 5f * Time.deltaTime);
    }
    
}
