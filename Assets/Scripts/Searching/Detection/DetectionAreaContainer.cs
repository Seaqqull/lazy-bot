using UnityEngine;
using System;

namespace LazyBot.Area.Detection
{
    public class DetectionAreaContainer : MonoBehaviour
    {
        private DetectionArea[] m_tracingAreas;

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


        void Awake()
        {
            m_tracingAreas = GetComponentsInChildren<DetectionArea>();
        }


        public void SwitchAllAreas(bool isActive)
        {
            for (int i = 0; i < m_tracingAreas.Length; i++)
                m_tracingAreas[i].enabled = isActive;
        }        

        public void SwitchArea(int index, bool isActive)
        {
            m_tracingAreas[index].enabled = isActive;
        }

        public void SwitchArea(string name, bool isActive)
        {
            for (int i = 0; i < m_tracingAreas.Length; i++)
                if (m_tracingAreas[i].Name == name)
                {
                    m_tracingAreas[i].enabled = isActive;
                    break;
                }
        }


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
    }
}
