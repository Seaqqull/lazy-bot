using UnityEngine;
using System;

namespace LazyBot.Target.Property
{
    /// <summary>
    /// Contains all target properties needed to be gotten from the detected target
    /// </summary>
    [CreateAssetMenu(menuName = "Target/Type")]
    public class TargetTypeSO : ScriptableObject, IEquatable<TargetTypeSO>
    {
        [SerializeField] private string m_name;
        [SerializeField] private LazyBot.Entity.Data.IntMask m_mask;

        [SerializeField] private bool m_dataOnDetection;

        public LazyBot.Entity.Data.IntMask Mask
        {
            get { return this.m_mask; }
        }
        public bool DataOnDetection
        {
            get { return this.m_dataOnDetection; }
        }        
        public string Name
        {
            get { return this.m_name; }
        }


        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj as TargetTypeSO);
        }


        public bool Equals(TargetTypeSO other)
        {
            return (this.m_name == other.m_name);
        }
    }
}
