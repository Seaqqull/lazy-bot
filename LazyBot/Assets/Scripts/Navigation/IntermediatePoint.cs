using UnityEngine;
using System;


namespace LazyBot.Navigation
{
    /// <summary>
    /// Base point, that represents some position in scene.
    /// </summary>
    public class IntermediatePoint : MonoBehaviour, IEquatable<IntermediatePoint>
    {
#pragma warning disable 0649
        [System.Serializable]
        public class IPointIdentifier : Utility.Data.Identifier<IntermediatePoint> { }

        
        /// <summary>
        /// Size of gizmo.
        /// </summary>
        [SerializeField] private float _size = 0.3f;

        [SerializeField] protected IPointIdentifier _id;

        /// <summary>
        /// Color of gizmo.
        /// </summary>
        [SerializeField] private Color _color = Color.black;
#pragma warning restore 0649

        public uint Id
        {
            get { return this._id.Id; }
        }


        private void Awake()
        {
            _id.CalculateId();
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
