using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    private int _score;
    [SerializeField] private Text scoreText;
    
    private void UpdateScoreText()
    {
        scoreText.text = $"Score: {_score}";
    }
    
    private void Start()
    {
        var viruses = FindObjectsByType<Virus>(FindObjectsSortMode.None);
        foreach (Virus virus in viruses)
        {
            virus.OnVirusCaptured += VirusCaptured;
        }
        UpdateScoreText();
    }

    private void VirusCaptured(Virus virus)
    {
        virus.OnVirusCaptured -= VirusCaptured;
        ++_score;
        UpdateScoreText();
    }

    private void OnDisable()
    {
        var viruses = FindObjectsByType<Virus>(FindObjectsSortMode.None);
        foreach (Virus virus in viruses)
        {
            virus.OnVirusCaptured -= VirusCaptured;
        }
    }
}
