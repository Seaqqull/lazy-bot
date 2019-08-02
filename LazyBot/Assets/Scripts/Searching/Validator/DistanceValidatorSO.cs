using LazyBot.Area.Detection;
using LazyBot.Area.Searching;
using UnityEngine;

[CreateAssetMenu(menuName = "Validator/Distance")]
public class DistanceValidatorSO : DetectionValidatorSO
{
    public override bool Validate(SearchingArea searchingArea, DetectionArea detectionArea)
    {        
        Vector3 positionWithOffset = searchingArea.Socket.position + searchingArea.Data.Offset;

        return !((detectionArea.transform.position - positionWithOffset).magnitude > searchingArea.Data.Radius);
    }
    
}
