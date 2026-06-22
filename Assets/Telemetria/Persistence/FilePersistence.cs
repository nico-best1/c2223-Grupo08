using System;
using System.Collections.Generic;
using System.IO;

public class FilePersistence : APersistence
{
    // espacio minimo requerido en disco (2GB)
    const long MINIMUM_SPACE_DISK = 2L * 1024 * 1024 * 1024;
    const long MINIMUM_SPACE_TO_WRITE = 100L * 1024 * 1024; // 100MB mínimo para escribir

    string path;
    StreamWriter writer;
    DriveInfo drive;

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

        bool isNewFile = !File.Exists(path);

        // se obtiene informacion del disco donde se guardaran los archivos
        drive = new DriveInfo(Path.GetPathRoot(Path.GetFullPath(p)));
        long freeSpace = drive.AvailableFreeSpace;

        if (freeSpace < MINIMUM_SPACE_DISK)
            throw new System.Exception("No hay suficiente espacio en el disco duro(2gb)");

        // Abrir el stream
        OpenStream();  

        if (isNewFile && this.serializer != null)
        {
            string header = this.serializer.getHeader();
            if (!string.IsNullOrEmpty(header))
            {
                writer.WriteLine(header);
                writer.Flush();
            }
        }
    }

    public override void Flush(List<string> serializedEvents) {
        if(serializedEvents == null)
            throw new InvalidOperationException("Se ha tratado de guardar una lista vacia.");

        if (writer == null)
            throw new InvalidOperationException("El stream no está abierto. Llama a setPath() primero.");

        // Comprobar espacio disponible antes de escribir
        if (drive.AvailableFreeSpace < MINIMUM_SPACE_TO_WRITE)
            throw new IOException("Espacio insuficiente en disco para continuar escribiendo (mínimo 100MB)");

        try
        {
            // Escribir 
            foreach (var line in serializedEvents)
            {
                writer.WriteLine(line);
            }

            // Lanzar flush sin esperar
            writer.Flush();
        }
        catch (IOException ex) when (ex.HResult == unchecked((int)0x80070070))
        {
            // Error específico de disco lleno (ERROR_DISK_FULL)
            CloseStream();
            throw new IOException("Disco lleno: no se pudieron guardar los eventos.", ex);
        }
    }

    // Liberar recursos al destruir el objeto
    ~FilePersistence()
    {
        CloseStream();
    }
}
