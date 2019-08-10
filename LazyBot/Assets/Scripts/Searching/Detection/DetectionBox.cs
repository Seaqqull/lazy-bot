using UnityEngine;

namespace LazyBot.Area.Detection
{
    public class DetectionBox : DetectionArea
    {
        protected override void OnDrawGizmos()
        {
            if ((!base._colider) ||
                (base._state == Data.HitAreaState.Disabled) ||
                (base._state == Data.HitAreaState.Unknown)) return;

            Gizmos.color = base._gizmoColor;
            Gizmos.
                DrawWireCube(base._colider.bounds.center, new Vector3(base._gizmoSize, base._gizmoSize, base._gizmoSize));
        }

        protected override Collider GetCollider()
        {
            return GetComponent<BoxCollider>();
        }
    }
}
