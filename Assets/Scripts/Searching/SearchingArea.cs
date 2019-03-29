using System.Collections;
using UnityEngine.Events;
using UnityEngine;

namespace LazyBot.Area.Searching
{
    /// <summary>
    /// Searches for targets and sends found to subscriber.
    /// </summary>
    public class SearchingArea : MonoBehaviour
    {
        [System.Serializable]
        public class SearchingAreaDrawEvent : UnityEvent<SearchingArea> { }
        [System.Serializable]
        public class TargetUpdateEvent : UnityEvent<SearchingArea, LazyBot.Area.Detection.DetectionArea> { }

        [SerializeField] private LazyBot.Area.Data.ObservationType m_type = LazyBot.Area.Data.ObservationType.Undefined;

        /// <summary>
        /// Update frequency of target detection.
        /// </summary>
        [SerializeField] private FloatReference m_updateRate;
        [SerializeField] [Range(0, ushort.MaxValue)] private ushort m_priority = 0;

        [SerializeField] private DetectionValidatorSO[] m_onTargetDetection;        
        [SerializeField] private SearchingAreaDrawEvent m_onDrawGizmo;
        [SerializeField] private TargetUpdateEvent m_onTargetUpdate;

        /// <summary>
        /// Area characteristics.
        /// </summary>
        [SerializeField] private LazyBot.Area.Data.AreaData m_data;

        /// <summary>
        /// Reference on target detection function.
        /// </summary>
        private Coroutine m_searchingCorotation;
        private static uint m_idCounter = 0;
        private uint m_id;

        public LazyBot.Area.Data.ObservationType Type
        {
            get { return this.m_type; }
        }
        public LazyBot.Area.Data.AreaData Data
        {
            get { return this.m_data; }
        }
        public Transform Socket
        {
            get { return this.m_data.Socket; }
        }
        public ushort Priority
        {
            get { return this.m_priority; }
        }        
        public uint Id
        {
            get { return this.m_id; }
        }
        

        private void Awake()
        {
            m_id = m_idCounter++;

            m_data.Socket = m_data.Socket ?? this.transform;
        }

        private void OnEnable()
        {
            SetIsActive(true);
        }

        private void OnDisable()
        {
            SetIsActive(false);
        }

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (!m_data.Socket)
                m_data.Socket = transform;

            m_onDrawGizmo.Invoke(this);
#endif
        }


        private void SetIsActive(bool isActive)
        {
            if (isActive)
            {
                m_searchingCorotation = StartCoroutine("FindTargetsWithDelay", m_updateRate.Value);
            }
            else if ((!isActive) && (m_searchingCorotation != null))
            {
                StopCoroutine(m_searchingCorotation);
            }
            else
            {
                Debug.LogError("Can't stop coroutation that refers to null.");
            }
        }

        private IEnumerator FindTargetsWithDelay(float delay)
        {
            while (true)
            {
                yield return new WaitForSeconds(delay);
                
                // some detection code
                for (int i = 0; i < m_data.EnemyTags.Length; i++)
                {
                    GameObject[] targets = GameObject.FindGameObjectsWithTag(m_data.EnemyTags[i]);

                    for (int j = 0; j < targets.Length; j++)
                    {
                        LazyBot.Area.Detection.DetectionArea detectionArea = targets[j].GetComponent<LazyBot.Area.Detection.DetectionArea>();

                        int k;
                        for (k = 0; k < m_onTargetDetection.Length; k++)
                            if (!m_onTargetDetection[k].Validate(this, detectionArea)) break;

                        if (k == m_onTargetDetection.Length)
                            m_onTargetUpdate.Invoke(this, detectionArea);
                    }
                }
                
            }
        }
    }
}
