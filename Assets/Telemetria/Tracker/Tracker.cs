
using System;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;

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

    private readonly object lockObject = new object(); // Sincronizacion para volcado asincrono

    private CircularArray events;

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

            // se configura el tipo de serializador segun el formato elegido
            switch (format)
            {
                case formatType.JSON:
                    per.setSerializer(new JSONSerializer());
                    break;

                case formatType.CSV:
                    per.setSerializer(new CSVSerializer());
                    break;

                default:
                    instance.persistenceObject = null;
                    return "formato no reconocible";
            }

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

        instance.events = new CircularArray();

        // se registra el evento de inicio de sesion
        instance.TrackEvent(new Session_Start(), true);

        return null;
    }

    // metodo para finalizar la sesion
    public static void End(bool flush = true)
    {
        // se registra el evento de final de sesion
        instance.TrackEvent(new Session_End(), true);

        if (flush)
        {
            if (instance.persistenceObject != null)
            {
                // Flush síncrono para asegurar persistencia antes de salir
                instance.persistenceObject.Flush(instance.events.getEvents(instance.persistenceObject.getSerializer()));
            }
        }

        // se elimina la instancia
        instance = null;
    }

    // metodo para registrar un evento
    public void TrackEvent(TrackerEvent e, bool syncrono = false)
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
            if (syncrono)
            {
                // Por si se quiere hacer un track desde el hilo principal.
                // Tambien es necesario el lock porque Flush se puede lanzar desde otro hilo.
                lock (lockObject)
                {
                    events.addEvent(e);
                }
            }
            else
            {
                Task.Run(() =>
                {
                    lock (lockObject)
                    {
                        events.addEvent(e);
                    }
                });
            }
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
                lock (lockObject)
                {
                    persistenceObject.Flush(events.getEvents(persistenceObject.getSerializer()));
                }
            });
        }
    }
}
