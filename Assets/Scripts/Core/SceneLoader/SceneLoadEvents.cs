using Core.Events;

namespace Core.Scenes
{
    public struct LoadSceneRequest : IEvent
    {
        public string Scene;
        public bool Additive;
        public bool ActivateOnLoad;
        public bool CancelPrevious;

        public LoadSceneRequest(string scene, bool additive = false, bool activateOnLoad = true, bool cancelPrevious = true)
        {
            Scene = scene;
            Additive = additive;
            ActivateOnLoad = activateOnLoad;
            CancelPrevious = cancelPrevious;
        }
    }

    public struct UnloadSceneRequest : IEvent
    {
        public string Scene;

        public UnloadSceneRequest(string scene)
        {
            Scene = scene;
        }
    }

    public struct ReloadActiveRequest : IEvent{}

    public struct SceneLoadStarted : IEvent
    {
        public string Scene;
        public bool Additive;

        public SceneLoadStarted(string scene, bool additive)
        {
            Scene = scene;
            Additive = additive;
        }
    }

    public struct SceneLoadProgress : IEvent
    {
        public string Scene;
        public float Progress; // 0..1

        public SceneLoadProgress(string scene, float progress)
        {
            Scene = scene;
            Progress = progress;
        }
    }

    public struct SceneLoadCompleted : IEvent
    {
        public string Scene;
        public bool Additive;

        public SceneLoadCompleted(string scene, bool additive)
        {
            Scene = scene;
            Additive = additive;
        }
    }

    public struct SceneUnloadCompleted : IEvent
    {
        public string Scene;

        public SceneUnloadCompleted(string scene)
        {
            Scene = scene;
        }
    }

    public struct SceneLoadCanceled : IEvent
    {
        public string Scene;

        public SceneLoadCanceled(string scene)
        {
            Scene = scene;
        }
    }
}
