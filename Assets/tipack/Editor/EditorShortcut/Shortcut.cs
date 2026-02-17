using UnityEngine;
using System;

namespace tiplay
{
    [Serializable]
    public struct Shortcut
    {
        public AssistantKey assistant;
        public KeyCode key;

        public bool IsPressDown()
        {
            if (key == KeyCode.None)
                return false;

            if (Event.current.type != EventType.KeyDown)
                return false;

            if (!assistant.IsPressDown())
                return false;

            return Event.current.keyCode == key;
        }
    }
}

