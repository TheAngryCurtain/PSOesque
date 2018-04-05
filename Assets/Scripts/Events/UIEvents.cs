using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEvents
{
    public class SceneLoadedEvent : VSGameEvent
    {
        public Enums.eScene LoadedScene;

        public SceneLoadedEvent(Enums.eScene scene)
        {
            LoadedScene = scene;
        }
    }
}
