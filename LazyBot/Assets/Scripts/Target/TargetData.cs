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

        public bool Equals(PropertyContacts obj)
        {
            return (Id == obj.Id);
        }

        public override bool Equals(System.Object obj)
        {
            if ((obj == null) ||
                !(this.GetType().Equals(obj.GetType())))
                return false;

            return this.Equals(obj as PropertyContacts);
        }
    }

    public class TargetInfo
    {
        public class TargetContainer
        {
            private Dictionary<uint, List<Target>> _targets;//<searchingAreaId, >

            public Dictionary<uint, List<Target>> Targets
            {
                get
                {
                    return _targets ??
                        (_targets = new Dictionary<uint, List<Target>>());
                }
            }


            public void Erase(uint searchingAreaId)
            {
                if (Targets.ContainsKey(searchingAreaId))
                    _targets.Remove(searchingAreaId);
            }

            public void AddArea(uint searchingAreaId)
            {
                if (!Targets.ContainsKey(searchingAreaId))
                    _targets.Add(searchingAreaId, new List<Target>());
            }

            public List<Target> Cut(uint searchingAreaId)
            {
                List<Target> targets;

                if (!Targets.TryGetValue(searchingAreaId, out targets)) return null;

                _targets.Remove(searchingAreaId);
                return targets;
            }

            public List<Target> Get(uint searchingAreaId)
            {
                List<Target> targets;

                Targets.TryGetValue(searchingAreaId, out targets);
                return targets;
            }

            public List<Target> Remove(uint searchingAreaId)
            {
                return Cut(searchingAreaId);
            }

            public void Erase(uint searchingAreaId, int targetIndex)
            {
                List<Target> targets;

                if (!(Targets.TryGetValue(searchingAreaId, out targets)) ||
                    (targetIndex >= targets.Count)) return;

                targets.RemoveAt(targetIndex);
            }

            public Target Cut(uint searchingAreaId, int targetIndex)
            {
                List<Target> targets;
                Target target;

                if (!(Targets.TryGetValue(searchingAreaId, out targets)) ||
                    (targetIndex >= targets.Count)) return null;

                target = targets[targetIndex];
                targets.RemoveAt(targetIndex);
                return target;
            }

            public Target Get(uint searchingAreaId, int targetIndex)
            {
                List<Target> targets;

                if (!(Targets.TryGetValue(searchingAreaId, out targets)) ||
                    (targetIndex >= targets.Count)) return null;

                return targets[targetIndex];
            }

            public void AddTarget(uint searchingAreaId, Target target)
            {
                AddArea(searchingAreaId);

                _targets[searchingAreaId].Add(target);
            }

            public Target Remove(uint searchingAreaId, int targetIndex)
            {                
                return Cut(searchingAreaId, targetIndex);
            }

            public void AddTarget(uint searchingAreaId, List<(uint, dynamic)> data)
            {
                AddArea(searchingAreaId);

                Target newIndo = new Target();
                foreach (var property in data)
                {
                    newIndo.Add(property.Item1, property.Item2);
                }
                _targets[searchingAreaId].Add(newIndo);
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
                TargetContainer targetContainer;

                Data.TryGetValue(key, out targetContainer);
                return targetContainer;
            }            
        }

        public int Count
        {
            get
            {
                int count = 0;
                foreach (var targets in Data.Values)
                {
                    foreach (var target in targets.Targets.Values)
                    {
                        count += target.Count;
                    }                    
                }
                return count;
            }
        }


        public void AddType(LazyBot.Target.Property.TargetTypeSO newType)
        {
            if (!Data.ContainsKey(newType))
                _data.Add(newType, new TargetContainer());
        }        

    }
}
