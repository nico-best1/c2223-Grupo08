using System;
using System.IO;
using System.Threading.Tasks;

public class FilePersistence : APersistence
{
    // espacio minimo requerido en disco (2GB)
    const long MINIMUM_SPACE_DISK = 2L * 1024 * 1024 * 1024;
    const long MINIMUM_SPACE_TO_WRITE = 100L * 1024 * 1024; // 100MB mínimo para escribir

    string path;
    StreamWriter writer;
    DriveInfo drive;

    Task pendingFlush = null;

    private void OpenStream()
    {
        writer = new StreamWriter(path, append: true);
        writer.AutoFlush = false; // Controlamos el flush manualmente
    }

    private void CloseStream()
    {
        if (writer != null)
        {
            try
            {
                writer.Flush();
                writer.Close();
            }
            catch { /* ignorar errores al cerrar */ }
            finally
            {
                writer.Dispose();
                writer = null;
            }
        }
    }

    public void setPath(string p) {
        path = p;

        // Cerrar stream anterior si existía
        CloseStream();

        // se obtiene informacion del disco donde se guardaran los archivos
        drive = new DriveInfo(Path.GetPathRoot(Path.GetFullPath(p)));
        long freeSpace = drive.AvailableFreeSpace;

        if (freeSpace < MINIMUM_SPACE_DISK)
            throw new System.Exception("No hay suficiente espacio en el disco duro(2gb)");

        // Abrir el stream
        OpenStream();
    }

    public override async Task Flush() {
        if (writer == null)
            throw new InvalidOperationException("El stream no está abierto. Llama a setPath() primero.");

        // Comprobar espacio disponible antes de escribir
        if (drive.AvailableFreeSpace < MINIMUM_SPACE_TO_WRITE)
            throw new IOException("Espacio insuficiente en disco para continuar escribiendo (mínimo 100MB)");

        try
        {
            // Esperar al flush anterior antes de empezar el siguiente
            if (pendingFlush != null)
                await pendingFlush;

            for (int i = MaxBuffer + index - eventSize; i < MaxBuffer + index; i++)
            {
                writer.WriteLine(this.serializer.serialize(events[i % MaxBuffer]));
            }

            // Lanzar flush sin esperar
            pendingFlush = writer.FlushAsync();
        }
        catch (IOException ex) when (ex.HResult == unchecked((int)0x80070070))
        {
            // Error específico de disco lleno (ERROR_DISK_FULL)
            CloseStream();
            throw new IOException("Disco lleno: no se pudieron guardar los eventos.", ex);
        }
        finally {
            eventSize = 0; 
        }
    }

    // Liberar recursos al destruir el objeto
    ~FilePersistence()
    {
        CloseStream();
    }
}
