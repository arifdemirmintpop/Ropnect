using UnityEngine;

namespace tiplay
{
    public enum AssistantKey { Shift, Control, Alt, Command };

    public static class AssistantKeyExtension
    {
        public static bool IsPressDown(this AssistantKey assistant)
        {
            return IsAssistantKeyDown(assistant);
        }

        private static bool IsAssistantKeyDown(AssistantKey assistantKey)
        {
            if (assistantKey == AssistantKey.Alt)
                return Event.current.alt;

            if (assistantKey == AssistantKey.Command)
                return Event.current.command;

            if (assistantKey == AssistantKey.Control)
                return Event.current.control;

            if (assistantKey == AssistantKey.Shift)
                return Event.current.shift;

            return false;
        }
    }
}