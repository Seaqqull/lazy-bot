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
        [SerializeField] private DetectionArea[] m_tracingAreas;

        /// <summary>
        /// Wether to load detection areas at startup.
        /// </summary>
        private bool m_isAutoLoad;

        public DetectionArea[] TracingAreas
        {
            get
            {
                return this.m_tracingAreas;
            }
        }
        public int Length
        {
            get { return m_tracingAreas.Length; }
        }


        private void Awake()
        {
            if (m_isAutoLoad)
                m_tracingAreas = GetComponentsInChildren<DetectionArea>();
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

            for (int i = 0; i < m_tracingAreas.Length; i++)
                m_tracingAreas[i].enabled = isActive;
        }        


        /// <summary>
        /// Stitches activation state of specific area.
        /// </summary>
        /// <param name="index">index of area</param>
        /// <param name="isActive">activation state</param>
        public void SwitchArea(int index, bool isActive)
        {
            if (!enabled) return;

            m_tracingAreas[index].enabled = isActive;
        }

        /// <summary>
        /// Stitches activation state of specific area.
        /// </summary>
        /// <param name="name">name of area</param>
        /// <param name="isActive">activation state</param>
        public void SwitchArea(string name, bool isActive)
        {
            if (!enabled) return;

            for (int i = 0; i < m_tracingAreas.Length; i++)
                if (m_tracingAreas[i].Name == name)
                {
                    m_tracingAreas[i].enabled = isActive;
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
            if (index >= m_tracingAreas.Length && index < 0)
                return null;
            return m_tracingAreas[index].transform;
        }

        public void SetHealthLink(Action<float> onDamage)
        {
            for (int i = 0; i < m_tracingAreas.Length; i++)
                m_tracingAreas[i].OnDamage = onDamage;
        }

        public void SetSoundLink(Func<Vector3, float> onSound)
        {
            for (int i = 0; i < m_tracingAreas.Length; i++)
                m_tracingAreas[i].OnSound = onSound;
        }

        public LazyBot.Area.Data.HitAreaState GetPointState(int index)
        {
            if ((index >= m_tracingAreas.Length) && (index < 0))
                return LazyBot.Area.Data.HitAreaState.Unknown;
            return m_tracingAreas[index].State;
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<DetectionArea> GetEnumerator()
        {
            foreach (var area in m_tracingAreas)
            {
                yield return area;
            }
        }
        
    }
}
