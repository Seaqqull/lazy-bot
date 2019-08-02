using LazyBot.Area.Detection;
using LazyBot.Area.Searching;
using UnityEngine;

[CreateAssetMenu(menuName = "Validator/Activity")]
public class ActivityValidatorSO : DetectionValidatorSO
{
    public override bool Validate(SearchingArea searchingArea, DetectionArea detectionArea)
    {
        return !((detectionArea.State == LazyBot.Area.Data.HitAreaState.Disabled) ||
            (detectionArea.State == LazyBot.Area.Data.HitAreaState.Unknown));
    }
}
