
using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

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

    // Control selectivo para activacion o desactivacion de eventos en caliente
    private System.Collections.Generic.HashSet<string> disabledEvents = new System.Collections.Generic.HashSet<string>();

    public void DisableEvent(string eventType)
    {
        disabledEvents.Add(eventType);
    }

    public void EnableEvent(string eventType)
    {
        disabledEvents.Remove(eventType);
    }

    public bool IsEventEnabled(string eventType)
    {
        return !disabledEvents.Contains(eventType);
    }

    // metodo de inicializacion del tracker
    public static string Init(string sessionId, string path, bool filePersistence = true, formatType format = formatType.JSON)
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
        instance.TrackEvent(new Session_Start());

        return null;
    }

    // metodo para finalizar la sesion
    public static void End(bool flush = true)
    {

        // se registra el evento de fin de sesion
        instance.TrackEvent(new Session_End());

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

        // Filtro para ignorar eventos desactivados desde el inspector o configuracion
        if (disabledEvents.Contains(e.eventType))
            return;

        // se genera un id unico para el evento
        string eventId = "event_" + eventCount;

        // Se asignan datos comunes de forma automatica reduciendo acoplamiento
        e.setSessionId(sessionId);
        e.setEventId(eventId);
        e.setGameId("slime_escape");

        // se envia el evento al sistema de guardado
        if (persistenceObject != null)
        {
            Task.Run(() =>
            {
                persistenceObject.Send(e);
            });
        }

        // se incrementa el contador de eventos
        eventCount++;
    }

    // metodo para forzar el guardado de datos
    public void Flush()
    {
        if (persistenceObject != null)
        {
            Task.Run(() =>
            {
                persistenceObject.Flush();
            });
        }
    }
}
