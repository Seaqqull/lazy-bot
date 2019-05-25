using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
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
