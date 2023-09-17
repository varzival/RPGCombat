using Newtonsoft.Json.Linq;
using RPG.Saving;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour, ISaveable, IJSONSaveable
    {
        bool played = false;

        public object CaptureState()
        {
            return played;
        }

        public JToken CaptureStateAsJToken()
        {
            return JToken.FromObject(played);
        }

        public void RestoreState(object state)
        {
            played = (bool)state;
        }

        public void RestoreStateFromJToken(JToken state)
        {
            played = state.ToObject<bool>();
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
