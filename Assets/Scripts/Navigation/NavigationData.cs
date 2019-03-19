using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LazyBot.Navigation.Data
{
    /// <summary>
    /// Represents point belonging to a certain group.
    /// </summary>
    public enum PointType
    {
        Undefined,
        Path,
        Suspicion,
        Target
    }

    /// <summary>
    /// Action, that will be performed on reaching the point.
    /// </summary>
    public enum PointAction
    {
        Undefined,
        Continue,
        Stop,
        Attack
    }
    
    /// <summary>
    /// Represents point of some type in scene and action, 
    /// that need to perform on reaching destination.
    /// </summary>
    [System.Serializable]
    public class NavigationPoint
    {
        [SerializeField] private PointType m_type = PointType.Path;
        [SerializeField] private PointAction m_action = PointAction.Undefined;
        [SerializeField] [Range(0, 255)] private ushort m_priority = 0;
        [SerializeField] private IntermediatePoint m_point;

        /// <summary>
        /// The delay time of the transition to the next step.
        /// </summary>
        [SerializeField] [Range(0, ushort.MaxValue)] private float m_transferDelay = 0;

        /// <summary>
        /// Minimum relative speed in zone of point's impact radius.
        /// </summary>
        [SerializeField] [Range(0.0f, 1.0f)] private float m_minImpactSpeed = 1.0f;
        /// <summary>
        /// Base animation speed without impact.
        /// </summary>
        [SerializeField] [Range(0.0f, 1.0f)] private float m_animationSpeed = 1.0f;
        /// <summary>
        /// Base movement speed without impact.
        /// </summary>
        [SerializeField] [Range(0.0f, 1.0f)] private float m_movementSpeed = 1.0f;
        /// <summary>
        /// Radius at which point is considered as reached.
        /// </summary>
        [SerializeField] [Range(0, 255)] private float m_accuracyRadius = 0.5f;
        /// <summary>
        /// Radius at which point start affect on 
        /// target movement and animation speed.
        /// </summary>
        [SerializeField] [Range(0, 255)] private float m_impactRadius = 0.0f;


        public IntermediatePoint Point
        {
            get { return this.m_point; }
            set { this.m_point = value; }
        }
        /// <summary>
        /// Radius at which point is considered as reached.
        /// </summary>
        public float AccuracyRadius
        {
            get { return this.m_accuracyRadius; }
            set { this.m_accuracyRadius = value; }
        }
        /// <summary>
        /// Minimum relative speed in zone of point's impact radius.
        /// </summary>
        public float MinImpactSpeed
        {
            get { return this.m_minImpactSpeed; }
            set { this.m_minImpactSpeed = value; }
        }
        /// <summary>
        /// Base animation speed without impact.
        /// </summary>
        public float AnimationSpeed
        {
            get { return this.m_animationSpeed; }
            set { this.m_animationSpeed = value; }
        }
        /// <summary>
        /// Transform of attached intermediate point.
        /// </summary>
        public Transform Transform
        {
            get { return this.m_point?.transform; }
        }
        /// <summary>
        /// Base movement speed without impact.
        /// </summary>
        public float MovementSpeed
        {
            get { return this.m_movementSpeed; }
            set { this.m_movementSpeed = value; }
        }
        /// <summary>
        /// The delay time of the transition to the next step.
        /// </summary>
        public float TransferDelay
        {
            get { return this.m_transferDelay; }
            set { this.m_transferDelay = value; }
        }
        /// <summary>
        /// Radius at which point start affect on 
        /// target movement and animation speed.
        /// </summary>
        public float ImpactRadius
        {
            get { return this.m_impactRadius; }
            set { this.m_impactRadius = value; }
        }
        /// <summary>
        /// Action, that will be performed on reaching the point.
        /// </summary>
        public PointAction Action
        {
            get { return this.m_action; }
            set { this.m_action = value; }
        }
        public ushort Priority
        {
            get { return this.m_priority; }
            set { this.m_priority = value; }
        }
        /// <summary>
        /// Represents point belonging to a certain group.
        /// </summary>
        public PointType Type
        {
            get { return this.m_type; }
            set { this.m_type = value; }
        }


        public NavigationPoint(NavigationPoint point)
        {
            this.m_transferDelay = point.m_transferDelay;
            this.m_minImpactSpeed = point.m_minImpactSpeed;
            this.m_type = point.m_type;
            this.m_action = point.m_action;
            this.m_priority = point.m_priority;
            this.m_animationSpeed = point.m_animationSpeed;
            this.m_movementSpeed = point.m_movementSpeed;
            this.m_accuracyRadius = point.m_accuracyRadius;
            this.m_impactRadius = point.m_impactRadius;
            this.m_point = point.m_point;
        }

        /// <summary>
        /// Destroys gameObject of attached intermediate point.
        /// <remarks>
        /// Only suspicion or other, which are used only by 
        /// one instance of navigation class.
        /// </remarks>
        /// </summary>
        public void DestroyPoint()
        {
            if ((m_point) &&
                (m_type == PointType.Suspicion))
                Object.Destroy(m_point.gameObject);
        }
        
    }
}
