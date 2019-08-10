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
        [SerializeField] private PointType _type = PointType.Path;
        [SerializeField] private PointAction _action = PointAction.Undefined;
        [SerializeField] [Range(0, 255)] private ushort _priority = 0;
        [SerializeField] private IntermediatePoint _point;

        /// <summary>
        /// The delay time of the transition to the next step.
        /// </summary>
        [SerializeField] [Range(0, ushort.MaxValue)] private float _transferDelay = 0;

        /// <summary>
        /// Minimum relative speed in zone of point's impact radius.
        /// </summary>
        [SerializeField] [Range(0.0f, 1.0f)] private float _minImpactSpeed = 1.0f;
        /// <summary>
        /// Base animation speed without impact.
        /// </summary>
        [SerializeField] [Range(0.0f, 1.0f)] private float _animationSpeed = 1.0f;
        /// <summary>
        /// Base movement speed without impact.
        /// </summary>
        [SerializeField] [Range(0.0f, 1.0f)] private float _movementSpeed = 1.0f;
        /// <summary>
        /// Radius at which point is considered as reached.
        /// </summary>
        [SerializeField] [Range(0, 255)] private float _accuracyRadius = 0.5f;
        /// <summary>
        /// Radius at which point start affect on 
        /// target movement and animation speed.
        /// </summary>
        [SerializeField] [Range(0, 255)] private float _impactRadius = 0.0f;


        public IntermediatePoint Point
        {
            get { return this._point; }
            set { this._point = value; }
        }
        /// <summary>
        /// Radius at which point is considered as reached.
        /// </summary>
        public float AccuracyRadius
        {
            get { return this._accuracyRadius; }
            set { this._accuracyRadius = value; }
        }
        /// <summary>
        /// Minimum relative speed in zone of point's impact radius.
        /// </summary>
        public float MinImpactSpeed
        {
            get { return this._minImpactSpeed; }
            set { this._minImpactSpeed = value; }
        }
        /// <summary>
        /// Base animation speed without impact.
        /// </summary>
        public float AnimationSpeed
        {
            get { return this._animationSpeed; }
            set { this._animationSpeed = value; }
        }
        /// <summary>
        /// Transform of attached intermediate point.
        /// </summary>
        public Transform Transform
        {
            get { return this._point?.transform; }
        }
        /// <summary>
        /// Base movement speed without impact.
        /// </summary>
        public float MovementSpeed
        {
            get { return this._movementSpeed; }
            set { this._movementSpeed = value; }
        }
        /// <summary>
        /// The delay time of the transition to the next step.
        /// </summary>
        public float TransferDelay
        {
            get { return this._transferDelay; }
            set { this._transferDelay = value; }
        }
        /// <summary>
        /// Radius at which point start affect on 
        /// target movement and animation speed.
        /// </summary>
        public float ImpactRadius
        {
            get { return this._impactRadius; }
            set { this._impactRadius = value; }
        }
        /// <summary>
        /// Action, that will be performed on reaching the point.
        /// </summary>
        public PointAction Action
        {
            get { return this._action; }
            set { this._action = value; }
        }
        public ushort Priority
        {
            get { return this._priority; }
            set { this._priority = value; }
        }
        /// <summary>
        /// Represents point belonging to a certain group.
        /// </summary>
        public PointType Type
        {
            get { return this._type; }
            set { this._type = value; }
        }


        public NavigationPoint(NavigationPoint point)
        {
            this._transferDelay = point._transferDelay;
            this._minImpactSpeed = point._minImpactSpeed;
            this._type = point._type;
            this._action = point._action;
            this._priority = point._priority;
            this._animationSpeed = point._animationSpeed;
            this._movementSpeed = point._movementSpeed;
            this._accuracyRadius = point._accuracyRadius;
            this._impactRadius = point._impactRadius;
            this._point = point._point;
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
            if ((_point) &&
                (_type == PointType.Suspicion))
                Object.Destroy(_point.gameObject);
        }
        
    }
}
