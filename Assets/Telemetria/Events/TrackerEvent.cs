
using UnityEngine;

public class TrackerEvent
{
    public string eventType;
    public string sessionId;
    public string eventId;
    public int timeStamp;

    public TrackerEvent(int timeStamp)
    {
        this.eventType = this.GetType().Name;
        this.timeStamp = timeStamp;
    }

    public void setSessionId(string id) { sessionId = id; }
    public void setEventId(string id) { eventId = id; }

    public virtual string ToJSON()
    {
        return JsonUtility.ToJson(this);
    }

    public virtual string ToCSV()
    {
        // Variables separadas por comas.
        return $"{eventType},{sessionId},{eventId},{timeStamp}";
    }
}
