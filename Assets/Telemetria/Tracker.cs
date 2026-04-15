

using System.Collections;
using System.Collections.Generic;
using System.Data;

public class Tracker
{
    static Tracker instance;
    APersistence persistenceObject;

    public static void Init(bool filePersistence = true, string format = "JSON") {
        instance = new Tracker();
        if(filePersistence)
            instance.persistenceObject = new FilePersistence();
        else
            instance.persistenceObject = new FilePersistence(); //De momento solo hay persistencia en local

        switch (format)
        {
            case "JSON":
                instance.persistenceObject.setSerializer(new JSONSerializer());
                break;
            default:
                instance.persistenceObject.setSerializer(new JSONSerializer());
                break;
        }
    }

    public static void End(bool flush = true) {
        if(flush)
            instance.persistenceObject.Flush();
    }

    public static Tracker Instance() {
        return instance;
    }

    public static void TrackEvent(TrackerEvent e, Dictionary<string, string> args) {
        instance.persistenceObject.Send(e);
    }
}
