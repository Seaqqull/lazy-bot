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
        [SerializeField] private float _size = 0.3f;
        /// <summary>
        /// Color of gizmo.
        /// </summary>
        [SerializeField] private Color _color = Color.black;

        /// <summary>
        /// Id to perform comparison.
        /// </summary>
        protected static uint _idCounter = 0;
        protected uint _id;


        public uint Id
        {
            get { return this._id; }
        }


        private void Awake()
        {
            _id = _idCounter++;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = _color;
            Gizmos.DrawWireSphere(transform.position, _size);
        }


        public override int GetHashCode()
        {
            return this._id.GetHashCode();
        }

        public bool Equals(IntermediatePoint obj)
        {
            return (this._id == obj._id);
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
