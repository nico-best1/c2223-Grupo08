using System.Collections.Generic;

public class CircularArray
{
    private const int MaxBuffer = 50;
    private Queue<TrackerEvent> queue = new Queue<TrackerEvent>(MaxBuffer);

    /// <summary>
    /// Añade un evento a la cola para luego guardalo por persistencia.
    /// </summary>
    /// <param name="e">Evento a añadir</param>
    /// <returns>0 si se ha añadido. 1 si se ha sobreescrito un elemento. -1 Error</returns>
    public int addEvent(TrackerEvent e)
    {
        if (e == null)
            return -1;

        int result = 0;
        if (queue.Count == MaxBuffer)
        {
            // descarta el mas viejo
            queue.Dequeue();
            result = 1;
        }

        queue.Enqueue(e);
        return result;
    }

    public List<string> getEvents(ISerializer serializer)
    {
        List<string> serializedEvents = new List<string>();
        while (queue.Count > 0)
            serializedEvents.Add(serializer.serialize(queue.Dequeue()));
        return serializedEvents;
    }
}
