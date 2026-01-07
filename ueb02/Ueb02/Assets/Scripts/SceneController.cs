using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerInput))]
public class SceneController : MonoBehaviour
{

    [SerializeField] private PlayerHealthManager _playerHealth;
    
    /**
     * PlayerInput Methode, die das Level neu startet, wenn man verloren hat,
     * bzw. das nächste Lädt, wenn man gewonnen hat.
     * @param val wird nicht benötigt
     */
    private void OnRestartLevel(InputValue val)
    {
        switch (_playerHealth._status)
        {
            case PlayerHealthManager.PlayerStatus.PLAYER_WON:
                if (SceneManager.GetActiveScene().name.Equals("Level1"))
                {
                    Time.timeScale = 1.0f;
                    SceneManager.LoadScene("Level2");
                }
                break;
            case PlayerHealthManager.PlayerStatus.PLAYER_DEAD:
                Time.timeScale = 1.0f;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                break;
            case PlayerHealthManager.PlayerStatus.PLAYER_PLAYING:
            default:
                break;
        }
    }
    
    /**
     * PlayerInput Methode, die immer das erste Level lädt.
     * @param val wird nicht benötigt
     */
    private void OnSelectLevel1(InputValue val)
    {
        
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Level1");
    }
    
    /**
    * PlayerInput Methode, die immer das zweite Level lädt.
    * @param val wird nicht benötigt
    */
    private void OnSelectLevel2(InputValue val)
    {
        
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Level2");
    }
    
}
