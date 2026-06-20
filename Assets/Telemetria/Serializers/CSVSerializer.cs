
public class CSVSerializer : ISerializer
{
    public string serialize(TrackerEvent e) {  return e.ToCSV(); }
    public string getHeader() { return "eventType,sessionId,eventId,gameId,timeStamp,level_id,room_id,pos_x,pos_y,cause"; }

}
