using Unity.Mathematics;
using UnityEngine;

public class ProgresionEvent2 : ProgresionEvent
{
    public string room_id;

    public float2 player_position = new float2(0,0);

    public ProgresionEvent2(string eventType, int timeStamp, string level_id, string room_id) : base(eventType, timeStamp, level_id) { this.room_id = room_id; }

    public ProgresionEvent2(string eventType, int timeStamp, string level_id, string room_id, float2 player_position) : base(eventType, timeStamp, level_id) { this.room_id = room_id; this.player_position = player_position; }

    public void setRoomId(string id) {  room_id = id; }
    public string getRoomId() { return room_id; }

    public void setPlayerPosition(float2 pos) { player_position = pos; }
    public float2 getPlayerPosition() { return player_position; }

    public override string ToJSON() { return JsonUtility.ToJson(this); }
}
