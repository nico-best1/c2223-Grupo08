using UnityEngine;

public class ProgresionEvent2 : ProgresionEvent
{
    public string room_id;

    public ProgresionEvent2(string eventType, int timeStamp, string level_id, string room_id) : base(eventType, timeStamp, level_id) { this.room_id = room_id; }

    public void setRoomId(string id) {  room_id = id; }
    public string getRoomId() { return room_id; }

    public override string ToJSON() { return JsonUtility.ToJson(this); }
}
