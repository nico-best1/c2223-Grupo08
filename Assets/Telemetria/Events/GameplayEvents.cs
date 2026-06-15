using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class Player_Death : TrackerEvent
{
    public string level_id;
    public string room_id;
    public float2 player_position = new float2(0, 0);
    public string cause = "";

    public Player_Death(int timeStamp, string levelId, string roomId, float2 position, string cause) : base(timeStamp)
    {
        this.level_id = levelId;
        this.room_id = roomId;
        this.player_position = position;
        this.cause = cause;
    }

    public override string ToCSV()
    {
        return $"{base.ToCSV()},{level_id},{room_id},{player_position.x},{player_position.y}, {cause}";
    }
}