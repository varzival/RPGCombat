using TMPro;
using UnityEngine;

namespace RPG.UI
{
    public class DamageText : MonoBehaviour
    {
        public void SetTextValue(float value)
        {
            GetComponentInChildren<TextMeshProUGUI>().text = value + "";
        }
    }
}
