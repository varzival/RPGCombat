using System.Collections;
using RPG.Saving;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        [SerializeField]
        float fadeInDuration = 3f;
        const string defaultSaveFile = "save.json";

        private void Start()
        {
            StartCoroutine(LoadOnStart());
        }

        private IEnumerator LoadOnStart()
        {
            yield return GetComponent<JSONSavingSystem>().LoadLastScene(defaultSaveFile);
            yield return FindObjectOfType<Fader>().FadeIn(fadeInDuration);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                Delete();
            }
        }

        public void Load()
        {
            GetComponent<JSONSavingSystem>().Load(defaultSaveFile);
        }

        public void Save()
        {
            GetComponent<JSONSavingSystem>().Save(defaultSaveFile);
        }

        public void Delete()
        {
            GetComponent<JSONSavingSystem>().Delete(defaultSaveFile);
        }
    }
}
