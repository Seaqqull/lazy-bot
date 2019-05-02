using UnityEngine.Events;
using UnityEngine;

namespace LazyBot.Entity
{
    /// <summary>
    /// Behaviour of entity. Controlls health, stamina etc.
    /// </summary>
    public class EntityBehaviour : MonoBehaviour
    {
        [SerializeField] private LazyBot.Entity.Data.SliderFloatEntityData m_health;
        [SerializeField] private LazyBot.Entity.Data.SliderFloatEntityData m_stamina;

        /// <summary>
        /// Point in scene used to chase the target.
        /// </summary>
        [SerializeField] private LazyBot.Navigation.Data.NavigationPoint m_trackingPoint;

        /// <summary>
        /// Triggers on damage taken.
        /// </summary>
        [SerializeField] private UnityEvent m_onDamage;
        /// <summary>
        /// Triggers on death.
        /// </summary>
        [SerializeField] private UnityEvent m_onDeath;

        private static uint m_idCounter = 0;
        private bool m_isDeath;
        private uint m_id;

        /// <summary>
        /// Point in scene used to chase the target.
        /// </summary>
        public LazyBot.Navigation.Data.NavigationPoint TrackingPoint
        {
            get { return this.m_trackingPoint; }
        }
        public bool IsDeath
        {
            get { return this.m_isDeath; }
        }
        public uint Id
        {
            get { return this.m_id; }
        }


        protected virtual void Awake()
        {
            m_id = m_idCounter++;
        }

        protected virtual void Update()
        {
            if (m_isDeath) return;

            m_stamina.Regenerate(Time.deltaTime);
            m_health.Regenerate(Time.deltaTime);
        }


        protected virtual void Death()
        {
            m_stamina.IsLock = true;
            m_health.IsLock = true;
            m_isDeath = true;

            m_onDeath.Invoke();
        }


        public void Damage(float damage)
        {
            if (m_isDeath) return;

            m_health.Change(-damage, true);

            if ((m_health.IsLow) &&
                ((!m_isDeath) && (!m_health.IsUnlimited)))
                Death();
            else m_onDamage.Invoke();
        }

        public bool DoHealthAction(int healthConsumption)
        {
            if (m_isDeath) return false;

            return m_health.Change(-healthConsumption);
        }

        public bool IsHealthAction(int healthConsumption)
        {
            if (m_isDeath) return false;

            return m_health.IsChangable(-healthConsumption);
        }

        public bool DoStaminaAction(int staminaCosumption)
        {
            if (m_isDeath) return false;

            return m_stamina.Change(-staminaCosumption);
        }

        public bool IsStaminaAction(int staminaCosumption)
        {
            if (m_isDeath) return false;

            return m_stamina.IsChangable(-staminaCosumption);
        }
    }
}
