public static class TutorialManagement
{
    private static TutorialManager manager;

    public static void RegisterManager(TutorialManager manager)
    {
        TutorialManagement.manager = manager;
    }

    public static void PlayTutorial(TutorialType type, float duration)
    {
        manager.PlayTutorial(type, duration);
    }

    public static void CloseTutorial(TutorialType type)
    {
        manager.CloseTutorial(type);
    }
}
