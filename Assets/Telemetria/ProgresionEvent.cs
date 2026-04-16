using UnityEngine;

public class ProgresionEvent : TrackerEvent
{
    public string level_id;

    public ProgresionEvent(string eventType, int timeStamp, string level_id) : base(eventType, timeStamp) { this.level_id = level_id; }

    public void setLevelId(string id) {  level_id = id; }
    public string getLevelId() { return level_id; }

    public override string ToJSON() { return JsonUtility.ToJson(this); }
}
