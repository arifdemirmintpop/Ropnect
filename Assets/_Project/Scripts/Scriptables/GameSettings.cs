using UnityEngine;

//[CreateAssetMenu(menuName = "Tipack Settings")]
public class GameSettings : ScriptableObject
{
    [Header("Splash Scene")]
    public float splashDuration = 2f;

    [Header("Level Settings")]
    public int tutorialLevelCount = 3;

    [Header("Transition Settings"), Min(0)]
    public float transitionFadeTime = 0;

    private static GameSettings instance;
    public static GameSettings GetInstance()
    {
        instance ??= Resources.Load<GameSettings>(nameof(GameSettings));
        return instance;
    }
}
