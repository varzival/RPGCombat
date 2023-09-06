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

        public event Action<float> SetAlpha;

        private void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 1;
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
                SetAlpha?.Invoke(canvasGroup.alpha);
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
                SetAlpha?.Invoke(canvasGroup.alpha);
                yield return null;
            }
        }
    }
}
