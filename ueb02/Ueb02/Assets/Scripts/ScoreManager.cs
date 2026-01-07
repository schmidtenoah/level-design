using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    
    public static int _score { get; private set; }
    [SerializeField] private TextMeshProUGUI _scoreText;
    
    /**
     * Zeigt die neue Anzahl angefangenen Viren in dem UI an.
     */
    private void UpdateScoreText()
    {
        if (!_scoreText.IsUnityNull())
        {
            _scoreText.text = _score.ToString();
        }
    }
    
    /**
     * Zählt alle Viren, die sich in der Szene befinden und fügt jedem das VirusCaptured-Event hinzu, sowie
     * initialisiert damit den Score und das UI.
     */
    private void Start()
    {
        var viruses = FindObjectsByType<Virus>(FindObjectsSortMode.None);
        _score = viruses.Length;
        foreach (Virus virus in viruses)
        {
            virus.OnVirusCaptured += VirusCaptured;
        }
        UpdateScoreText();
    }

    /**
     * Deabonniert das Event vom übergebenen Virus-Objekt und verringert den score.
     * @param virus der Virus, der gefangen wurde
     */
    private void VirusCaptured(Virus virus)
    {
        virus.OnVirusCaptured -= VirusCaptured;
        --_score;
        UpdateScoreText();
    }

    /**
     * Deabonniert alle restlichen Events aller Viren der Szene.
     */
    private void OnDisable()
    {
        var viruses = FindObjectsByType<Virus>(FindObjectsSortMode.None);
        foreach (Virus virus in viruses)
        {
            virus.OnVirusCaptured -= VirusCaptured;
        }
    }
    
}
