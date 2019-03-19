using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LazyBot.Navigation
{
    /// <summary>
    /// Priority navigation. Where next point will be 
    /// point with higher priority.
    /// </summary>
    public class PriorityNearnessNavigation : NavigationContainer
    {
        /// <summary>
        /// Distance at which points with same priority are 
        /// considered as equal.
        /// </summary>
        [SerializeField] private float m_nearnessPrecision = 0.1f;

        /// <summary>
        /// Return next with higher priority or 
        /// random (based on container settings) navigation point.
        /// </summary>
        /// <param name="destinationIndex">Relative index of navigation point, updates after choosing new point.</param>
        /// <returns>Position of target navigation point.</returns>
        public override Vector3 GetDestination(ref int destinationIndex)
        {
            if (Points.Count == 0)
                throw new System.Exception("Can't get destination, navigation is empty.");

            if (m_isRandom)
                return GetRandomPoint(ref destinationIndex);

            float distanceTemp = 0.0f,
                  distance = int.MaxValue;
            ushort priority = 0;

            for (int i = 0; i < Points.Count; i++)
            {
                distanceTemp = Vector3.Distance(m_ownerTransform.position, Points[i].Transform.position);
                if ((i == m_previousPoint) ||
                    (Points[i].Priority < priority)||
                    ((Points[i].Priority == priority) && (distanceTemp - distance > m_nearnessPrecision))) continue;

                priority = Points[i].Priority;
                distance = distanceTemp;
                destinationIndex = i;
            }
            return Points[destinationIndex].Transform.position;
        }
    }
}
