using UnityEngine;
using System;

namespace LazyBot.Area.Detection
{    
    /// <summary>
    /// Area used for detection by searchers
    /// </summary>
    public abstract class DetectionArea : MonoBehaviour
    {
        protected static Color editor_gizmo_color = Color.blue;

        [SerializeField] protected string m_name;

        [SerializeField] protected Data.HitAreaState m_state = Data.HitAreaState.Enabled;

        [SerializeField] [Range(0, ushort.MaxValue)] protected float m_damageMultiplier = 1.0f;

        protected Func<Vector3, float> m_onSound = delegate { return 0; };
        protected Action<float> m_onDamage = delegate { };

        protected Color m_gizmoColor = editor_gizmo_color;

        protected static uint m_idCounter = 0;
        protected bool m_wasFound = false;
        protected Collider m_colider;
        protected uint m_id;

        public Func<Vector3, float> OnSound
        {
            get { return this.m_onSound; }
            set { this.m_onSound = value; }
        }
        public Data.HitAreaState State
        {
            get
            {
                return this.m_state;
            }
            set
            {
                if (((m_state == Data.HitAreaState.Unknown) || (m_state == Data.HitAreaState.Disabled)) &&
                    (value != Data.HitAreaState.Enabled))
                    return;

                SetState(value);
            }
        }
        public Action<float> OnDamage
        {
            get { return this.m_onDamage; }
            set { this.m_onDamage = value; }
        }
        public float DamageMultiplier
        {
            get { return this.m_damageMultiplier; }
            set { this.m_damageMultiplier = value; }
        }        
        public Collider Collider
        {
            get
            {
                return this.m_colider;
            }
        }
        public string Name
        {
            get
            {
                return this.m_name;
            }
        }
        public uint Id
        {
            get { return this.m_id; }
        }
        
        public Color m_gizmoColorInactive = Color.grey;
        public Color m_gizmoColorActive = Color.blue;
        public Color m_gizmoColorFound = Color.red;
        public float m_gizmoSize = 0.1f;

        protected virtual void Awake()
        {
            m_id = m_idCounter++;

            m_colider = GetCollider();
        }

        protected virtual void OnEnable()
        {
            State = Data.HitAreaState.Enabled;
        }

        protected virtual void OnDisable()
        {
            State = Data.HitAreaState.Disabled;
        }

        protected virtual void OnDestroy()
        {
            m_state = Data.HitAreaState.Enabled;
        }

        protected virtual void LateUpdate()
        {
            if (m_state == Data.HitAreaState.Unknown) return;

            if ((!m_wasFound) &&
                (m_state != Data.HitAreaState.Disabled))
                State = Data.HitAreaState.Enabled;

            SetAreaColor();
            m_wasFound = false;
        }


        /// <summary>
        /// Changes gizmo color based on its state.
        /// </summary>
        protected virtual void SetAreaColor()
        {
            switch (m_state)
            {                
                case Data.HitAreaState.Disabled:
                    this.m_gizmoColor = this.m_gizmoColorInactive;
                    break;
                case Data.HitAreaState.Enabled:
                    this.m_gizmoColor = this.m_gizmoColorActive;
                    break;
                case Data.HitAreaState.Found:
                    this.m_gizmoColor = this.m_gizmoColorFound;
                    break;
                default:
                    break;
            }
        }

        protected virtual Collider GetCollider()
        {
            return GetComponent<Collider>();
        }

        protected virtual void SetState(Data.HitAreaState incomeState)
        {
            if (incomeState == Data.HitAreaState.Found)
                m_wasFound = true;
            if (incomeState == Data.HitAreaState.Disabled)
                m_wasFound = false;

            m_state = incomeState;            
        }


        protected abstract void OnDrawGizmos();


        public void PerformDamage(float damage)
        {
            m_onDamage(damage * m_damageMultiplier);
        }

        public float EmitSound(Vector3 listener)
        {            
            return m_onSound(listener);
        }
    }
}
