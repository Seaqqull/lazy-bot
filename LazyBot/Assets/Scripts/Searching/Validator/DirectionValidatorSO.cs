using LazyBot.Area.Detection;
using LazyBot.Area.Searching;
using UnityEngine;

[CreateAssetMenu(menuName = "Validator/Direction")]
public class DirectionValidatorSO : DetectionValidatorSO
{
    public override bool Validate(SearchingArea searchingArea, DetectionArea detectionArea)
    {
        Vector3 forwardRotation = Quaternion.Euler(searchingArea.Data.Rotation) * searchingArea.Socket.forward;
        Vector3 positionWithOffset = searchingArea.Socket.position + searchingArea.Data.Offset;                
        Vector3 vectorSubtraction = detectionArea.transform.position - positionWithOffset;

        return !((searchingArea.Data.Angle != 360) &&
                        (Vector2.Angle(new Vector2(vectorSubtraction.x, vectorSubtraction.z), new Vector2(forwardRotation.x, forwardRotation.z)) > searchingArea.Data.Angle / 2));
    }    
}
