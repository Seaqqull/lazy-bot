using System.Collections;
using System.Drawing;
using System.Linq;
using UnityEngine;

namespace LazyBot.Entity
{
    /// <summary>
    /// Controller of entity. 
    /// Contain some handlers of behaviour events.
    /// Controls entity state and its behaviour.
    /// </summary>
    public abstract class EntityController : MonoBehaviour
    {
        /// <summary>
        /// Index of active state.
        /// </summary>
        [SerializeField] [Range(0, ushort.MaxValue)] protected int _activeState;
        /// <summary>
        /// Period of destination path update.
        /// </summary>
        [SerializeField] [Range(0, ushort.MaxValue)] protected float _pathUpdateDelay;
        /// <summary>
        /// Used to controll the transition between states(eg: restriction, permission);
        /// </summary>
        [SerializeField] [Range(0, 100)] protected int _awareness;
        [SerializeField] protected LazyBot.Entity.Data.EntityState[] _statesSteady;
        [SerializeField] protected LazyBot.Entity.Data.EntityState[] _states;

        protected LazyBot.Area.Detection.DetectionAreaContainer _detectionAreas;
        protected LazyBot.Navigation.NavigationContainer _navigationPath;
        protected LazyBot.Target.Data.TargetInfo _targets;

        protected LazyBot.Audio.AudioContainer _audioContainer;
        protected EntityBehaviour _behaviour;

        protected Coroutine _sleepCoroutation;
        protected float _timeSincePathUpdate;
        protected Transform _transform;
        protected int _navigationIndex;

        protected BitArray _steadyActivation;
        protected Point[] _stateSteadyOrder; // X - state index, Y - state priority
        protected Point[] _stateOrder; // X - state index, Y - state priority

        protected bool _isSleep;
        protected bool _isBlock; // Used in two cases: isBlock on sleep \ isBlock on state(sleep deactivated)

        public LazyBot.Area.Detection.DetectionAreaContainer DetectionAreas
        {
            get
            {
                return _detectionAreas;
            }
        }
        public LazyBot.Navigation.NavigationContainer NavigationPath
        {
            get { return this._navigationPath; }
        }
        public LazyBot.Entity.EntityBehaviour Behaviour
        {
            get { return this._behaviour; }
        }
        public LazyBot.Target.Data.TargetInfo Targets
        {
            get { return this._targets; }
        }
        public int NavigationIndex
        {
            get { return this._navigationIndex; }
            set { this._navigationIndex = value; }
        }
        public Transform Transform
        {
            get { return this._transform; }
        }
        public bool IsBlocked
        {
            get { return this._isBlock; }
            set { this._isBlock = value; }
        }
        public bool IsSleep
        {
            get { return this._isSleep; }
        }
        

        protected virtual void Awake()
        {
            _detectionAreas = GetComponent<LazyBot.Area.Detection.DetectionAreaContainer>();
            _navigationPath = GetComponent<LazyBot.Navigation.NavigationContainer>();
            _targets = new LazyBot.Target.Data.TargetInfo();

            _audioContainer = GetComponent<LazyBot.Audio.AudioContainer>();
            _behaviour = GetComponent<EntityBehaviour>();            

            _transform = gameObject.transform;
        }

        protected virtual void Start()
        {
            UpdateStates(_statesSteady, out _stateSteadyOrder);
            UpdateStates(_states, out _stateOrder);

            OnStateChange();

            DoStateAction(_states, _activeState, Data.StateAction.OnEnter);

            ActivateSteadyState();
        }

        protected virtual void Update()
        {
            if (_behaviour.IsDeath) return;

            _timeSincePathUpdate += Time.deltaTime;

            int i;
            Data.EntityState state;

            for (i = 0; i < _stateSteadyOrder.Length; i++)
            {
                state = _statesSteady[_stateSteadyOrder[i].X];

                if (state.State.Validate(this))
                {
                    if (!_steadyActivation[_stateSteadyOrder[i].X])
                    {
                        _steadyActivation[_stateSteadyOrder[i].X] = true;
                        state.State.OnStateEnter(this);
                    }
                    state.State.Excute(this);
                }
                else if (_steadyActivation[_stateSteadyOrder[i].X])
                {                   
                    _steadyActivation[_stateSteadyOrder[i].X] = false;
                    state.State.OnStateExit(this);                 
                }
            }

            for (i = 0; i < _stateOrder.Length && !_isBlock; i++)
            {
                state = _states[_stateOrder[i].X];

                if ((_stateOrder[i].X == _activeState) ||
                    (!(state.CheckOn & _states[_activeState].Id)) ||
                    (!(state.State.Validate(this)))) continue;

                state.State.OnStateExit(this);

                _activeState = _stateOrder[i].X;
                OnStateChange();

                state.State.OnStateEnter(this);                
                break;
            }

            DoStateAction(_states, _activeState, Data.StateAction.Execute);
        }
        

        /// <summary>
        /// Cancels delayed execute of exiting from sleep.
        /// </summary>
        protected void ResetSleep()
        {
            _isSleep = false;
            if (_sleepCoroutation != null)
            {
                StopCoroutine(_sleepCoroutation);
                _sleepCoroutation = null;
            }
        }

        protected void ActivateSteadyState()
        {
            Data.EntityState state;
            _steadyActivation = new BitArray(_stateSteadyOrder.Length);            

            for (int i = 0; i < _stateSteadyOrder.Length; i++)
            {
                state = _statesSteady[_stateSteadyOrder[i].X];

                if (!state.State.Validate(this))
                    continue;

                _steadyActivation[i] = true;

                state.State.OnStateEnter(this);
                state.State.Excute(this);
            }
        }

        protected void AddTargetType(Target.Property.TargetTypeSO newType)
        {
            if (!_targets.Data.ContainsKey(newType))
                _targets.AddType(newType);
        }

        /// <summary>
        /// Updates ordered que of states based on its priority.
        /// </summary>
        protected void UpdateStates(LazyBot.Entity.Data.EntityState[] states, out Point[] order)
        {
            order = new Point[states.Length];

            for (int i = 0; i < states.Length; i++)
            {
                order[i].X = i;
                order[i].Y = states[i].Priority;

                states[i].Id = (uint)i;

                states[i].CheckOn.Remove((uint)i);
                states[i].CheckOn.ValidatesValues(0, (uint)(states.Length - 1));

                states[i].CheckOn.Bake();
            }

            order = order.OrderByDescending((state) => state.Y).ToArray();
        }

        protected void DoStateAction(LazyBot.Entity.Data.EntityState state, LazyBot.Entity.Data.StateAction action)
        {            
            switch (action)
            {
                case Data.StateAction.Validate:
                    state.State.Validate(this);
                    break;
                case Data.StateAction.OnEnter:
                    state.State.OnStateEnter(this);
                    break;
                case Data.StateAction.Execute:
                    state.State.Excute(this);
                    break;
                case Data.StateAction.OnExit:
                    state.State.OnStateExit(this);
                    break;
                default:
#if UNITY_EDITOR
                    Debug.LogError("Unknown state action", this);
#endif
                    break;
            }
        }

        protected void DoStateAction(LazyBot.Entity.Data.EntityState[] states, int actionPosition, LazyBot.Entity.Data.StateAction action)
        {
            if (actionPosition >= states.Length) return;

            DoStateAction(states[actionPosition], action);
        }


        protected virtual void OnStateChange()
        {
            if (_states.Length == 0) return;

            ResetSleep();
            _isBlock = _states[_activeState].IsBlocking;
        }


        public virtual void Death()
        {
            //if (!_animation.IsDead)
            //    _animation.Dead();
            //return;
            Destroy(gameObject);
        }

        public virtual void WakeUp()
        {
            ResetSleep();
            _isBlock = _states[_activeState].IsBlocking;
        }

        public virtual void Damage(float damage)
        {
            _behaviour.Damage(damage);
        }

        /// <summary>
        /// Turns controller to sleep.
        /// </summary>
        /// <param name="time">
        /// Time to be sleepped. 
        /// Negative value used to perform unlimited sleep
        /// </param>
        public virtual void TurnSleep(float time) 
        {
            ResetSleep();

            _isSleep = true;
            _isBlock = _states[_activeState].IsBlockingOnSleep;

            if (time < 0.0f) return;

            _sleepCoroutation = RunLaterValued(
                ()=> 
                {
                    this._isSleep = false;
                    this._isBlock = _states[_activeState].IsBlocking;
                },
                time
            );
        }

        /// <summary>
        /// Turns controller to sleep. 
        /// After specified amount of time performs instant state change.
        /// </summary>
        /// <param name="time">
        /// Time to be sleepped.
        /// Negative value perform instant state change.
        /// </param>
        public virtual void MakeInstantStateChange(float time)
        {
            ResetSleep();

            if (time < 0.0f)
            {
                _isBlock = false;
                return;
            }

            _isSleep = true;
            _isBlock = true;

            _sleepCoroutation = RunLaterValued(
                () =>
                {
                    this._isSleep = false;
                    this._isBlock = false;
                },
                time
            );
        }


        /// <summary>
        /// Performs movement.
        /// </summary>
        public abstract void Move();

        /// <summary>
        /// Returns distance to destination point.
        /// </summary>
        /// <returns>Distance to destination point.</returns>
        public abstract float Distance();

        /// <summary>
        /// Updates speed.
        /// </summary>
        /// <param name="speed">Speed.</param>
        public abstract void UpdateSpeed(float speed);

        /// <summary>
        /// Updates destination position.
        /// </summary>
        /// <param name="destination">Position in scene.</param>
        /// <param name="isImmediate">Is update must be executed immediately.</param>
        public abstract void UpdatePath(Vector3 destination = new Vector3(), bool isImmediate = false);


        public void OnUpdatePathDestination()
        {
            UpdatePath(
                _navigationPath.GetNearestPoint(ref _navigationIndex),
                true
            );
        }

        public void OnUpdatePathDestinationNext()
        {
            UpdatePath(
                _navigationPath.GetDestination(ref _navigationIndex),
                true
            );
        }

        public void ClearTarget(LazyBot.Area.Searching.SearchingArea area)
        {
            if (area.TargetType == null) return;

            Target.Data.TargetInfo.TargetContainer targetContainer;

            if (_targets.Data.TryGetValue(area.TargetType, out targetContainer))
                targetContainer.Erase(area.Id);
        }

        public void InitTargetContainer(LazyBot.Area.Searching.SearchingArea area)
        {
            if (area.TargetType == null) return;

            AddTargetType(area.TargetType);

            _targets[area.TargetType].AddArea(area.Id);
        }

        public void AddTarget(LazyBot.Area.Searching.SearchingArea area, LazyBot.Area.Detection.DetectionArea detectionArea)
        {
            if (area.TargetType == null) return;

            AddTargetType(area.TargetType);

            if (area.TargetType.DataOnDetection)// Fill data
                _targets[area.TargetType].AddTarget(area.Id, 
                    LazyBot.Manager.EntityManager.Instance.GetProperties(detectionArea.OwnerId, area.TargetType.Mask));
            //else data will be filled on state execution
        }

        /// <summary>
        /// Runs custom method after delay.
        /// </summary>
        /// <param name="method">Custom method.</param>
        /// <param name="waitSeconds">Delay.</param>
        protected void RunLater(System.Action method, float waitSeconds)
        {
            if (waitSeconds < 0 || method == null)
            {
                return;
            }
            StartCoroutine(RunLaterCoroutine(method, waitSeconds));
        }

        /// <summary>
        /// Runs custom method after delay.
        /// </summary>
        /// <param name="method">Custom method.</param>
        /// <param name="waitSeconds">Delay.</param>
        protected Coroutine RunLaterValued(System.Action method, float waitSeconds)
        {
            if (waitSeconds < 0 || method == null)
            {
                return null;
            }
            return StartCoroutine(RunLaterCoroutine(method, waitSeconds));
        }

        /// <summary>
        /// Runs custom method after delay.
        /// </summary>
        /// <param name="method">Custom method.</param>
        /// <param name="waitSeconds">Delay.</param>
        protected IEnumerator RunLaterCoroutine(System.Action method, float waitSeconds)
        {
            yield return new WaitForSeconds(waitSeconds);
            method();
        }

    }
}
