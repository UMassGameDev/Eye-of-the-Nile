using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/** \brief
Reads and writes the DataManager's data, using SerializedData to format it.

Documentation updated 2/3/2025
\author Stephen Nuttall
*/
class FileReadWrite
{
    /// Name of the file to read and write from.
    string fileName;

    /// Constructor. To create a FileReadWrite object, you have to give it a file name.
    public FileReadWrite(string saveFileName)
    {
        fileName = saveFileName;
    }

    /// Writes data from the DataManager to a file.
    public void WriteData(DataManager dataManager)
    {
        BinaryFormatter formatter = new();

        string path = Application.persistentDataPath + "/" + fileName;
        FileStream stream = new(path, FileMode.Create);

        SerializedData data = new(dataManager);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    /// Reads data for the DataManager from a file.
    public SerializedData ReadData()
    {
        string path = Application.persistentDataPath + "/" + fileName;
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new();
            FileStream stream = new(path, FileMode.Open);

            SerializedData data = formatter.Deserialize(stream) as SerializedData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogWarning("Could not find save file (" + fileName + "). Loading fresh save instead. Expected path: " + path);
            return null;
        }
    }
}
