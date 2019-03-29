using UnityEngine;

public abstract class AreaTargetVisualizerSO : ScriptableObject
{
    public abstract void Visualize(LazyBot.Area.Searching.SearchingArea searchingArea, LazyBot.Area.Detection.DetectionArea detectionArea);
}
