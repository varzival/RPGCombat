using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Saving
{
    public class SavingSystem : MonoBehaviour
    {
        private ISerializer serializer;

        private void Start()
        {
            serializer = new BinarySerializer();
        }

        public void Save(string saveFile)
        {
            Dictionary<string, object> state = LoadFile(saveFile);
            Dictionary<string, object> currentState = CaptureState();
            currentState.ToList().ForEach(x => state[x.Key] = x.Value);
            SaveFile(saveFile, state);
        }

        public IEnumerator LoadLastScene(string saveFile)
        {
            Dictionary<string, object> dict = LoadFile(saveFile);
            int buildIndex = (int)dict["scene"];
            Debug.Log($"Loading scene with buildIndex {buildIndex}");
            if (buildIndex != SceneManager.GetActiveScene().buildIndex)
            {
                yield return SceneManager.LoadSceneAsync(buildIndex);
            }
            RestoreState(dict);
        }

        public void Load(string saveFile)
        {
            RestoreState(LoadFile(saveFile));
        }

        private void SaveFile(string saveFile, object state)
        {
            string path = GetPathFromSaveFile(saveFile);
            Debug.Log($"saving to {path}");
            using (FileStream fileStream = File.Open(path, FileMode.Create))
            {
                serializer.Serialize(fileStream, state);
            }
        }

        private Dictionary<string, object> LoadFile(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);
            Debug.Log($"loading from {path}");

            if (!File.Exists(path))
                return new Dictionary<string, object>();

            using (FileStream fileStream = File.Open(path, FileMode.Open))
            {
                return (Dictionary<string, object>)serializer.Deserialize(fileStream);
            }
        }

        private Dictionary<string, object> CaptureState()
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (SaveableEntity saveableEntity in FindObjectsOfType<SaveableEntity>())
            {
                dictionary[saveableEntity.UUID] = saveableEntity.CaptureState();
            }
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            Debug.Log($"Saving scene with buildIndex {buildIndex}");
            dictionary.Add("scene", buildIndex);
            return dictionary;
        }

        private void RestoreState(Dictionary<string, object> state)
        {
            foreach (SaveableEntity saveableEntity in FindObjectsOfType<SaveableEntity>())
            {
                if (state.ContainsKey(saveableEntity.UUID))
                    saveableEntity.RestoreState(state[saveableEntity.UUID]);
            }
        }

        [Obsolete("use SerializableVector3 instead")]
        private byte[] SerializeVector(Vector3 vector)
        {
            byte[] vectorBytes = new byte[3 * 4];
            BitConverter.GetBytes(vector.x).CopyTo(vectorBytes, 0);
            BitConverter.GetBytes(vector.y).CopyTo(vectorBytes, 4);
            BitConverter.GetBytes(vector.z).CopyTo(vectorBytes, 8);
            return vectorBytes;
        }

        [Obsolete("use SerializableVector3 instead")]
        private Vector3 DeserializeVector(byte[] byteArray)
        {
            Vector3 result = new Vector3();
            result.x = BitConverter.ToSingle(byteArray, 0);
            result.y = BitConverter.ToSingle(byteArray, 4);
            result.z = BitConverter.ToSingle(byteArray, 8);
            return result;
        }

        private string GetPathFromSaveFile(string saveFile)
        {
            return Path.Combine(Application.persistentDataPath, saveFile);
        }
    }
}
