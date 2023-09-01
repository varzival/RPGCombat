using System;
using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        CanvasGroup canvasGroup;

        // avoids issues when starting a coroutine while the other one is running
        bool stopFadeIn = false;
        bool stopFadeOut = false;

        public event Action<float> FadedTo;

        private void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 1;
        }

        private void FadeToAlpha(float alpha)
        {
            canvasGroup.alpha = alpha;
            FadedTo?.Invoke(alpha);
        }

        public IEnumerator FadeOut(float time)
        {
            stopFadeIn = true;
            stopFadeOut = false;
            canvasGroup.alpha = 0;
            while (canvasGroup.alpha < 1 - Mathf.Epsilon * 10)
            {
                if (stopFadeOut)
                {
                    stopFadeOut = false;
                    yield break;
                }
                float deltaAlpha = Time.deltaTime / time;
                canvasGroup.alpha += deltaAlpha;
                yield return null;
            }
        }

        public IEnumerator FadeIn(float time)
        {
            stopFadeIn = false;
            stopFadeOut = true;
            canvasGroup.alpha = 1;
            while (canvasGroup.alpha > 0 + Mathf.Epsilon * 10)
            {
                if (stopFadeIn)
                {
                    stopFadeIn = false;
                    yield break;
                }
                float deltaAlpha = Time.deltaTime / time;
                canvasGroup.alpha -= deltaAlpha;
                yield return null;
            }
        }
    }
}
