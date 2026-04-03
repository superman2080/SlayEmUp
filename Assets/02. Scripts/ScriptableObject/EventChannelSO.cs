using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EventChannelSO", menuName = "Scriptable Objects/EventChannelSO")]
public class EventChannelSO : ScriptableObject
{
    private List<EventListener> listeners = new List<EventListener>();

    public void RegisterListener(EventListener listener)
    {
        listeners.Add(listener);
    }

    public void UnRegisterListener(EventListener listener)
    {
        listeners.Remove(listener);
    }

    public void RaiseEvent()
    {
        foreach (var listener in listeners)
        {
            listener.OnEventRaised();
        }
    }
}
