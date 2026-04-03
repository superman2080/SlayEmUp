using UnityEngine;
using UnityEngine.Events;

public class EventListener : MonoBehaviour
{
    [Header("Event Attribute")]
    public EventChannelSO eventSO;
    public UnityEvent response;

    private void OnEnable()
    {
        eventSO.RegisterListener(this);
    }

    private void OnDisable()
    {
        eventSO.UnRegisterListener(this);
    }

    public void OnEventRaised()
    {
        response?.Invoke();
    }

}
