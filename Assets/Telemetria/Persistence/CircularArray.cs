using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor.PackageManager;
public class CircularArray
{
    private const int MaxBuffer = 50;
    private int head = 0;
    private TrackerEvent[] events = new TrackerEvent[MaxBuffer];
    private int eventSize = 0;

    /// <summary>
    /// Añade un evento al array para luego guardalo por persistencia.
    /// </summary>
    /// <param name="e">Evento a añadir</param>
    /// <returns>0 si se ha añadido. 1 si se ha sobreescrito un elemento. -1 Error</returns>
    public int addEvent(TrackerEvent e)
    {
        if(e == null)
            return -1;

        int index = (head + eventSize) % MaxBuffer;

        if (eventSize == MaxBuffer)
        {
            events[head] = e;

            head = (head + 1) % MaxBuffer;

            return 1;
        }

        events[index] = e;

        eventSize++;

        return 0;
    }

    public List<string> getEvents(ISerializer serializer)
    {
        List<string> serializedEvents = new List<string>();

        for (int i = 0; i < eventSize; i++)
        {
            serializedEvents.Add(serializer.serialize(events[(i+head) % MaxBuffer]));
        }
        eventSize = 0;

        return serializedEvents;
    }
}
