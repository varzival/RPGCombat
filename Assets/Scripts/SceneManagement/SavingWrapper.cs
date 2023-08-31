using System.Collections;
using RPG.Saving;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        [SerializeField]
        float fadeInDuration = 3f;
        const string defaultSaveFile = "save.sav";

        private void Start()
        {
            StartCoroutine(LoadOnStart());
        }

        private IEnumerator LoadOnStart()
        {
            yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);
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
            GetComponent<SavingSystem>().Load(defaultSaveFile);
        }

        public void Save()
        {
            GetComponent<SavingSystem>().Save(defaultSaveFile);
        }

        public void Delete()
        {
            GetComponent<SavingSystem>().Delete(defaultSaveFile);
        }
    }
}
