using UnityEngine;

namespace RPG.UI
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField]
        GameObject DamageText;

        public void Spawn(float value)
        {
            Instantiate(DamageText, transform.position, Quaternion.identity);
        }
    }
}
