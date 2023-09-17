using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace RPG.Saving
{
    public class BinarySerializer : ISerializer<Dictionary<string, object>>
    {
        public Dictionary<string, object> CaptureState(
            int activeSceneBuildIndex,
            SaveableEntity[] saveableEntities
        )
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (SaveableEntity saveableEntity in saveableEntities)
            {
                dictionary[saveableEntity.UUID] = saveableEntity.CaptureState();
            }
            int buildIndex = activeSceneBuildIndex;
            dictionary.Add("scene", buildIndex);
            return dictionary;
        }

        public void RestoreState(
            Dictionary<string, object> state,
            SaveableEntity[] saveableEntities
        )
        {
            foreach (SaveableEntity saveableEntity in saveableEntities)
            {
                if (state.ContainsKey(saveableEntity.UUID))
                    saveableEntity.RestoreState(state[saveableEntity.UUID]);
            }
        }

        public object Deserialize(FileStream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(stream);
        }

        public void Serialize(FileStream stream, object obj)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, obj);
        }
    }
}
