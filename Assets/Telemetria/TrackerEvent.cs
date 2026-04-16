
using UnityEngine;

public class TrackerEvent
{
    public string eventType;
    public string sessionId;
    public string eventId;
    public int timeStamp;

    public TrackerEvent(string eventType, int timeStamp) { this.eventType = eventType; this.timeStamp = timeStamp; }

    public void setTimeStamp(int t) { timeStamp = t; }
    public void setEventType(string type) { eventType = type; }
    public void setSessionId(string id) { sessionId = id; }
    public void setEventId(string id) { eventId = id; }

    public int getTimeStamp(int t) { return timeStamp; }
    public string setEventType() { return eventType; }
    public string setSessionId() { return sessionId; }
    public string setEventId() { return eventId; }

    public virtual string ToJSON() { 
        return JsonUtility.ToJson(this);
    }
}
