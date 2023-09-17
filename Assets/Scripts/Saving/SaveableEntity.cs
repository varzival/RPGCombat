using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace RPG.Saving
{
    [ExecuteAlways]
    public class SaveableEntity : MonoBehaviour
    {
        [SerializeField]
        string uuid = "";
        public string UUID
        {
            get { return uuid; }
        }

        static Dictionary<string, SaveableEntity> saveableEntitiyLookup =
            new Dictionary<string, SaveableEntity>();

#if UNITY_EDITOR
        private void Awake()
        {
            if (Application.IsPlaying(gameObject))
                return;

            // dont generate uuids for prefabs
            if (gameObject.scene.buildIndex < 0)
                return;

            SerializedObject obj = new SerializedObject(this);
            SerializedProperty property = obj.FindProperty("uuid");

            // avoids resetting uuid between loading scenes
            if (
                property.stringValue != ""
                && saveableEntitiyLookup.ContainsKey(property.stringValue)
                && saveableEntitiyLookup[property.stringValue] == null
            )
            {
                saveableEntitiyLookup.Remove(uuid);
            }

            if (property.stringValue == "" || !IsUnique(property.stringValue))
            {
                property.stringValue = System.Guid.NewGuid().ToString();
                obj.ApplyModifiedProperties();
            }
        }

        private void Update()
        {
            if (!saveableEntitiyLookup.ContainsKey(uuid))
            {
                saveableEntitiyLookup.Add(uuid, this);
            }
        }
#endif

        public bool IsUnique(string id)
        {
            if (!saveableEntitiyLookup.ContainsKey(id))
                return true;
            else if (saveableEntitiyLookup[id] != this)
            {
                saveableEntitiyLookup.Remove(id);
                return true;
            }
            return saveableEntitiyLookup[id] == this;
        }

        public object CaptureState()
        {
            Dictionary<string, object> state = new Dictionary<string, object>();
            foreach (ISaveable saveable in GetComponents<ISaveable>())
            {
                state[saveable.GetType().ToString()] = saveable.CaptureState();
            }
            return state;
        }

        public void RestoreState(object state)
        {
            Dictionary<string, object> dict = (Dictionary<string, object>)state;
            foreach (ISaveable saveable in GetComponents<ISaveable>())
            {
                string key = saveable.GetType().ToString();
                if (dict.ContainsKey(key))
                    saveable.RestoreState(dict[key]);
            }
        }

        public JToken CaptureAsJtoken()
        {
            JObject state = new JObject();
            IDictionary<string, JToken> stateDict = state;
            foreach (IJSONSaveable jsonSaveable in GetComponents<IJSONSaveable>())
            {
                JToken token = jsonSaveable.CaptureStateAsJToken();
                string component = jsonSaveable.GetType().ToString();
                Debug.Log($"{name} Capture {component} = {token}");
                stateDict[jsonSaveable.GetType().ToString()] = token;
            }
            return state;
        }

        public void RestoreFromJToken(JToken s)
        {
            JObject state = s.ToObject<JObject>();
            IDictionary<string, JToken> stateDict = state;
            foreach (IJSONSaveable jsonSaveable in GetComponents<IJSONSaveable>())
            {
                string component = jsonSaveable.GetType().ToString();
                if (stateDict.ContainsKey(component))
                {
                    Debug.Log($"{name} Restore {component} => {stateDict[component]}");
                    jsonSaveable.RestoreStateFromJToken(stateDict[component]);
                }
            }
        }
    }
}
