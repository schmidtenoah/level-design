using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    private enum GameState
    {
        STATE_WON,
        STATE_LOST,
        STATE_PLAYING
    }

    public static event Action OnGameLost; 
    public static event Action OnGameWon;
    public static event Action<float> OnTimeChange;

    [SerializeField] private float _timeBeforeGameFreeze = 0.1f;
    [SerializeField] private float _timeLimit = 200f;
    private static float _currTimeLimit;
    private GameState _currState = GameState.STATE_PLAYING;


    /**
    * Erhöht die verbleibende Spielzeit um den angegebenen Wert (in Sekunden).
    *
    * @param timeSec Anzahl an Sekunden, um die das Zeitlimit erhöht wird
    */
    public static void GainTime(float timeSec) => _currTimeLimit += timeSec;

    /**
    * Initialisiert das Spiel beim Start:
    * Setzt das Zeitlimit und abonniert Events für Spielende bei Sieg oder Niederlage.
    */
    private void Start()
    {
        _currTimeLimit = _timeLimit;
        Bomb.OnBombHitPlayer += EndLostGame;
        BlockGoal.OnReachedGoal += EndWonGame;
    }

    /**
    * Hebt die Event-Registrierungen bei Deaktivierung des GameManagers wieder auf.
    */
    private void OnDisable()
    {
        Bomb.OnBombHitPlayer -= EndLostGame;
        BlockGoal.OnReachedGoal -= EndWonGame;
    }

    /**
    * Beendet das Spiel mit dem Status „verloren“.
    * Verhindert mehrfaches Auslösen, setzt Zustand, feuert Event und friert das Spiel ein.
    */
    private void EndLostGame()
    {
        if (_currState.Equals(GameState.STATE_LOST)) return;
        
        OnGameLost?.Invoke();
        _currState = GameState.STATE_LOST;
        StartCoroutine(FreezeGame());
    }
    
    /**
    * Beendet das Spiel mit dem Status „gewonnen“.
    * Löst entsprechendes Event aus und friert das Spiel ein.
    */
    private void EndWonGame()
    {
        OnGameWon?.Invoke();
        StartCoroutine(FreezeGame());
        _currState = GameState.STATE_WON;
    }

    /**
    * Aktualisiert die verbleibende Spielzeit im laufenden Spiel.
    * Löst Event zur Zeitaktualisierung aus.
    * Beendet das Spiel, wenn die Zeit abgelaufen ist.
    */
    private void Update()
    {
        _currTimeLimit -= Time.deltaTime;
        OnTimeChange?.Invoke(_currTimeLimit / _timeLimit);
        if (_currTimeLimit <= 0f)
        {
            EndLostGame();
        } 
        else if (_currTimeLimit > _timeLimit)
        {
            _timeLimit = Math.Abs(_currTimeLimit);
        }
    }

    /**
    * Wechselt je nach aktuellem Spielstatus das Level:
    * - Bei Gewinn wird das nächste Level geladen
    * - Bei Verlust wird das aktuelle Level neu geladen
    */
    private void OnChangeLevel()
    {
        switch (_currState)
        {
            case GameState.STATE_WON:
                if (SceneManager.GetActiveScene().name.Equals("Level1"))
                {
                    Time.timeScale = 1.0f;
                    SceneManager.LoadScene("Level2");
                }
                else if (SceneManager.GetActiveScene().name.Equals("Level2"))
                {
                    Time.timeScale = 1.0f;
                    SceneManager.LoadScene("Level1");
                }
                break;

            case GameState.STATE_LOST:
            case GameState.STATE_PLAYING:
                Time.timeScale = 1.0f;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                break;
        }
    }

    /**
    * Lädt das erste Level und setzt die Spielzeit zurück.
    */
    private void OnSelectLvl1()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Level1");
    }
    
    /**
    * Lädt das zweite Level und setzt die Spielzeit zurück.
    */
    private void OnSelectLvl2()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Level2");
    }

    /**
    * Coroutine, die das Spiel nach kurzer Verzögerung anhält (Time.timeScale = 0).
    *
    * @return IEnumerator für die Coroutine-Verwaltung
    */
    private IEnumerator FreezeGame()
    {
        yield return new WaitForSeconds(_timeBeforeGameFreeze);
        Time.timeScale = 0f;
    }
    
}
