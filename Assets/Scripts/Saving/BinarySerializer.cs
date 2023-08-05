using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace RPG.Saving
{
    public class BinarySerializer : ISerializer
    {
        object ISerializer.Deserialize(FileStream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(stream);
        }

        void ISerializer.Serialize(FileStream stream, object obj)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, obj);
        }
    }
}
