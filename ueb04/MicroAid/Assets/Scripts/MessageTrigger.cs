using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MessageTrigger : MonoBehaviour
{

    public static event Action<string> OnMessageTrigger; 

    [SerializeField] private bool _oneTimeTrigger = true;
    [SerializeField] private string _message = "Hello";

    /**
     * Macht den Collider zum Trigger.
     */
    private void Start()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    /**
     * Wenn der Spieler den Collider betritt, soll die Nachricht angezeigt werden.
     * @param other Collider des anderen Objektes
     */
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        
        OnMessageTrigger?.Invoke(_message);
        if (_oneTimeTrigger)
        {
            Destroy(gameObject);
        }
    }
    
}
