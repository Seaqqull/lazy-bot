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

        [SerializeField] LazyBot.Entity.Data.EntityState[] m_states;

        protected LazyBot.Navigation.NavigationContainer m_navigationPath;
        protected LazyBot.Navigation.NavigationContainer m_targets;

        protected LazyBot.Audio.AudioContainer m_audioContainer;
        protected EntityBehaviour m_behaviour;

        protected Coroutine m_sleepCoroutation;
        protected Point[] m_stateOrder; // X - state index, Y - state priority
        protected Transform m_transform;
        protected float m_timeSincePathUpdate;
        protected int m_navigationIndex;
        protected int m_targetIndex;
        protected bool m_isSleep;
        protected bool m_isBlock; // Used in two cases: isBlock on sleep \ isBlock on state(sleep deactivated)
        
        public LazyBot.Navigation.NavigationContainer NavigationPath
        {
            get { return this.m_navigationPath; }
        }
        public LazyBot.Entity.EntityBehaviour Behaviour
        {
            get { return this.m_behaviour; }
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
        public int TargetIndex
        {
            get { return this.m_targetIndex; }
            set { this.m_targetIndex = value; }
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
            m_navigationPath = GetComponent<LazyBot.Navigation.NavigationContainer>();
            m_targets = gameObject.AddComponent<LazyBot.Navigation.SimpleNavigation>();

            m_audioContainer = GetComponent<LazyBot.Audio.AudioContainer>();
            m_behaviour = GetComponent<EntityBehaviour>();            

            m_transform = gameObject.transform;
        }

        protected virtual void Start()
        {
            UpdateStates();
            OnStateChange();

            m_states[m_activeState].State.OnStateEnter(this);
        }

        protected virtual void Update()
        {
            if (m_behaviour.IsDeath) return;

            m_timeSincePathUpdate += Time.deltaTime;

            for (int i = 0; i < m_stateOrder.Length && !m_isBlock; i++)
            {
                if ((m_stateOrder[i].X == m_activeState) ||
                    (!(m_states[m_stateOrder[i].X].CheckOn & m_states[m_stateOrder[i].X].Id)) ||
                    (!(m_states[m_stateOrder[i].X].State.Validate(this)))) continue;

                m_states[m_activeState].State.OnStateExit(this);

                m_activeState = m_stateOrder[i].X;
                OnStateChange();

                m_states[m_activeState].State.OnStateEnter(this);                
                break;
            }

            m_states[m_activeState].State.Excute(this);
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

        /// <summary>
        /// Updates ordered que of states based on its priority.
        /// </summary>
        protected void UpdateStates()
        {
            m_stateOrder = new Point[m_states.Length];

            for (int i = 0; i < m_states.Length; i++)
            {
                m_stateOrder[i].X = i;
                m_stateOrder[i].Y = m_states[i].Priority;

                m_states[i].Id = (uint)i;

                m_states[i].CheckOn.Remove(i);
                m_states[i].CheckOn.ValidatesValues(0, m_states.Length - 1);

                m_states[i].CheckOn.Bake();
            }

            m_stateOrder = m_stateOrder.OrderByDescending((state) => state.Y).ToArray();
        }


        protected virtual void OnStateChange()
        {
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
