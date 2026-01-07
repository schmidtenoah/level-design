using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{

    public static event Action<int> OnScoreChange;
    private static int _score = 0;
    
    /**
     * Events bestellen und Anfangsscore anhand aller Viren der Szene setzen.
     */
    private void Start()
    {
        var viruses = FindObjectsByType<Virus>(FindObjectsSortMode.None);
        _score = viruses.Length;
        Virus.OnVirusHealed += VirusHealed;
        Boss.OnVirusSpawn += AddNewVirus;
        OnScoreChange?.Invoke(_score);
        Boss.OnBossDefeated += VirusHealed;
    }

    /**
     * Verringert den Score, weil ein Virus geheilt wurde.
     */
    private static void VirusHealed()
    {
        --_score;
        OnScoreChange?.Invoke(_score);
    }

    /**
     * Erhöht den Score, weil ein neues Virus erschaffen wurde.
     */
    private static void AddNewVirus()
    {
        ++_score;
        OnScoreChange?.Invoke(_score);
    }

    /**
     * Deabonniert alle Events.
     */
    private void OnDisable()
    {
        Boss.OnBossDefeated -= VirusHealed;
        Virus.OnVirusHealed -= VirusHealed;
        Boss.OnVirusSpawn -= AddNewVirus;
    }
    
}
