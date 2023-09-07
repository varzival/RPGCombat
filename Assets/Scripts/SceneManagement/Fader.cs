using System;
using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        CanvasGroup canvasGroup;

        // avoids issues when starting a coroutine while the other one is running
        Coroutine currentCoroutine = null;

        public event Action<float> SetAlpha;

        private void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 1;
        }

        public IEnumerator FadeOut(float time)
        {
            yield return Fade(1f, time);
        }

        public IEnumerator FadeIn(float time)
        {
            yield return Fade(0f, time);
        }

        private IEnumerator Fade(float targetAlpha, float time)
        {
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            currentCoroutine = StartCoroutine(FadeRoutine(targetAlpha, time));
            yield return currentCoroutine;
        }

        private IEnumerator FadeRoutine(float targetAlpha, float time)
        {
            while (!Mathf.Approximately(canvasGroup.alpha, targetAlpha))
            {
                float deltaAlpha = Time.deltaTime / time;
                canvasGroup.alpha += deltaAlpha * Mathf.Sign(targetAlpha - canvasGroup.alpha);
                SetAlpha?.Invoke(canvasGroup.alpha);
                yield return null;
            }
        }
    }
}
