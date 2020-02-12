using LazyBot.Target.Data;
using UnityEngine;

namespace LazyBot.Target.Property
{        
    /// <summary>
    /// Specific target property
    /// </summary>
    public abstract class TargetPropertySO : ScriptableObject
    {
#pragma warning disable 0649
        [SerializeField] private PropertyContacts _data;
#pragma warning restore 0649

        public string Name
        {
            get { return this._data.Name; }
        }
        public uint Id
        {
            get { return this._data.Id; }
        }


        public abstract dynamic GetProperty(LazyBot.Entity.EntityController entity);
    }
}
