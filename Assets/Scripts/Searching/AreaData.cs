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
        [SerializeField] private Transform m_socket;
        /// <summary>
        /// Offset position relative to socket.
        /// </summary>
        [SerializeField] private Vector3 m_offset;
        /// <summary>
        /// Rotation relative to socket.
        /// </summary>
        [SerializeField] private Vector3 m_rotation;

        /// <summary>
        /// Radius of detection.
        /// </summary>
        [SerializeField] [Range(0, 500)] private float m_radius;
        /// <summary>
        /// Field of view detection.
        /// </summary>
        [SerializeField] [Range(0, 360)] private float m_angle;
        
        /// <summary>
        /// Layer that represents obstacles, which can prevent detection of targets.
        /// </summary>
        [SerializeField] private LayerMask m_obstacleMask;
        /// <summary>
        /// Layer that represents targets.
        /// </summary>
        [SerializeField] private LayerMask m_targetMask;
        /// <summary>
        /// Tags, which used to search targets.
        /// </summary>
        [SerializeField] [TagSelector] private string[] m_enemyTags;
        
        public LayerMask ObstacleMask
        {
            get
            {
                return this.m_obstacleMask;
            }

            set
            {
                this.m_obstacleMask = value;
            }
        }
        public LayerMask TargetMask
        {
            get
            {
                return this.m_targetMask;
            }

            set
            {
                this.m_targetMask = value;
            }
        }
        public string[] EnemyTags
        {
            get
            {
                return this.m_enemyTags;
            }
            set
            {
                this.m_enemyTags = value;
            }
        }
        public Transform Socket
        {
            get { return this.m_socket; }
            set { this.m_socket = value; }
        }
        public Vector3 Rotation
        {
            get
            {
                return this.m_rotation;
            }
            set
            {
                this.m_rotation = value;
            }
        }
        public Vector3 Offset
        {
            set
            {
                this.m_offset = value;
            }
            get
            {
                return this.m_offset;
            }
        }
        public float Radius
        {
            get
            {
                return this.m_radius;
            }

            set
            {
                this.m_radius = value;
            }
        }
        public float Angle
        {
            get
            {
                return this.m_angle;
            }

            set
            {
                this.m_angle = value;
            }
        }

    }
}