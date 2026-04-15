
public abstract class APersistence
{
    protected const int MaxBuffer = 50;
    protected int index;
    protected TrackerEvent[] events = new TrackerEvent[MaxBuffer];
    protected ISerializer serializer;

    public void setSerializer(ISerializer s)
    {
        serializer = s;
    }

    public void Send(TrackerEvent e)
    {
        if (index < MaxBuffer - 1)
            index++;
        else
            index = 0;
        events[index] = e;
    }

    public abstract void Flush();
}
