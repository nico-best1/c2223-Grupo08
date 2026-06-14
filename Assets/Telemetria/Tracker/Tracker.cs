
using System;
using System.Diagnostics;
using System.IO;

// definicion de los formatos posibles para guardar los datos
public enum formatType
{
    JSON, CSV
}

// clase principal Singleton encargada de registrar eventos
public class Tracker
{
    //evitar llamadas al constructor con new
    private Tracker() { }

    //unica instancia del tracker
    private static Tracker instance = null;

    public static Tracker Instance
    {
        get { return instance; }
    }

    // objeto encargado de guardar los datos
    APersistence persistenceObject;

    // identificador de la sesion
    string sessionId;

    // contador de eventos enviados
    int eventCount;

    // metodo de inicializacion del tracker
    public static string Init(string sessionId, int timeStamp, string path, bool filePersistence = true, formatType format = formatType.JSON)
    {

        // se crea la instancia del tracker
        instance = new Tracker();
        instance.sessionId = sessionId;

        if (filePersistence)
        {
            FilePersistence per = new FilePersistence();
            string filePath = "";
            string date = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            filePath = path + "/telemetry_" + sessionId + "_" + date;

            // si el formato es json
            if (format == formatType.JSON)
            {
                // se genera un nombre de archivo unico
                filePath = filePath + ".json";
            }
            // si el formato es csv
            else if (format == formatType.CSV)
            {
                UnityEngine.Debug.Log("CSV");

                // se genera un nombre de archivo unico
                filePath = filePath + ".csv";
            }

            try
            {
                // se configura la ruta donde se guardaran los datos
                per.setPath(filePath);
            }
            catch (Exception e)
            {
                instance.persistenceObject = null;
                return e.Message;
            }

            // se asigna el sistema de persistencia
            instance.persistenceObject = per;
        }
        else
        {
            // si no hay persistencia o espacio suficiente, no se guarda nada
            instance.persistenceObject = null;
            return "persistencia en local desactivado";
        }

        // se configura el tipo de serializador segun el formato elegido
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
                return "formato no reconocible";
        }

        // se registra el evento de inicio de sesion
        instance.TrackEvent(new Session_Start(timeStamp));

        return null;
    }

    // metodo para finalizar la sesion
    public static void End(int timeStamp, bool flush = true)
    {

        // se registra el evento de fin de sesion
        instance.TrackEvent(new Session_End(timeStamp));

        // se fuerzan a guardar los datos pendientes
        if (flush)
            instance.Flush();

        // se elimina la instancia
        instance = null;
    }

    // metodo para registrar un evento
    public void TrackEvent(TrackerEvent e)
    {

        // si no hay sistema de persistencia, no hace nada
        if (persistenceObject == null)
            return;

        // se genera un id unico para el evento
        string eventId = "event_" + eventCount;

        // se asignan datos al evento
        e.setSessionId(sessionId);
        e.setEventId(eventId);

        // se envia el evento al sistema de guardado
        persistenceObject.Send(e);

        // se incrementa el contador de eventos
        eventCount++;
    }

    // metodo para forzar el guardado de datos
    public void Flush()
    {
        try
        {
            if (persistenceObject != null)
                persistenceObject.Flush();
        }
        catch(Exception e) { /* de momento se ignoran las excepciones */ }
    }
}
