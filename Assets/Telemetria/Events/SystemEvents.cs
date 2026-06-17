using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class Session_Start : TrackerEvent
{
    public Session_Start() : base() { }
}

public class Session_End : TrackerEvent
{
    public Session_End() : base() { }
}

public class Level_Start : TrackerEvent
{
    public string level_id;

    public Level_Start(string levelId) : base()
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

    public Level_Complete(string levelId) : base()
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

    public Room_Start(string levelId, string roomId) : base()
    {
        this.level_id = levelId;
        this.room_id = roomId;
    }

    public override string ToCSV()
    {
        return $"{base.ToCSV()},{level_id},{room_id}";
    }
}

public class Room_Complete : TrackerEvent
{
    public string level_id;
    public string room_id;
    public float2 player_position = new float2(0, 0);
    public bool reset = false;

    public Room_Complete(string levelId, string roomId, float2 position, bool reset) : base()
    {
        this.level_id = levelId;
        this.room_id = roomId;
        player_position = position;
        this.reset = reset;
    }

    public override string ToCSV()
    {
        return $"{base.ToCSV()},{level_id},{room_id},{player_position.x},{player_position.y},{reset}";
    }
}