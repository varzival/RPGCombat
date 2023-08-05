using System.IO;

namespace RPG.Saving
{
    public interface ISerializer
    {
        void Serialize(FileStream stream, object bytes);
        object Deserialize(FileStream stream);
    }
}
