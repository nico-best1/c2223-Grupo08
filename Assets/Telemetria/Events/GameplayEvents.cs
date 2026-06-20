using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class Manual_Reset : TrackerEvent
{
    public string level_id;
    public string room_id;
    public float2 player_position = new float2(0, 0);

    public Manual_Reset(string levelId, string roomId, float2 position) : base()
    {
        this.level_id = levelId;
        this.room_id = roomId;
        this.player_position = position;
    }

    public override string ToCSV()
    {
        // Para que salga con punto en vez de coma
        string px = player_position.x.ToString(System.Globalization.CultureInfo.InvariantCulture);
        string py = player_position.y.ToString(System.Globalization.CultureInfo.InvariantCulture);

        return $"{base.ToCSV()},{level_id},{room_id},{px},{py},";
    }
}

public class Player_Death : TrackerEvent
{
    public string level_id;
    public string room_id;
    public float2 player_position = new float2(0, 0);
    public string cause = "";

    public Player_Death(string levelId, string roomId, float2 position, string cause) : base()
    {
        this.level_id = levelId;
        this.room_id = roomId;
        this.player_position = position;
        this.cause = cause;
    }

    public override string ToCSV()
    {
        string px = player_position.x.ToString(System.Globalization.CultureInfo.InvariantCulture);
        string py = player_position.y.ToString(System.Globalization.CultureInfo.InvariantCulture);

        return $"{base.ToCSV()},{level_id},{room_id},{px},{py},{cause}";
    }
}