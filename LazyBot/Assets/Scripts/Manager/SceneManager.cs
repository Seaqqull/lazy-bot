using UnityEngine;

namespace LazyBot.Manager
{
    public class SceneManager : MonoBehaviour
    {
        [Range(0, 4)] public int vSyncCount = 0;
        public int MaxFPS = 30;

        void Start()
        {
#if UNITY_EDITOR
            QualitySettings.vSyncCount = vSyncCount;
            Application.targetFrameRate = MaxFPS;
#endif
        }
    }
}