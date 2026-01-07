using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    
    private enum GameState { STATE_WON, STATE_LOST, STATE_PLAYING }
    private GameState _currGameState;

    /**
     * Events bestellen.
     */
    private void Awake()
    {
        Time.timeScale = 1.0f;
        Heart.OnHeartHealed += WinGame;
        PlayerHealthManager.OnPlayerDead += LoseGame;
        _currGameState = GameState.STATE_PLAYING;
    }

    /**
     * Events abbestellen.
     */
    private void OnDisable()
    {
        Heart.OnHeartHealed -= WinGame;
        PlayerHealthManager.OnPlayerDead -= LoseGame;
    }

    /**
     * Beendet das gewonnene Spiel.
     */
    private void WinGame()
    {
        Time.timeScale = 0f;
        _currGameState = GameState.STATE_WON;
    }
    
    /**
     * Beendet das verlorene Spiel.
     */
    private void LoseGame()
    {
        Time.timeScale = 0f;
        _currGameState = GameState.STATE_LOST;
    }
    
    /**
     * PlayerInput Methode, die das Level neu startet, wenn man verloren hat,
     * bzw. das nächste Lädt, wenn man gewonnen hat.
     */
    private void OnRestartLevel()
    {
        switch (_currGameState)
        {
            case GameState.STATE_WON:
                if (SceneManager.GetActiveScene().name.Equals("Level1"))
                    LoadLevel("Level2");
                break;
            case GameState.STATE_LOST:
                LoadLevel(SceneManager.GetActiveScene().name);
                break;
            case GameState.STATE_PLAYING:
            default:
                break;
        }
    }
    
    /**
     * PlayerInput Methode, die immer das erste Level lädt.
     */
    private void OnSelectLevel1() => LoadLevel("Level1");

    /**
    * PlayerInput Methode, die immer das zweite Level lädt.
    */
    private void OnSelectLevel2() => LoadLevel("Level2");

    /**
     * Lädt eine Scene mit dem übergebenen Namen.
     * @param sceneName Name der Scene die geladen werden soll
     */
    private static void LoadLevel(string sceneName)
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(sceneName);
    }
    
}
