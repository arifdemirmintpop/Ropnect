using System.Collections.Generic;
using tiplay;
using UnityEngine.UI;
using System;
using UnityEngine;

namespace tiplay.SceneTransitionKit
{
    public static class TransitionManager
    {
        private static TransitionCanvas transitionCanvas;

        private static TransitionCanvas GetTransitionCanvas()
        {
            if (!transitionCanvas)
            {
                transitionCanvas = GameObject.Instantiate(TransitionSettings.TransitionCanvas);
                GameObject.DontDestroyOnLoad(transitionCanvas.gameObject);
            }

            return transitionCanvas;
        }

        public static void Play(Action onSceneLoadStarted, Func<bool> waitForAsyncLoading, Action allowSceneActivation, Action onSceneLoadCompleted)
        {
            GetTransitionCanvas().StartTransition(onSceneLoadStarted, waitForAsyncLoading, allowSceneActivation, onSceneLoadCompleted);
        }
    }
}