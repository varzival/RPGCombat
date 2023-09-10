using RPG.Saving;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour, ISaveable
    {
        bool played = false;

        public object CaptureState()
        {
            return played;
        }

        public void RestoreState(object state)
        {
            played = (bool)state;
        }

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
