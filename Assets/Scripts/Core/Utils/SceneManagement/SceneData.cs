using System;
using Eflatun.SceneReference;
using Services.SceneManagement.Enums;

namespace Core.Utils.SceneManagement
{
    [Serializable]
    public struct SceneData
    {
        public SceneReference scene;
        public TypeScene typeScene;
        public string Name => scene.Name;
    }
}