using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;


namespace LazyBot.Area.Detection
{
    /// <summary>
    /// Used as container for storing/accessing detection areas.
    /// </summary>
    public class DetectionAreaContainer : MonoBehaviour, IEnumerable<DetectionArea>
    {
        [SerializeField] private DetectionArea[] _tracingAreas;


        private IReadOnlyList<DetectionArea> _tracingAreasRestricted;


        /// <summary>
        /// Wether to load detection areas at startup.
        /// </summary>
        private bool _isAutoLoad;


        public IReadOnlyList<DetectionArea> TracingAreas
        {
            get
            {
                return this._tracingAreasRestricted ??
                    (this._tracingAreasRestricted = this._tracingAreas);
            }
        }
        public int Length
        {
            get { return _tracingAreas.Length; }
        }


        private void Awake()
        {
            if (_isAutoLoad)
                _tracingAreas = GetComponentsInChildren<DetectionArea>();
        }

        private void OnEnable()
        {
            SwitchAllAreas(true);
        }

        private void OnDisable()
        {
            SwitchAllAreas(false);
        }


        /// <summary>
        /// Switches active state of all areas.
        /// </summary>
        /// <param name="isActive">activation state</param>
        private void SwitchAllAreas(bool isActive)
        {
            if (!enabled) return;

            for (int i = 0; i < _tracingAreas.Length; i++)
                _tracingAreas[i].enabled = isActive;
        }        


        /// <summary>
        /// Stitches activation state of specific area.
        /// </summary>
        /// <param name="index">index of area</param>
        /// <param name="isActive">activation state</param>
        public void SwitchArea(int index, bool isActive)
        {
            if (!enabled) return;

            _tracingAreas[index].enabled = isActive;
        }

        /// <summary>
        /// Stitches activation state of specific area.
        /// </summary>
        /// <param name="name">name of area</param>
        /// <param name="isActive">activation state</param>
        public void SwitchArea(string name, bool isActive)
        {
            if (!enabled) return;

            for (int i = 0; i < _tracingAreas.Length; i++)
                if (_tracingAreas[i].Name == name)
                {
                    _tracingAreas[i].enabled = isActive;
                    break;
                }
        }

        /// <summary>
        /// Returns position of specific area in scene.
        /// </summary>
        /// <param name="index">index of area</param>
        /// <returns>Position in scene</returns>
        public Transform GetPointTransform(int index)
        {
            if (index >= _tracingAreas.Length && index < 0)
                return null;
            return _tracingAreas[index].transform;
        }

        public void SetHealthLink(Action<float> onDamage)
        {
            for (int i = 0; i < _tracingAreas.Length; i++)
                _tracingAreas[i].OnDamage = onDamage;
        }

        public void SetSoundLink(Func<Vector3, float> onSound)
        {
            for (int i = 0; i < _tracingAreas.Length; i++)
                _tracingAreas[i].OnSound = onSound;
        }

        public LazyBot.Area.Data.HitAreaState GetPointState(int index)
        {
            if ((index >= _tracingAreas.Length) && (index < 0))
                return LazyBot.Area.Data.HitAreaState.Unknown;
            return _tracingAreas[index].State;
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<DetectionArea> GetEnumerator()
        {
            foreach (var area in _tracingAreas)
            {
                yield return area;
            }
        }
        
    }
}
