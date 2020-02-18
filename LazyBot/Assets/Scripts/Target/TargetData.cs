using System.Collections.Generic;
using System;

namespace LazyBot.Target.Data
{
    public class Target
    {
#pragma warning disable 0414
        private Dictionary<uint, dynamic> _data;
        // We assume, that by default if laterCall is null, then the data was filled directly
        private bool _isFilled = true; 
        private Action _lateCall;
#pragma warning restore 0414

        public Dictionary<uint, dynamic> Data
        {
            get
            {
                return this._data ?? 
                    (this._data = new Dictionary<uint, dynamic>());
            }
        }
        public bool IsFilled
        {
            get 
            {
                return this._isFilled;
            }
        }

        public bool FillData()
        {
            if (_lateCall == null) return false;

#pragma warning disable 0168
            try
            {
                _lateCall();
                _isFilled = true;

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
#pragma warning restore 0168
        }

        public void SetCall(Action lateCall)
        {
            _isFilled = false;
            _lateCall = lateCall;
        }

    }

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
            private IReadOnlyDictionary<uint, List<Target>> _targetsRestricted;
            private Dictionary<uint, List<Target>> _targets =
                new Dictionary<uint, List<Target>>();//<searchingAreaId, >

            public IReadOnlyDictionary<uint, List<Target>> Targets
            {
                get
                {
                    return this._targetsRestricted ??
                        (this._targetsRestricted = this._targets);
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

            public void AddTarget(uint searchingAreaId, Action action)
            {
                AddArea(searchingAreaId);

                Target newIndo = new Target();

                newIndo.SetCall(action);

                _targets[searchingAreaId].Add(newIndo);
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
                    newIndo.Data.Add(property.Item1, property.Item2);
                }
                _targets[searchingAreaId].Add(newIndo);
            }
        }

        private IReadOnlyDictionary<LazyBot.Target.Property.TargetTypeSO, TargetContainer> _dataRestricted;
        private Dictionary<LazyBot.Target.Property.TargetTypeSO, TargetContainer> _data = 
            new Dictionary<Property.TargetTypeSO, TargetContainer>();        


        public IReadOnlyDictionary<LazyBot.Target.Property.TargetTypeSO, TargetContainer> Data
        {
            get
            {
                return this._dataRestricted ??
                    (this._dataRestricted = this._data);
            }
        }


        public TargetContainer this[LazyBot.Target.Property.TargetTypeSO targetType]
        {
            get
            {                
                Data.TryGetValue(targetType, out TargetContainer targetContainer);
                return targetContainer;
            }            
        }

        public TargetContainer this[string typeName]
        {
            get
            {
                foreach(var entry in Data)
                {
                    if (entry.Key.name == typeName) 
                        return entry.Value;
                }
                return null;
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
