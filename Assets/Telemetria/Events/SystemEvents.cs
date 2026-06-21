using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class Session_Start : TrackerEvent
{
    public Session_Start() : base() { }
    public override string ToCSV() { return $"{base.ToCSV()},,,,,"; }
}

public class Session_End : TrackerEvent
{
    public Session_End() : base() { }
    public override string ToCSV() { return $"{base.ToCSV()},,,,,"; }
}

public class Level_Start : TrackerEvent
{
    public string level_id;

    public Level_Start(string levelId) : base()
    {
        this.level_id = levelId;
    }

    public override string ToCSV() { return $"{base.ToCSV()},{level_id},,,,"; }
}

public class Level_Complete : TrackerEvent
{
    public string level_id;

    public Level_Complete(string levelId) : base()
    {
        this.level_id = levelId;
    }

    public override string ToCSV() { return $"{base.ToCSV()},{level_id},,,,"; }
}