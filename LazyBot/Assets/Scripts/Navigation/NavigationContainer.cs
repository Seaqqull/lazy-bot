using System.Collections.Generic;
using UnityEngine;

namespace LazyBot.Navigation
{
    /// <summary>
    /// Base container for navigation points.
    /// </summary>
    public abstract class NavigationContainer : MonoBehaviour
    {
        /// <summary>
        /// Socket of target, that used to calculate movement and animation speed.
        /// </summary>
        [SerializeField] protected Transform _ownerTransform;

        [SerializeField] protected List<LazyBot.Navigation.Data.NavigationPoint> _points = 
            new List<LazyBot.Navigation.Data.NavigationPoint>();

        /// <summary>
        /// Is next destination point random.
        /// </summary>
        [SerializeField] protected bool _isRandom = false;
        [SerializeField] protected int _startupPoint = 0;
        
        [SerializeField] protected Color _impactColor = Color.yellow;
        [SerializeField] protected Color _accuracyColor = Color.red;
        [SerializeField] protected Color _lineColor = Color.green;


        protected IReadOnlyList<LazyBot.Navigation.Data.NavigationPoint> _pointsRestricted;


        public IReadOnlyList<LazyBot.Navigation.Data.NavigationPoint> Points
        {
            get
            {
                return this._pointsRestricted ??
                    (this._pointsRestricted = this._points);
            }
        }
        /// <summary>
        /// Destination navigation point.
        /// </summary>
        public LazyBot.Navigation.Data.NavigationPoint DestinationPoint
        {
            get
            {
                if (Points.Count == 0)
                    throw new System.Exception("Can't get destination point, navigation is empty.");
                return Points[_previousPoint];
            }
        }
        /// <summary>
        /// Destination position.
        /// </summary>
        public Vector3 PointPosition
        {
            get
            {
                if (Points.Count == 0)
                    throw new System.Exception("Can't get destination position, navigation is empty.");
                return Points[_previousPoint].Transform.position;
            }
        }
        /// <summary>
        /// Is next destination point random.
        /// </summary>
        public bool IsRandom
        {
            get { return this._isRandom; }
            set { this._isRandom = value; }
        }
        public int Length
        {
            get { return Points.Count; }
        }

        /// <summary>        
        /// Current destination point.
        /// </summary>
        protected int _previousPoint = -1;
        /// <summary>
        /// Next point, that will be setted as destination, 
        /// after reaching current destination point + time delay.
        /// </summary>
        protected int _destinationPoint;


        protected virtual void Awake()
        {
            if (_ownerTransform == null)
                _ownerTransform = GetComponent<Transform>();
            _destinationPoint = _startupPoint;
        }

        protected virtual void OnDrawGizmosSelected()
        {
#if UNITY_EDITOR
            if (Points.Count == 0) return;

            Gizmos.color = _lineColor;

            int i;
            for (i = 0; i < Points.Count - 1; i++)
            {
                if ((Points[i].Transform != null) &&
                   (Points[i + 1].Transform != null))
                {
                    Gizmos.DrawLine(Points[i].Transform.position,
                        Points[i + 1].Transform.position);

                    UnityEditor.Handles.color = _impactColor;
                    UnityEditor.Handles.DrawWireArc(Points[i].Transform.position,
                        Vector3.up, Vector3.forward, 360, Points[i].ImpactRadius);

                    UnityEditor.Handles.color = _accuracyColor;
                    UnityEditor.Handles.DrawWireArc(Points[i].Transform.position,
                        Vector3.up, Vector3.forward, 360, Points[i].AccuracyRadius);
                }
                else
                    throw new System.Exception(string.Format("Point reference {0} or {1} does not exist.", i, i + 1));
            }

            if ((Points.Count == 1) || (!Points[i].Point))
                throw new System.Exception(string.Format("Point reference {0} does not exist.", i));

            UnityEditor.Handles.color = _impactColor;
            UnityEditor.Handles.DrawWireArc(Points[i].Transform.position,
                Vector3.up, Vector3.forward, 360, Points[i].ImpactRadius);
            UnityEditor.Handles.color = _accuracyColor;
            UnityEditor.Handles.DrawWireArc(Points[i].Transform.position,
                Vector3.up, Vector3.forward, 360, Points[i].AccuracyRadius);
#endif
        }

        /// <summary>
        /// Clears navigation and reset its destination point to zero.
        /// </summary>
        public void Clear()
        {
            _points.Clear();
            ResetToZero();
        }

        /// <summary>
        /// Resets destination point to zero.
        /// </summary>
        public void ResetToZero()
        {
            _destinationPoint = 0;
            _previousPoint = -1;
        }

        /// <summary>
        /// Removes navigation point.
        /// </summary>
        /// <param name="index">Positional index.</param>
        public void RemoveAt(int index)
        {
            if ((index < 0) ||
                (Points.Count == 0) ||
                (index >= Points.Count))
                throw new System.Exception(string.Format("No such {0} index to be removed.", index));

            Points[index].DestroyPoint();
            _points.RemoveAt(index);
        }

        /// <summary>
        /// Removes navigation point.
        /// </summary>
        /// <param name="index">Positional index.</param>
        /// <param name="relIndex">Relative index, updates after deletion.</param>
        public void RemoveAt(int index, ref int relIndex)
        {
            if ((index < 0) ||
                (Points.Count == 0) ||
                (index >= Points.Count))
                throw new System.Exception(string.Format("No such {0} index to be removed.", index));

            Points[index].DestroyPoint();
            _points.RemoveAt(index);

            if (index == relIndex)
                relIndex = -1;
            else if (index < relIndex)
                relIndex--;
        }

        /// <summary>
        /// Returns position of navigation point.
        /// </summary>
        /// <param name="index">Positional index.</param>
        /// <returns>Position in scene.</returns>
        public Vector3 GetPathPosition(int index)
        {
            if ((index < 0) ||
                (Points.Count == 0) ||
                (index >= Points.Count))
                throw new System.Exception(string.Format("Can't get {0} navigation position, no such navigation point.", index));

            return Points[index].Transform.position;
        }

        /// <summary>
        /// Returns navigation point.
        /// </summary>
        /// <param name="index">Positional index.</param>
        /// <returns>Navigation point.</returns>
        public LazyBot.Navigation.Data.NavigationPoint GetPoint(int index)
        {
            if ((index < 0) ||
                (Points.Count == 0) ||
                (index >= Points.Count))
                throw new System.Exception(string.Format("Can't get {0} navigation point, no such navigation point.", index));

            return Points[index];
        }

        /// <summary>
        /// Search for the nearest navigation point.
        /// </summary>
        /// <param name="destination">Relative index of nearest navigation point, updates after finding nearest point.</param>
        /// <returns>Position in scene.</returns>
        public Vector3 GetNearestPoint(ref int destination)
        {
            if (Points.Count == 0)
                throw new System.Exception("Can't get nearest point position, navigation is empty.");

            float distanceMin = int.MaxValue;
            float distanceTemp = 0.0f;

            for (int i = 0; i < Points.Count; i++)
            {
                distanceTemp = Vector3.Distance(_ownerTransform.position,
                    Points[i].Transform.position);
                if (distanceTemp < distanceMin)
                {
                    distanceMin = distanceTemp;
                    destination = i;
                }
            }

            _previousPoint = destination;
            _destinationPoint = (_previousPoint + 1) % Points.Count;

            return Points[destination].Transform.position;
        }

        /// <summary>
        /// Searches for the nearest navigation point.
        /// </summary>
        /// <param name="destination">Relative index of nearest navigation point, updates after finding nearest point.</param>
        /// <param name="position">Position from which performs calculation of distance.</param>
        /// <returns>Position in scene</returns>
        public Vector3 GetNearestPoint(ref int destination, Vector3 position)
        {
            if (Points.Count == 0)
                throw new System.Exception("Can't get nearest point position, navigation is empty.");

            float distanceMin = int.MaxValue;
            float distanceTemp = 0.0f;

            for (int i = 0; i < Points.Count; i++)
            {
                distanceTemp = Vector3.Distance(position,
                    Points[i].Transform.position);
                if (distanceTemp < distanceMin)
                {
                    distanceMin = distanceTemp;
                    destination = i;
                }
            }

            _previousPoint = destination;
            _destinationPoint = (_previousPoint + 1) % Points.Count;

            return Points[destination].Transform.position;
        }

        /// <summary>
        /// Updates destination point.
        /// </summary>
        /// <param name="destinationIndex">Relative index of navigation point, updates after choosing new destination.</param>
        /// <returns>Position of next navigation point.</returns>
        public Vector3 GetNextPoint(ref int destinationIndex)
        {
            if (Points.Count == 0)
                throw new System.Exception("Can't get next point, navigation is empty.");

            Vector3 destination = Points[_destinationPoint].Transform.position;
            destinationIndex = _destinationPoint;

            _previousPoint = _destinationPoint;
            _destinationPoint = (_destinationPoint + 1) % Points.Count;

            return destination;
        }

        /// <summary>
        /// Returns random navigation point.
        /// </summary>
        /// <param name="destinationIndex">Relative index of navigation point, updates after choosing new destination.</param>
        /// <returns>Position in scene.</returns>
        public Vector3 GetRandomPoint(ref int destinationIndex)
        {
            if (Points.Count == 0)
                throw new System.Exception("Can't get random point, navigation is empty.");

            int randomPoint;

            do
            {                
                randomPoint = Random.Range(0, Points.Count);
            }
            while ((randomPoint == _previousPoint) ||
                   (randomPoint == _destinationPoint));


            Vector3 destination = Points[_destinationPoint].Transform.position;
            destinationIndex = _destinationPoint;
            //_prePreviousPoint = _previousPoint;
            _previousPoint = _destinationPoint;
            _destinationPoint = randomPoint;

            return destination;
        }

        /// <summary>
        /// Adds navigation point.
        /// </summary>
        /// <param name="point">Point to add.</param>
        public void Add(LazyBot.Navigation.Data.NavigationPoint point)
        {
            _points.Add(point);
        }

        /// <summary>
        /// Adds list of navigation point.
        /// </summary>
        /// <param name="points">List of navigation point.</param>
        public void Add(List<LazyBot.Navigation.Data.NavigationPoint> points)
        {
            for (int i = 0; i < Points.Count; i++)
            {
                _points.Add(points[i]);
            }
            points.Clear();
        }

        /// <summary>
        /// Adds navigation point.
        /// </summary>
        /// <param name="point">Point to add.</param>
        /// <param name="index">Relative index of added navigation point.</param>
        public void Add(LazyBot.Navigation.Data.NavigationPoint point, ref int index)
        {
            index = Points.Count;
            _points.Add(point);
        }

        /// <summary>
        /// Calculates target's speed on path, based on its position.
        /// </summary>
        /// <param name="speed">Target's absolute speed.</param>
        public void CalculateSpeedOnPath(ref float speed)
        {
            for (int i = 0; i < Points.Count; i++)
            {
                CalculateSpeedOnPoint(ref speed, _ownerTransform.position, i);
            }
        }

        /// <summary>
        /// Calculates target's speed on point, based on its position.
        /// </summary>
        /// <param name="speed">Target's absolute speed.</param>
        /// <param name="index">Index of navigation point.</param>
        public void CalculateSpeedOnPoint(ref float speed, int index)
        {
            float distance = Vector3.Distance(_ownerTransform.position, Points[index].Transform.position);

            if (distance <= Points[index].ImpactRadius)
            {
                speed = Mathf.Lerp(speed * Points[index].MinImpactSpeed, speed,
                        distance / Points[index].ImpactRadius);
            }
        }

        /// <summary>
        /// Calculates target's speed on path, based on its position.
        /// </summary>
        /// <param name="speed">Target's absolute speed.</param>
        /// <param name="position">Target's position.</param>
        public void CalculateSpeedOnPath(ref float speed, Vector3 position)
        {
            for (int i = 0; i < Points.Count; i++)
            {
                CalculateSpeedOnPoint(ref speed, position, i);
            }
        }

        /// <summary>
        /// Calculates target's speed on point, based on its position.
        /// </summary>
        /// <param name="speed">Target's absolute speed.</param>
        /// <param name="position">Target's position.</param>
        /// <param name="index">Index of navigation point.</param>
        public void CalculateSpeedOnPoint(ref float speed, Vector3 position, int index)
        {
            float distance = Vector3.Distance(position, Points[index].Transform.position);

            if (distance <= Points[index].ImpactRadius)
            {
                speed = Mathf.Lerp(speed * Points[index].MinImpactSpeed, speed,
                        distance / Points[index].ImpactRadius);
            }
        }


        /// <summary>
        /// Return next or random (based on container settings) navigation point.
        /// </summary>
        /// <param name="destinationIndex">Relative index of navigation point.</param>
        /// <returns>Position of target navigation  point.</returns>
        public abstract Vector3 GetDestination(ref int destinationIndex);
    }
}
