using UnityEngine;

public class TrackerManager : MonoBehaviour
{
    Tracker tracker;
    [SerializeField]
    bool filePersistence = true;
    [SerializeField]
    string format = "JSON";

    void Start()
    {
        if (Tracker.Instance == null)
        {
            string path = Application.persistentDataPath;
            string sessionId = System.Guid.NewGuid().ToString();
            string error = Tracker.Init(sessionId, (int)Time.time, path, filePersistence, format);
            if(error != null)
                Debug.LogWarning(error);
        }
        tracker = Tracker.Instance;
    }

    void OnApplicationQuit()
    {
        Tracker.End((int)Time.time);
        tracker = null;
    }
}
