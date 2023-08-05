using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour
    {
        bool played = false;

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject.tag == "Player" && !played)
            {
                GetComponent<PlayableDirector>().Play();
                played = true;
            }
        }
    }
}
