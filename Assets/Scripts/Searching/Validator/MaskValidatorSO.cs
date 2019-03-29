using LazyBot.Area.Detection;
using LazyBot.Area.Searching;
using UnityEngine;

[CreateAssetMenu(menuName = "Validator/Mask")]
public class MaskValidatorSO : DetectionValidatorSO
{
    public override bool Validate(SearchingArea searchingArea, DetectionArea detectionArea)
    {
        return !(((1 << detectionArea.Collider.gameObject.layer) & searchingArea.Data.TargetMask) == 0);
    }
}
