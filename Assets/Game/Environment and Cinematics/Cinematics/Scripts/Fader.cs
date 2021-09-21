using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        CanvasGroup myCanvasGroup;
        Coroutine currentFade;

        public void Awake()
        {
            myCanvasGroup = GetComponent<CanvasGroup>();
        }

        public void FadeOutImmediate()
        {
            myCanvasGroup.alpha = 1;
        }

        public Coroutine FadeOut(float time)
        {
            return Fade(0.5f, time);
        }

        IEnumerator FadeRoutine(float target, float time)
        {
            while (!Mathf.Approximately(myCanvasGroup.alpha, target))
            {
                myCanvasGroup.alpha = Mathf.MoveTowards(myCanvasGroup.alpha, target, Time.deltaTime / time);
                if (myCanvasGroup.alpha > 1) myCanvasGroup.alpha = 1;
                yield return null;
            }
        }

        public Coroutine FadeIn(float time)
        {
            return Fade(0, time);
        }

        public Coroutine Fade(float target, float time)
        {
            if (currentFade != null) StopCoroutine(currentFade);
            currentFade = StartCoroutine(FadeRoutine(target, time));
            return currentFade;
        }
    }
}
