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

        public bool Equals(TargetTypeSO obj)
        {
            return (this.m_name == obj.m_name);
        }

        public override bool Equals(System.Object obj)
        {
            if ((obj == null) ||
                !(this.GetType().Equals(obj.GetType())))
                return false;

            return this.Equals(obj as TargetTypeSO);
        }

    }
}
