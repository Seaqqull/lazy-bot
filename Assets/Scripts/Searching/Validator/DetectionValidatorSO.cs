using UnityEngine;

public abstract class DetectionValidatorSO : ScriptableObject
{
    public abstract bool Validate(LazyBot.Area.Searching.SearchingArea searchingArea, LazyBot.Area.Detection.DetectionArea detectionArea);
}
