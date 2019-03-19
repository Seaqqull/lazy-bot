using System;
using UnityEngine;

namespace LazyBot.Navigation
{
    /// <summary>
    /// Base point, that represents some position in scene.
    /// </summary>
    public class IntermediatePoint : MonoBehaviour, IEquatable<IntermediatePoint>
    {
        /// <summary>
        /// Size of gizmo.
        /// </summary>
        [SerializeField] private float m_size = 0.3f;
        /// <summary>
        /// Color of gizmo.
        /// </summary>
        [SerializeField] private Color m_color = Color.black;

        /// <summary>
        /// Id to perform comparison.
        /// </summary>
        protected static uint m_idCounter = 0;
        protected uint m_id;


        public uint Id
        {
            get { return this.m_id; }
        }


        private void Awake()
        {
            m_id = m_idCounter++;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = m_color;
            Gizmos.DrawWireSphere(transform.position, m_size);
        }

        /// <summary>
        /// Compare two points by their ids.
        /// </summary>
        /// <param name="other">Other object to compare.</param>
        /// <returns>Are objects the same.</returns>
        public bool Equals(IntermediatePoint other)
        {
            return (this.m_id == other.m_id);
        }
    }
}