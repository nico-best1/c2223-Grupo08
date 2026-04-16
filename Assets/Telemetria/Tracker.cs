
using System.IO;


public class Tracker
{
    static Tracker instance = null;
    const long MINIMUM_SPACE_DISK = 2L * 1024 * 1024 * 1024;
    APersistence persistenceObject;
    string sessionId;
    int eventCount;
    int fileCount = 0;

    public static void Init(string sessionId, int timeStamp, string path, bool filePersistence = true, string format = "JSON") {
        instance = new Tracker();
        instance.sessionId = sessionId;

        DriveInfo drive = new DriveInfo(Path.GetPathRoot(path));

        long freeSpace = drive.AvailableFreeSpace;

        if (filePersistence && freeSpace > MINIMUM_SPACE_DISK)
        {
            FilePersistence per = new FilePersistence();
            string filePath = path + "/telemetry_" + instance.fileCount + ".json";
            while (File.Exists(filePath))
            {
                instance.fileCount++;
                filePath = path + "/telemetry_" + instance.fileCount + ".json";
            }
            //File.CreateText(filePath);
            per.setPath(filePath);
            instance.persistenceObject = per;
        }
        else
        {
            instance.persistenceObject = null;
            return;
        }

        switch (format)
        {
            case "JSON":
                instance.persistenceObject.setSerializer(new JSONSerializer());
                break;
            default:
                instance.persistenceObject.setSerializer(new JSONSerializer());
                break;
        }

        instance.TrackEvent(new TrackerEvent("Session_Start", timeStamp));
    }

    public static void End(int timeStamp, bool flush = true) {

        instance.TrackEvent(new TrackerEvent("Session_End", timeStamp));

        if (flush)
            instance.flush();
        instance = null;
    }

    public static Tracker Instance {
        get { return instance; }
    }

    public void TrackEvent(TrackerEvent e) {
        if (persistenceObject == null)
            return;

        string eventId = "event_"+eventCount;
        e.setSessionId(sessionId);
        e.setEventId(eventId);
        persistenceObject.Send(e);
        eventCount++;
    }

    public void flush()
    {
        if(persistenceObject != null)
            persistenceObject.Flush();
    }
}
