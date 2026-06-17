using System;
using UnityEngine;

public class TrackerEvent
{
    public string eventType;
    public string sessionId;
    public string eventId;
    public string gameId;
    public long timeStamp; // Usar 64 bits para evitar desbordamiento en 2038

    public TrackerEvent()
    {
        this.eventType = this.GetType().Name;
        // Asignacion automatica del timestamp en formato Unix Epoch ms
        this.timeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    public void setSessionId(string id) { sessionId = id; }
    public void setEventId(string id) { eventId = id; }
    public void setGameId(string id) { gameId = id; }

    public virtual string ToJSON()
    {
        return JsonUtility.ToJson(this);
    }

    public virtual string ToCSV()
    {
        // Variables separadas por comas.
        return $"{eventType},{sessionId},{eventId},{gameId},{timeStamp}";
    }
}
