

public class ProgresionEvent : TrackerEvent
{
    string level_id;

    public void setLevelId(string id) {  level_id = id; }
    public string getLevelId() { return level_id; }

    public string ToJSON() { return ToJSON(); }
}
