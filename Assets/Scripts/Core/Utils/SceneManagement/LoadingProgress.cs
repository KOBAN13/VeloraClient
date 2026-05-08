using System;
using UnityEngine;

namespace Core.Utils.SceneManagement
{
    public class LoadingProgress : IProgress<float>
    {
        public event Action<float> Progressed;

        private readonly float _from;
        private readonly float _to;

        public LoadingProgress(float from = 0f, float to = 1f)
        {
            _from = Mathf.Clamp01(from);
            _to = Mathf.Clamp01(to);
        }
        
        public void Report(float value)
        {
            Progressed?.Invoke(Mathf.Lerp(_from, _to, Mathf.Clamp01(value)));
        }
    }
}
