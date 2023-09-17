using System.IO;

namespace RPG.Saving
{
    public interface ISerializer<StateObject>
    {
        void Serialize(FileStream stream, object bytes);
        object Deserialize(FileStream stream);

        StateObject CaptureState(int activeSceneBuildIndex, SaveableEntity[] saveableEntities);

        void RestoreState(StateObject state, SaveableEntity[] saveableEntities);
    }
}
