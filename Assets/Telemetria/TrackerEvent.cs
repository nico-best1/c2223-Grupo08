
using UnityEngine;

[System.Serializable]
struct CommonContent{
    public string sessionId;
    public string eventId;
    public int timeStamp;
}

public class TrackerEvent
{
    string eventType;
    CommonContent commonContent;

    public void setTimeStamp(int t) { commonContent.timeStamp = t; }
    public void setEventType(string type) { eventType = type; }
    public void setSessionId(string id) { commonContent.sessionId = id; }
    public void setEventId(string id) { commonContent.eventId = id; }

    public int getTimeStamp(int t) { return commonContent.timeStamp; }
    public string setEventType() { return eventType; }
    public string setSessionId() { return commonContent.sessionId; }
    public string setEventId() { return commonContent.eventId; }
    public string ToJSON() { 
        return JsonUtility.ToJson(commonContent);
    }
}
