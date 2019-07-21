using LazyBot.Target.Data;
using UnityEngine;

namespace LazyBot.Target.Property
{        
    /// <summary>
    /// Specific target property
    /// </summary>
    public abstract class TargetPropertySO : ScriptableObject
    {        
        [SerializeField] private PropertyContacts m_data;

        public string Name
        {
            get { return this.m_data.Name; }
        }
        public uint Id
        {
            get { return this.m_data.Id; }
        }


        public abstract dynamic GetProperty(LazyBot.Entity.EntityController entity);
    }
}
