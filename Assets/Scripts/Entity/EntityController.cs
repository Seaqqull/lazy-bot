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
        [SerializeField] [Range(0, ushort.MaxValue)] protected int m_activeState;
        /// <summary>
        /// Period of destination path update.
        /// </summary>
        [SerializeField] [Range(0, ushort.MaxValue)] protected float m_pathUpdateDelay;
        [SerializeField] protected LazyBot.Entity.Data.EntityState[] m_statesSteady;
        [SerializeField] protected LazyBot.Entity.Data.EntityState[] m_states;

        protected LazyBot.Area.Detection.DetectionAreaContainer m_detectionAreas;
        protected LazyBot.Navigation.NavigationContainer m_navigationPath;
        protected LazyBot.Target.Data.TargetInfo m_targets;

        protected LazyBot.Audio.AudioContainer m_audioContainer;
        protected EntityBehaviour m_behaviour;

        protected Coroutine m_sleepCoroutation;
        protected float m_timeSincePathUpdate;
        protected Transform m_transform;
        protected int m_navigationIndex;

        protected BitArray m_steadyActivation;
        protected Point[] m_stateSteadyOrder; // X - state index, Y - state priority
        protected Point[] m_stateOrder; // X - state index, Y - state priority

        protected bool m_isSleep;
        protected bool m_isBlock; // Used in two cases: isBlock on sleep \ isBlock on state(sleep deactivated)

        public LazyBot.Area.Detection.DetectionAreaContainer DetectionAreas
        {
            get
            {
                return m_detectionAreas;
            }
        }
        public LazyBot.Navigation.NavigationContainer NavigationPath
        {
            get { return this.m_navigationPath; }
        }
        public LazyBot.Entity.EntityBehaviour Behaviour
        {
            get { return this.m_behaviour; }
        }
        public LazyBot.Target.Data.TargetInfo Targets
        {
            get { return this.m_targets; }
        }
        public int NavigationIndex
        {
            get { return this.m_navigationIndex; }
            set { this.m_navigationIndex = value; }
        }
        public Transform Transform
        {
            get { return this.m_transform; }
        }
        public bool IsBlocked
        {
            get { return this.m_isBlock; }
            set { this.m_isBlock = value; }
        }
        public bool IsSleep
        {
            get { return this.m_isSleep; }
        }
        

        protected virtual void Awake()
        {
            m_detectionAreas = GetComponent<LazyBot.Area.Detection.DetectionAreaContainer>();
            m_navigationPath = GetComponent<LazyBot.Navigation.NavigationContainer>();
            m_targets = new LazyBot.Target.Data.TargetInfo();

            m_audioContainer = GetComponent<LazyBot.Audio.AudioContainer>();
            m_behaviour = GetComponent<EntityBehaviour>();            

            m_transform = gameObject.transform;
        }

        protected virtual void Start()
        {
            UpdateStates(m_statesSteady, out m_stateSteadyOrder);
            UpdateStates(m_states, out m_stateOrder);

            OnStateChange();

            DoStateAction(m_states, m_activeState, Data.StateAction.OnEnter);

            ActivateSteadyState();
        }

        protected virtual void Update()
        {
            if (m_behaviour.IsDeath) return;

            m_timeSincePathUpdate += Time.deltaTime;

            int i;
            Data.EntityState state;

            for (i = 0; i < m_stateSteadyOrder.Length; i++)
            {
                state = m_statesSteady[m_stateSteadyOrder[i].X];

                if (state.State.Validate(this))
                {
                    if (!m_steadyActivation[m_stateSteadyOrder[i].X])
                    {
                        m_steadyActivation[m_stateSteadyOrder[i].X] = true;
                        state.State.OnStateEnter(this);
                    }
                    state.State.Excute(this);
                }
                else if (m_steadyActivation[m_stateSteadyOrder[i].X])
                {                   
                    m_steadyActivation[m_stateSteadyOrder[i].X] = false;
                    state.State.OnStateExit(this);                 
                }
            }

            for (i = 0; i < m_stateOrder.Length && !m_isBlock; i++)
            {
                state = m_states[m_stateOrder[i].X];

                if ((m_stateOrder[i].X == m_activeState) ||
                    (!(state.CheckOn & m_states[m_activeState].Id)) ||
                    (!(state.State.Validate(this)))) continue;

                state.State.OnStateExit(this);

                m_activeState = m_stateOrder[i].X;
                OnStateChange();

                state.State.OnStateEnter(this);                
                break;
            }

            DoStateAction(m_states, m_activeState, Data.StateAction.Execute);
        }
        

        /// <summary>
        /// Cancels delayed execute of exiting from sleep.
        /// </summary>
        protected void ResetSleep()
        {
            m_isSleep = false;
            if (m_sleepCoroutation != null)
            {
                StopCoroutine(m_sleepCoroutation);
                m_sleepCoroutation = null;
            }
        }

        protected void ActivateSteadyState()
        {
            Data.EntityState state;
            m_steadyActivation = new BitArray(m_stateSteadyOrder.Length);            

            for (int i = 0; i < m_stateSteadyOrder.Length; i++)
            {
                state = m_statesSteady[m_stateSteadyOrder[i].X];

                if (!state.State.Validate(this))
                    continue;

                m_steadyActivation[i] = true;

                state.State.OnStateEnter(this);
                state.State.Excute(this);
            }
        }

        protected void AddTargetType(Target.Property.TargetTypeSO newType)
        {
            if (!m_targets.Data.ContainsKey(newType))
                m_targets.AddType(newType);
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
            if (m_states.Length == 0) return;

            ResetSleep();
            m_isBlock = m_states[m_activeState].IsBlocking;
        }


        public virtual void Death()
        {
            //if (!m_animation.IsDead)
            //    m_animation.Dead();
            //return;
            Destroy(gameObject);
        }

        public virtual void WakeUp()
        {
            ResetSleep();
            m_isBlock = m_states[m_activeState].IsBlocking;
        }

        public virtual void Damage(float damage)
        {
            m_behaviour.Damage(damage);
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

            m_isSleep = true;
            m_isBlock = m_states[m_activeState].IsBlockingOnSleep;

            if (time < 0.0f) return;

            m_sleepCoroutation = RunLaterValued(
                ()=> 
                {
                    this.m_isSleep = false;
                    this.m_isBlock = m_states[m_activeState].IsBlocking;
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
                m_isBlock = false;
                return;
            }

            m_isSleep = true;
            m_isBlock = true;

            m_sleepCoroutation = RunLaterValued(
                () =>
                {
                    this.m_isSleep = false;
                    this.m_isBlock = false;
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
                m_navigationPath.GetNearestPoint(ref m_navigationIndex),
                true
            );
        }

        public void OnUpdatePathDestinationNext()
        {
            UpdatePath(
                m_navigationPath.GetDestination(ref m_navigationIndex),
                true
            );
        }

        public void ClearTarget(LazyBot.Area.Searching.SearchingArea area)
        {
            if (area.TargetType == null) return;

            Target.Data.TargetInfo.TargetContainer targetContainer;

            if (m_targets.Data.TryGetValue(area.TargetType, out targetContainer))
                targetContainer.Erase(area.Id);
        }

        public void InitTargetContainer(LazyBot.Area.Searching.SearchingArea area)
        {
            if (area.TargetType == null) return;

            AddTargetType(area.TargetType);

            m_targets[area.TargetType].AddArea(area.Id);
        }

        public void AddTarget(LazyBot.Area.Searching.SearchingArea area, LazyBot.Area.Detection.DetectionArea detectionArea)
        {
            if (area.TargetType == null) return;

            AddTargetType(area.TargetType);

            if (area.TargetType.DataOnDetection)// Fill data
                m_targets[area.TargetType].AddTarget(area.Id, 
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
