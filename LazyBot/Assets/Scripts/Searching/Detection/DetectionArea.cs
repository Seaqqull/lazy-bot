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

        [SerializeField] protected string _name;

        [SerializeField] protected Data.HitAreaState _state = Data.HitAreaState.Enabled;

        [SerializeField] protected LazyBot.Entity.EntityController _owner;

        [SerializeField] [Range(0, ushort.MaxValue)] protected float _damageMultiplier = 1.0f;

        protected Func<Vector3, float> _onSound = delegate { return 0; };
        protected Action<float> _onDamage = delegate { };

        protected Color _gizmoColor = editor_gizmo_color;

        protected static uint _idCounter = 0;
        protected bool _wasFound = false;
        protected Collider _colider;
        protected uint _id;

        public Func<Vector3, float> OnSound
        {
            get { return this._onSound; }
            set { this._onSound = value; }
        }
        public Data.HitAreaState State
        {
            get
            {
                return this._state;
            }
            set
            {
                if (((_state == Data.HitAreaState.Unknown) || (_state == Data.HitAreaState.Disabled)) &&
                    (value != Data.HitAreaState.Enabled))
                    return;

                SetState(value);
            }
        }
        public Action<float> OnDamage
        {
            get { return this._onDamage; }
            set { this._onDamage = value; }
        }
        public float DamageMultiplier
        {
            get { return this._damageMultiplier; }
            set { this._damageMultiplier = value; }
        }        
        public Collider Collider
        {
            get
            {
                return this._colider;
            }
        }
        public uint OwnerId
        {
            get
            {
                if (_owner) return _owner.Behaviour.Id;

#if UNITY_EDITOR
                Debug.LogError("Owner not defined");
#endif
                return 0;
            }
        }
        public string Name
        {
            get
            {
                return this._name;
            }
        }
        public uint Id
        {
            get { return this._id; }
        }
        
        public Color _gizmoColorInactive = Color.grey;
        public Color _gizmoColorActive = Color.blue;
        public Color _gizmoColorFound = Color.red;
        public float _gizmoSize = 0.1f;


        protected virtual void Awake()
        {
            _id = _idCounter++;

            _colider = GetCollider();
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
            _state = Data.HitAreaState.Enabled;
        }

        protected virtual void LateUpdate()
        {
            if (_state == Data.HitAreaState.Unknown) return;

            if ((!_wasFound) &&
                (_state != Data.HitAreaState.Disabled))
                State = Data.HitAreaState.Enabled;

            SetAreaColor();
            _wasFound = false;
        }


        /// <summary>
        /// Changes gizmo color based on its state.
        /// </summary>
        protected virtual void SetAreaColor()
        {
            switch (_state)
            {                
                case Data.HitAreaState.Disabled:
                    this._gizmoColor = this._gizmoColorInactive;
                    break;
                case Data.HitAreaState.Enabled:
                    this._gizmoColor = this._gizmoColorActive;
                    break;
                case Data.HitAreaState.Found:
                    this._gizmoColor = this._gizmoColorFound;
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
                _wasFound = true;
            if (incomeState == Data.HitAreaState.Disabled)
                _wasFound = false;

            _state = incomeState;            
        }


        protected abstract void OnDrawGizmos();


        public void PerformDamage(float damage)
        {
            _onDamage(damage * _damageMultiplier);
        }

        public float EmitSound(Vector3 listener)
        {            
            return _onSound(listener);
        }
    }
}
