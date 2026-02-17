using UnityEngine;


namespace tiplay.SceneTransitionKit
{
    //[CreateAssetMenu(menuName = "Transition Settings")]
    public class TransitionSettings : ScriptableObject
    {
        [SerializeField] private TransitionCanvas transitionCanvas;

        public static TransitionCanvas TransitionCanvas => GetInstance().transitionCanvas;
        public static float FadeTime => GameSettings.GetInstance().transitionFadeTime;
        public static bool TransitionEnabled => !Mathf.Approximately(FadeTime, 0);

        private static TransitionSettings instance;
        private static TransitionSettings GetInstance()
        {
            instance ??= Resources.Load<TransitionSettings>(nameof(TransitionSettings));
            return instance;
        }
    }
}