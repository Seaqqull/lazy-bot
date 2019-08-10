using UnityEngine.Events;
using UnityEngine;

namespace LazyBot.Entity
{
    /// <summary>
    /// Behaviour of entity. Controlls health, stamina etc.
    /// </summary>
    public class EntityBehaviour : MonoBehaviour
    {
        [SerializeField] private LazyBot.Entity.Data.SliderFloatEntityData _health;
        [SerializeField] private LazyBot.Entity.Data.SliderFloatEntityData _stamina;

        /// <summary>
        /// Point in scene used to chase the target.
        /// </summary>
        [SerializeField] private LazyBot.Navigation.Data.NavigationPoint _trackingPoint;

        /// <summary>
        /// Triggers on damage taken.
        /// </summary>
        [SerializeField] private UnityEvent _onDamage;
        /// <summary>
        /// Triggers on death.
        /// </summary>
        [SerializeField] private UnityEvent _onDeath;

        private static uint _idCounter = 0;
        private bool _isDeath;
        private uint _id;

        /// <summary>
        /// Point in scene used to chase the target.
        /// </summary>
        public LazyBot.Navigation.Data.NavigationPoint TrackingPoint
        {
            get { return this._trackingPoint; }
        }
        public bool IsDeath
        {
            get { return this._isDeath; }
        }
        public uint Id
        {
            get { return this._id; }
        }


        protected virtual void Awake()
        {
            _id = _idCounter++;
        }

        protected virtual void Update()
        {
            if (_isDeath) return;

            _stamina.Regenerate(Time.deltaTime);
            _health.Regenerate(Time.deltaTime);
        }


        protected virtual void Death()
        {
            _stamina.IsLock = true;
            _health.IsLock = true;
            _isDeath = true;

            _onDeath.Invoke();
        }


        public void Damage(float damage)
        {
            if (_isDeath) return;

            _health.Change(-damage, true);

            if ((_health.IsLow) &&
                ((!_isDeath) && (!_health.IsUnlimited)))
                Death();
            else _onDamage.Invoke();
        }

        public bool DoHealthAction(int healthConsumption)
        {
            if (_isDeath) return false;

            return _health.Change(-healthConsumption);
        }

        public bool IsHealthAction(int healthConsumption)
        {
            if (_isDeath) return false;

            return _health.IsChangable(-healthConsumption);
        }

        public bool DoStaminaAction(int staminaCosumption)
        {
            if (_isDeath) return false;

            return _stamina.Change(-staminaCosumption);
        }

        public bool IsStaminaAction(int staminaCosumption)
        {
            if (_isDeath) return false;

            return _stamina.IsChangable(-staminaCosumption);
        }
    }
}
