using UnityEngine;

namespace LazyBot.Entity
{
    public class EntityControllerSimple : EntityController
    {
        [SerializeField] [Range(0.0f, ushort.MaxValue)] private float m_angularSpeed;
        [SerializeField] [Range(0.0f, ushort.MaxValue)] private float m_maxMovementSpeed;

        private float m_speed;        
        private Vector3 m_destination;
        private Vector3 m_direction;

        /// <summary>
        /// Returns distance to target.
        /// </summary>
        /// <returns>Distance to target.</returns>
        public override float Distance()
        {
            return Vector3.Distance(m_transform.position, m_destination);
        }

        /// <summary>
        /// Updates speed.
        /// </summary>
        /// <param name="speed">Speed.</param>
        public override void UpdateSpeed(float speed)
        {
            m_speed = (speed > m_maxMovementSpeed) ? m_maxMovementSpeed : speed;
        }

        /// <summary>
        /// Moves entity in the scene.
        /// </summary>
        public override void Move()
        {
            m_direction = m_destination - m_transform.position;

            float step = m_angularSpeed * Mathf.Deg2Rad * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(m_transform.forward, m_direction, step, 0.0f);
            //Debug.DrawRay(m_transform.position, newDir, Color.red);
            
            m_transform.rotation = Quaternion.LookRotation(newDir);
            m_transform.Translate(Vector3.forward * m_speed * Time.deltaTime);
        }

        /// <summary>
        /// Update's destination position.
        /// </summary>
        /// <param name="destination">Position in scene.</param>
        /// <param name="isImmediate">Is update must be executed immediately.</param>
        public override void UpdatePath(Vector3 destination = new Vector3(), bool isImmediate = false)
        {
            if ((!isImmediate) &&
                (m_timeSincePathUpdate < m_pathUpdateDelay))
                return;

            m_destination = (destination == Vector3.zero) ?
                m_destination : destination;            

            m_timeSincePathUpdate = 0.0f;
        }
    }
}
