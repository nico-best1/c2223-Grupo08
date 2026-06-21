using System.Collections.Generic;

public abstract class APersistence
{
    protected ISerializer serializer;

    public void setSerializer(ISerializer s)
    {
        serializer = s;
    }

    public ISerializer getSerializer() { return serializer; }

    public abstract void Flush(List<string> serializedEvents);

}
