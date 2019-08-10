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
        public class SearchingAreaEvent : UnityEvent<SearchingArea> { }
        [System.Serializable]
        public class TargetUpdateEvent : UnityEvent<SearchingArea, LazyBot.Area.Detection.DetectionArea> { }

        [SerializeField] private LazyBot.Area.Data.ObservationType _type = LazyBot.Area.Data.ObservationType.Undefined;
        [SerializeField] private LazyBot.Target.Property.TargetTypeSO _targetType;

        /// <summary>
        /// Update frequency of target detection.
        /// </summary>
        [SerializeField] private FloatReference _updateRate;
        [SerializeField] [Range(0, ushort.MaxValue)] private ushort _priority = 0;

        [SerializeField] private DetectionValidatorSO[] _onTargetDetection;
        [SerializeField] private SearchingAreaEvent _onInit;
        [SerializeField] private TargetUpdateEvent _onTargetUpdate;
        [SerializeField] private SearchingAreaEvent _onTargetClear;
        [SerializeField] private SearchingAreaEvent _onDrawGizmo;

        /// <summary>
        /// Area characteristics.
        /// </summary>
        [SerializeField] private LazyBot.Area.Data.AreaData _data;

        /// <summary>
        /// Reference on target detection function.
        /// </summary>
        private Coroutine _searchingCorotation;
        private static uint _idCounter = 0;
        private uint _id;

        public LazyBot.Target.Property.TargetTypeSO TargetType
        {
            get { return this._targetType; }
        }
        public LazyBot.Area.Data.ObservationType Type
        {
            get { return this._type; }
        }
        public LazyBot.Area.Data.AreaData Data
        {
            get { return this._data; }
        }
        public Transform Socket
        {
            get { return this._data.Socket; }
        }
        public ushort Priority
        {
            get { return this._priority; }
        }        
        public uint Id
        {
            get { return this._id; }
        }
        

        private void Awake()
        {
            _id = _idCounter++;

            _data.Socket = _data.Socket ?? this.transform;
        }

        private void Start()
        {
#if UNITY_EDITOR
            if (_targetType == null)
            {
                Debug.LogError("Target type empty!Need to set it to perform target updates.", gameObject);
            }
#endif
            _onInit.Invoke(this);
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
            if (!_data.Socket)
                _data.Socket = transform;

            _onDrawGizmo.Invoke(this);
#endif
        }


        private void SetIsActive(bool isActive)
        {
            if (isActive)
            {
                _searchingCorotation = StartCoroutine("FindTargetsWithDelay", _updateRate.Value);
            }
            else if ((!isActive) && (_searchingCorotation != null))
            {
                StopCoroutine(_searchingCorotation);
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

                _onTargetClear.Invoke(this);

                for (int i = 0; i < _data.EnemyTags.Length; i++)
                {
                    GameObject[] targets = GameObject.FindGameObjectsWithTag(_data.EnemyTags[i]);

                    for (int j = 0; j < targets.Length; j++)
                    {
                        int k;
                        LazyBot.Area.Detection.DetectionAreaContainer areas = targets[j].
                            GetComponent<LazyBot.Entity.EntityController>()?.DetectionAreas;

                        foreach (var dArea in areas)
                        {                            
                            for (k = 0; k < _onTargetDetection.Length; k++)
                                if (!_onTargetDetection[k].Validate(this, dArea)) break;

                            // If one of the targets areas was detected, then there is no sense 
                            // to check other because we will grab the same data from them
                            
                            // Could be upgraded with detection mask(on each dArea)
                            // then break will be removed, because we'll get different data from each dArea
                            if ((k == _onTargetDetection.Length) && (_targetType != null))
                            {
                                _onTargetUpdate.Invoke(this, dArea);
                                break;
                            }                            
                        }                        
                    }
                }
                
            }
        }
    }
}
