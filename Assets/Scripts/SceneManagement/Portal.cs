using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        enum DestinationIdentifier
        {
            A,
            B,
            C
        }

        [SerializeField]
        int sceneToLoad = -1;

        [SerializeField]
        Transform spawnPoint;

        [SerializeField]
        DestinationIdentifier destination;

        [SerializeField]
        float fadeOutDuration = 2f;

        [SerializeField]
        float fadeInDuration = 1f;

        Fader fader;

        private void Start()
        {
            fader = FindObjectOfType<Fader>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                StartCoroutine(Transition());
            }
        }

        private IEnumerator Transition()
        {
            DontDestroyOnLoad(gameObject);

            yield return fader.FadeOut(fadeOutDuration);

            FindObjectOfType<SavingWrapper>().Save();

            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            Debug.Log($"Scene {sceneToLoad} loaded");

            // waiting for one frame fixes issues with player positioning
            yield return null;

            FindObjectOfType<SavingWrapper>().Load();

            Portal otherPortal = GetOtherPortal();
            if (otherPortal)
            {
                bool spawnSuccess = otherPortal.SpawnPlayer();
                Debug.Log($"spawn sucess: {spawnSuccess}");
            }
            else
                Debug.LogError("other portal not found");

            FindObjectOfType<SavingWrapper>().Save();

            yield return new WaitForSeconds(1f);
            yield return fader.FadeIn(fadeInDuration);

            Destroy(gameObject);
        }

        private Portal GetOtherPortal()
        {
            Portal[] otherPortals = GameObject.FindObjectsOfType<Portal>();
            foreach (Portal portal in otherPortals)
            {
                if (portal != this && portal.destination == destination)
                    return portal;
            }
            return null;
        }

        public bool SpawnPlayer()
        {
            GameObject player = GameObject.FindWithTag("Player");
            //player.GetComponent<NavMeshAgent>().enabled = false;
            player.transform.rotation = spawnPoint.transform.rotation;
            //player.transform.position = spawnPoint.transform.position;
            return player.GetComponent<NavMeshAgent>().Warp(spawnPoint.transform.position);
            //player.GetComponent<NavMeshAgent>().enabled = true;
            //return true;
        }
    }
}
