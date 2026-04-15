
public class FilePersistence : APersistence
{
    public override void Flush() {
        for(int i = 0; i < MaxBuffer; i++)
        {
            events[index+i % MaxBuffer].ToJSON();
        }
    }
}
