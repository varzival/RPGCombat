using UnityEngine;

namespace RPG.UI
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField]
        GameObject DamageText;

        public void Spawn(float value)
        {
            GameObject damageText = Instantiate(
                DamageText,
                transform.position,
                Quaternion.identity
            );
            damageText.GetComponentInChildren<DamageText>().SetTextValue(value);
        }
    }
}
