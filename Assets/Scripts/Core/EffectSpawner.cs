using UnityEngine;

namespace RPG.Core
{
    public class EffectSpawner : MonoBehaviour
    {
        [SerializeField]
        GameObject effect;

        public void Spawn()
        {
            Instantiate(effect, transform);
        }
    }
}
