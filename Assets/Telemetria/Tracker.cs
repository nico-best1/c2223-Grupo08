
using System;
using System.Diagnostics;
using System.IO;

public enum formatType
{
    JSON, CSV
}

public class Tracker
{
    static Tracker instance = null;
    const long MINIMUM_SPACE_DISK = 2L * 1024 * 1024 * 1024;
    APersistence persistenceObject;
    string sessionId;
    int eventCount;
    int fileCount = 0;

    public static string Init(string sessionId, int timeStamp, string path, bool filePersistence = true, formatType format = formatType.JSON) {
        instance = new Tracker();
        instance.sessionId = sessionId;

        DriveInfo drive = new DriveInfo(Path.GetPathRoot(path));

        long freeSpace = drive.AvailableFreeSpace;

        if (filePersistence && freeSpace > MINIMUM_SPACE_DISK)
        {
            FilePersistence per = new FilePersistence();
            string filePath = "";
            if (format == formatType.JSON)
            {
                filePath = path + "/telemetry_" + instance.fileCount + ".json";
                while (File.Exists(filePath))
                {
                    instance.fileCount++;
                    filePath = path + "/telemetry_" + instance.fileCount + ".json";
                }
            }
            else if (format == formatType.CSV)
            {
                UnityEngine.Debug.Log("CSV");
                filePath = path + "/telemetry_" + instance.fileCount + ".csv";
                while (File.Exists(filePath))
                {
                    instance.fileCount++;
                    filePath = path + "/telemetry_" + instance.fileCount + ".csv";
                }
            }

                //File.CreateText(filePath);
                per.setPath(filePath);
            instance.persistenceObject = per;
        }
        else
        {
            instance.persistenceObject = null;
            if (!filePersistence)
                return "Persistencia en local desactivado";
            else
                return "No hay suficiente espacio en el disco duro (2GB)";
        }

        switch (format)
        {
            case formatType.JSON:
                instance.persistenceObject.setSerializer(new JSONSerializer());
                break;
            case formatType.CSV:
                instance.persistenceObject.setSerializer(new CSVSerializer());
                break;
            default:
                instance.persistenceObject = null;
                return "Formato no reconocible";
        }

        instance.TrackEvent(new TrackerEvent("Session_Start", timeStamp));
        return null;
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
