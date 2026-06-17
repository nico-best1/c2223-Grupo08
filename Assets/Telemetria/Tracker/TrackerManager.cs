using System;
using UnityEngine;

// clase que gestiona el tracker dentro de unity
public class TrackerManager : MonoBehaviour
{
    // indica si se guardaran datos en fichero
    [SerializeField]
    bool filePersistence = true;

    // formato en el que se guardaran los datos
    [SerializeField]
    formatType format = formatType.JSON;

    // lista de eventos desactivados al inicio (configurable desde el Inspector)
    [SerializeField]
    private System.Collections.Generic.List<string> disabledEventsAtStart = new System.Collections.Generic.List<string>();

    void Awake()
    {
        if (Tracker.Instance == null)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Elimina el duplicado
        }
    }

    // metodo que se ejecuta al iniciar el objeto
    void Start()
    {
        if (Tracker.Instance == null)
        {
            // ruta donde se guardaran los datos
            string path = Application.persistentDataPath;

            // se genera un id unico para la sesion
            string sessionId = System.Guid.NewGuid().ToString();

            // se inicializa el tracker
            string error = Tracker.Init(sessionId, path, filePersistence, format);

            // si hay error, se muestra por consola
            if (error != null)
                Debug.LogWarning(error);
            else
            {
                // desactivar eventos segun configuracion del inspector
                foreach (string eventType in disabledEventsAtStart)
                {
                    Tracker.Instance.DisableEvent(eventType);
                }
            }
        }
    }

    // metodo que se ejecuta al cerrar la aplicacion
    void OnApplicationQuit()
    {
        // se finaliza la sesion del tracker
        Tracker.End();
    }
}
