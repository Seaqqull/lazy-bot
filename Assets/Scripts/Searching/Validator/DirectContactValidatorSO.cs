using LazyBot.Area.Detection;
using LazyBot.Area.Searching;
using UnityEngine;

[CreateAssetMenu(menuName = "Validator/Direct")]
public class DirectContactValidatorSO : DetectionValidatorSO
{
    public override bool Validate(SearchingArea searchingArea, DetectionArea detectionArea)
    {        
        Vector3 positionWithOffset = searchingArea.Socket.position + searchingArea.Data.Offset;
        Vector3 vectorSubtraction = detectionArea.transform.position - positionWithOffset;

        return !(Physics.Raycast(positionWithOffset, vectorSubtraction.normalized, vectorSubtraction.magnitude, searchingArea.Data.ObstacleMask));
    }
}
