using System.Collections.Generic;
using UnityEngine;

namespace LazyBot.Area.Data
{
    public enum ObservationType
    {
        Undefined,
        Sonar,
        Sound,
        View
    }

    public enum HitAreaState
    {
        Unknown,
        Enabled,
        Found,
        Disabled
    }    

    /// <summary>
    /// Area characteristics.
    /// </summary>
    [System.Serializable]
    public class AreaData
    {
        /// <summary>
        /// Socket designating standard position. By default gameObject.
        /// </summary>
        [SerializeField] private Transform _socket;
        /// <summary>
        /// Offset position relative to socket.
        /// </summary>
        [SerializeField] private Vector3 _offset;
        /// <summary>
        /// Rotation relative to socket.
        /// </summary>
        [SerializeField] private Vector3 _rotation;

        /// <summary>
        /// Radius of detection.
        /// </summary>
        [SerializeField] [Range(0, 500)] private float _radius;
        /// <summary>
        /// Field of view detection.
        /// </summary>
        [SerializeField] [Range(0, 360)] private float _angle;
        
        /// <summary>
        /// Layer that represents obstacles, which can prevent detection of targets.
        /// </summary>
        [SerializeField] private LayerMask _obstacleMask;
        /// <summary>
        /// Layer that represents targets.
        /// </summary>
        [SerializeField] private LayerMask _targetMask;
        /// <summary>
        /// Tags, which used to search targets.
        /// </summary>
        [SerializeField] [TagSelector] private string[] _enemyTags;


        private IReadOnlyList<string> _enemyTagsRestricted;


        public IReadOnlyList<string> EnemyTags
        {
            get
            {
                return this._enemyTagsRestricted ??
                    (this._enemyTagsRestricted = this._enemyTags);
            }
        }
        public LayerMask ObstacleMask
        {
            get
            {
                return this._obstacleMask;
            }

            set
            {
                this._obstacleMask = value;
            }
        }
        public LayerMask TargetMask
        {
            get
            {
                return this._targetMask;
            }

            set
            {
                this._targetMask = value;
            }
        }        
        public Transform Socket
        {
            get { return this._socket; }
            set { this._socket = value; }
        }
        public Vector3 Rotation
        {
            get
            {
                return this._rotation;
            }
            set
            {
                this._rotation = value;
            }
        }
        public Vector3 Offset
        {
            set
            {
                this._offset = value;
            }
            get
            {
                return this._offset;
            }
        }
        public float Radius
        {
            get
            {
                return this._radius;
            }

            set
            {
                this._radius = value;
            }
        }
        public float Angle
        {
            get
            {
                return this._angle;
            }

            set
            {
                this._angle = value;
            }
        }

    }
}