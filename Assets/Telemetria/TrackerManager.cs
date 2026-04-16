using UnityEngine;

public class TrackerManager : MonoBehaviour
{
    Tracker tracker;
    [SerializeField]
    bool filePersistence = true;

    void Start()
    {
        if (Tracker.Instance == null)
        {
            string path = Application.persistentDataPath;
            string sessionId = System.Guid.NewGuid().ToString();
            Tracker.Init(sessionId, (int)Time.time, path, filePersistence);
        }
        tracker = Tracker.Instance;
    }

    void OnApplicationQuit()
    {
        Tracker.End((int)Time.time);
        tracker = null;
    }
}
