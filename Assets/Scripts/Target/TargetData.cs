using System.Collections.Generic;
using System;

namespace LazyBot.Target.Data
{    
    using Target = Dictionary<uint, dynamic>;//<propertyId, targetData>

    [System.Serializable]
    public class PropertyContacts : IEquatable<PropertyContacts>
    {
        public string Name;
        public uint Id;


        public override int GetHashCode()
        {
            return Id.GetHashCode() ^ Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj as PropertyContacts);
        }

        public bool Equals(PropertyContacts other)
        {
            return (Id == other.Id);
        }
    }

    public class TargetInfo
    {
        public class TargetContainer
        {
            private Dictionary<uint, List<Target>> m_targets;//<searchingAreaId, >

            public Dictionary<uint, List<Target>> Targets
            {
                get
                {
                    return m_targets ??
                        (m_targets = new Dictionary<uint, List<Target>>());
                }
            }


            public void Erase(uint searchingAreaId)
            {
                if (!Targets.ContainsKey(searchingAreaId)) return;

                m_targets.Remove(searchingAreaId);
            }

            public void AddArea(uint searchingAreaId)
            {
                if (!Targets.ContainsKey(searchingAreaId))
                    m_targets.Add(searchingAreaId, new List<Target>());
            }

            public List<Target> Cut(uint searchingAreaId)
            {
                if (!Targets.ContainsKey(searchingAreaId)) return null;

                List<Target> targets = m_targets[searchingAreaId];
                m_targets.Remove(searchingAreaId);

                return targets;
            }

            public List<Target> Get(uint searchingAreaId)
            {
                if (!Targets.ContainsKey(searchingAreaId)) return null;

                return m_targets[searchingAreaId];
            }

            public List<Target> Remove(uint searchingAreaId)
            {
                if (!Targets.ContainsKey(searchingAreaId)) return null;

                List<Target> targets = m_targets[searchingAreaId];
                m_targets.Remove(searchingAreaId);

                return targets;
            }

            public void Erase(uint searchingAreaId, int targetIndex)
            {
                if (!Targets.ContainsKey(searchingAreaId) ||
                    targetIndex >= m_targets[searchingAreaId].Count) return;

                m_targets[searchingAreaId].RemoveAt(targetIndex);
            }

            public Target Cut(uint searchingAreaId, int targetIndex)
            {
                if (!Targets.ContainsKey(searchingAreaId) ||
                    targetIndex >= m_targets[searchingAreaId].Count) return null;

                Target target = m_targets[searchingAreaId][targetIndex];
                m_targets[searchingAreaId].RemoveAt(targetIndex);

                return target;
            }

            public Target Get(uint searchingAreaId, int targetIndex)
            {
                if (!Targets.ContainsKey(searchingAreaId) ||
                    targetIndex >= m_targets[searchingAreaId].Count) return null;

                return m_targets[searchingAreaId][targetIndex];
            }

            public void AddTarget(uint searchingAreaId, Target target)
            {
                AddArea(searchingAreaId);

                m_targets[searchingAreaId].Add(target);
            }

            public Target Remove(uint searchingAreaId, int targetIndex)
            {
                if (!Targets.ContainsKey(searchingAreaId) ||
                    targetIndex >= m_targets[searchingAreaId].Count) return null;


                Target target = m_targets[searchingAreaId][targetIndex];
                m_targets[searchingAreaId].RemoveAt(targetIndex);

                return target;
            }

            public void AddTarget(uint searchingAreaId, List<(uint, dynamic)> data)
            {
                AddArea(searchingAreaId);

                Target newIndo = new Target();
                foreach (var property in data)
                {
                    newIndo.Add(property.Item1, property.Item2);
                }
                m_targets[searchingAreaId].Add(newIndo);
            }
            
        }
        
        private Dictionary<LazyBot.Target.Property.TargetTypeSO, TargetContainer> _data;

        public Dictionary<LazyBot.Target.Property.TargetTypeSO, TargetContainer> Data
        {
            get
            {
                return _data ??
                    (_data = new Dictionary<Property.TargetTypeSO, TargetContainer>());
            }
        }


        public TargetContainer this[LazyBot.Target.Property.TargetTypeSO key]
        {
            get
            {
                if (!Data.ContainsKey(key)) return null;
                return Data[key];
            }            
        }


        public void AddType(LazyBot.Target.Property.TargetTypeSO newType)
        {
            if (Data.ContainsKey(newType)) return;

            _data.Add(newType, new TargetContainer());
        }        
    }
}
