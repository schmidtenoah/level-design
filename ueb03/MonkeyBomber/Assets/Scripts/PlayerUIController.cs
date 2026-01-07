using UnityEngine;

public class PlayerUIController : MonoBehaviour
{
   
    [SerializeField] private Camera _cam;
    [SerializeField] private GameObject _bombPrefab;
    [SerializeField] private Transform _bombSlotParent;

    /**
    * Registriert Event-Listener für Änderungen der platzierbaren Bombenanzahl.
    */
    private void Awake()
    {
        BombInventory.OnPlacableBombChange += SetPlacableBombs;
    }

    /**
    * Hebt Event-Registrierung beim Deaktivieren des Objekts auf.
    */
    private void OnDisable()
    {
        BombInventory.OnPlacableBombChange -= SetPlacableBombs;
    }

    /**
    * Passt die Anzeige der verfügbaren Bomben an, 
    * indem Prefabs instanziiert oder entfernt werden.
    *
    * @param cnt Die aktuelle Anzahl an platzierbaren Bomben
    */
    private void SetPlacableBombs(int cnt)
    {
        int current = _bombSlotParent.childCount;

        if (cnt < current)
        {
            for (int i = 0; i < current - cnt; i++)
            {
                Destroy(_bombSlotParent.GetChild(0).gameObject);
            }
        }
        else if (cnt > current)
        {
            for (int i = 0; i < cnt - current; i++)
            {
                Instantiate(_bombPrefab, _bombSlotParent);
            }
        }
    }

    /**
    * Dreht das UI-Element so, dass es stets zur Kamera zeigt (billboard-Effekt).
    */
    private void LateUpdate()
    {
        Vector3 lookDir = transform.position - _cam.transform.position;
        lookDir.y = 0;

        if (lookDir.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(lookDir);
        }
    }
    
}
