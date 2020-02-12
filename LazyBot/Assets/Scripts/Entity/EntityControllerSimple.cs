using UnityEngine;

namespace LazyBot.Entity
{
    public class EntityControllerSimple : EntityController
    {
#pragma warning disable 0649
        [SerializeField] [Range(0.0f, ushort.MaxValue)] private float _angularSpeed;
        [SerializeField] [Range(0.0f, ushort.MaxValue)] private float _maxMovementSpeed;
#pragma warning restore 0649

        private float _speed;        
        private Vector3 _destination;
        private Vector3 _direction;

        /// <summary>
        /// Returns distance to target.
        /// </summary>
        /// <returns>Distance to target.</returns>
        public override float Distance()
        {
            return Vector3.Distance(_transform.position, _destination);
        }

        /// <summary>
        /// Updates speed.
        /// </summary>
        /// <param name="speed">Speed.</param>
        public override void UpdateSpeed(float speed)
        {
            _speed = (speed > _maxMovementSpeed) ? _maxMovementSpeed : speed;
        }

        /// <summary>
        /// Moves entity in the scene.
        /// </summary>
        public override void Move()
        {
            _direction = _destination - _transform.position;

            float step = _angularSpeed * Mathf.Deg2Rad * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(_transform.forward, _direction, step, 0.0f);
            //Debug.DrawRay(_transform.position, newDir, Color.red);
            
            _transform.rotation = Quaternion.LookRotation(newDir);
            _transform.Translate(Vector3.forward * _speed * Time.deltaTime);
        }

        /// <summary>
        /// Update's destination position.
        /// </summary>
        /// <param name="destination">Position in scene.</param>
        /// <param name="isImmediate">Is update must be executed immediately.</param>
        public override void UpdatePath(Vector3 destination = new Vector3(), bool isImmediate = false)
        {
            if ((!isImmediate) &&
                (_timeSincePathUpdate < _pathUpdateDelay))
                return;

            _destination = (destination == Vector3.zero) ?
                _destination : destination;            

            _timeSincePathUpdate = 0.0f;
        }
    }
}
