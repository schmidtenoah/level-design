using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MessageTrigger : MonoBehaviour
{

    public static event Action<string> OnDisplayMessage;
    [SerializeField] private string _displayMsg;
    [SerializeField] private bool _oneTimeTrigger;

    /**
    * Wird ausgelöst, wenn ein anderes Objekt diesen Trigger betritt.
    * Sendet die gespeicherte Nachricht über ein Event.
    *
    * @param other Der Collider des Objekts, das den Trigger ausgelöst hat
    */
    private void OnTriggerEnter(Collider other)
    {
        OnDisplayMessage?.Invoke(_displayMsg);
        if (_oneTimeTrigger)
        {
            Destroy(gameObject);
        }
    }
    
}
