using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LazyBot.Target.Data
{
    [System.Flags]
    public enum TargetProperty
    {
        Undefined = 0,
        Transform = 1 << 0,
        Health = 1 << 1,
        Loudness = 1 << 2
    }

    public class Target
    {
        private LazyBot.Area.Data.ObservationType m_observationType;
        private Dictionary<TargetProperty, dynamic> m_properties;
        private ushort m_priority;
        private uint m_id;
        
        public LazyBot.Area.Data.ObservationType ObservationType
        {
            get { return this.m_observationType; }
        }
        public Dictionary<TargetProperty, dynamic> Properties
        {
            get
            {
                return this.m_properties ?? 
                    (this.m_properties = new Dictionary<TargetProperty, dynamic>());
            }
        }
        public ushort Priority
        {
            get { return this.m_priority; }
        }
        public uint Id
        {
            get { return this.m_id; }
        }

        public Target(uint id, ushort priority, LazyBot.Area.Data.ObservationType observationType)
        {
            m_observationType = observationType;
            m_priority = priority;
            m_id = id;            
        }
    }

    public class TargetContainer
    {
        private LazyBot.Target.TargetTypeSO m_type;
        private Dictionary<uint, List<Target>> m_targets;//SA id

        public LazyBot.Target.TargetTypeSO Type
        {
            get { return this.m_type; }
        }


        public void Erase(uint searchingAreaId)
        {
            if (!m_targets.ContainsKey(searchingAreaId)) return;

            m_targets.Remove(searchingAreaId);
        }

        public List<Target> Cut(uint searchingAreaId)
        {
            if (!m_targets.ContainsKey(searchingAreaId)) return null;

            List<Target>  Targets = m_targets[searchingAreaId];
            m_targets[searchingAreaId] = new List<Target>();

            return Targets;
        }

        public List<Target> Get(uint searchingAreaId)
        {
            if (!m_targets.ContainsKey(searchingAreaId)) return null;

            return m_targets[searchingAreaId];
        }

        public List<Target> Remove(uint searchingAreaId)
        {
            if (!m_targets.ContainsKey(searchingAreaId)) return null;

            List<Target> Targets = m_targets[searchingAreaId];
            m_targets.Remove(searchingAreaId);

            return Targets;
        }

        public void Add(uint searchingAreaId, Target target)
        {
            if (!m_targets.ContainsKey(searchingAreaId))
                m_targets.Add(searchingAreaId, new List<Target>());

            m_targets[searchingAreaId].Add(target);
        }
    }
}