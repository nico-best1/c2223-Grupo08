using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class Session_Start : TrackerEvent
{
    public Session_Start(int timeStamp) : base(timeStamp) { }
}

public class Session_End : TrackerEvent
{
    public Session_End(int timeStamp) : base(timeStamp) { }
}

public class Level_Start : TrackerEvent
{
    public string level_id;

    public Level_Start(int timeStamp, string levelId) : base(timeStamp)
    {
        this.level_id = levelId;
    }

    public override string ToCSV()
    {
        return $"{base.ToCSV()},{level_id}";
    }
}

public class Level_Complete : TrackerEvent
{
    public string level_id;

    public Level_Complete(int timeStamp, string levelId) : base(timeStamp)
    {
        this.level_id = levelId;
    }

    public override string ToCSV()
    {
        return $"{base.ToCSV()},{level_id}";
    }
}

public class Room_Start : TrackerEvent
{
    public string level_id;
    public string room_id;
    public float2 player_position = new float2(0, 0);

    public Room_Start(int timeStamp, string levelId, string roomId, float2 position) : base(timeStamp)
    {
        this.level_id = levelId;
        this.room_id = roomId;
        player_position = position;
    }

    public override string ToCSV()
    {
        return $"{base.ToCSV()},{level_id},{room_id},{player_position.x},{player_position.y}";
    }
}

public class Room_Complete : TrackerEvent
{
    public string level_id;
    public string room_id;
    public float2 player_position = new float2(0, 0);
    public bool reset = false;

    public Room_Complete(int timeStamp, string levelId, string roomId, float2 position, bool reset) : base(timeStamp)
    {
        this.level_id = levelId;
        this.room_id = roomId;
        player_position = position;
        this.reset = reset;
    }

    public override string ToCSV()
    {
        return $"{base.ToCSV()},{level_id},{room_id},{player_position.x},{player_position.y}, {reset}";
    }
}