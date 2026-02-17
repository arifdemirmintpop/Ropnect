using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace tiplay.SceneTransitionKit
{
    public class TransitionCanvas : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;

        private Coroutine transitionCoroutine;
        private Queue<TransitionRequest> requests = new Queue<TransitionRequest>();

        public bool IsTransitionActive => transitionCoroutine != null;

        public void StartTransition(Action onSceneLoadStarted, Func<bool> waitForAsyncLoading, Action allowSceneActivation, Action onSceneLoadCompleted)
        {
            if (IsTransitionActive)
            {
                requests.Enqueue(new TransitionRequest(onSceneLoadStarted, waitForAsyncLoading, allowSceneActivation, onSceneLoadCompleted));
                return;
            }
            transitionCoroutine = StartCoroutine(TransitionRoutine(onSceneLoadStarted, waitForAsyncLoading, allowSceneActivation, onSceneLoadCompleted));
        }

        IEnumerator TransitionRoutine(Action onSceneLoadStarted, Func<bool> waitForAsyncLoading, Action allowSceneActivation, Action onSceneLoadCompleted)
        {
            canvasGroup.blocksRaycasts = true;

            onSceneLoadStarted?.Invoke();

            while (!waitForAsyncLoading.Invoke())
                yield return null;

            if (TransitionSettings.TransitionEnabled)
                yield return FadeCoroutine(1, TransitionSettings.FadeTime);

            allowSceneActivation?.Invoke();
            yield return new WaitForSecondsRealtime(0.1f);

            if (TransitionSettings.TransitionEnabled)
                yield return FadeCoroutine(0, TransitionSettings.FadeTime);

            onSceneLoadCompleted?.Invoke();

            canvasGroup.blocksRaycasts = false;

            transitionCoroutine = null;

            if (requests.Count > 0)
            {
                TransitionRequest req = requests.Dequeue();
                StartTransition(req.OnSceneLoadStarted, req.WaitForAsyncLoading, req.AllowSceneActivation, req.OnSceneLoadCompleted);
            }
        }

        IEnumerator FadeCoroutine(float target, float fadeTime)
        {
            while (canvasGroup.alpha != target)
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, target, 1 / fadeTime * Time.deltaTime);
                yield return null;
            }
        }
    }
}