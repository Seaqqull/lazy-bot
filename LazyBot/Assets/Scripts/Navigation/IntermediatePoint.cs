using UnityEngine;
using System;


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


        public override int GetHashCode()
        {
            return this.m_id.GetHashCode();
        }

        public bool Equals(IntermediatePoint obj)
        {
            return (this.m_id == obj.m_id);
        }

        public override bool Equals(System.Object obj)
        {
            if ((obj == null) ||
                !(this.GetType().Equals(obj.GetType())))
                return false;

            return this.Equals(obj as IntermediatePoint);
        }

    }
}
