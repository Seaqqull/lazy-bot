using System;
using UnityEngine;

namespace LazyBot.Target
{
    [CreateAssetMenu(menuName = "Target/Type")]
    public class TargetTypeSO : ScriptableObject, IEquatable<TargetTypeSO>
    {
        [SerializeField] private string m_name;
        [SerializeField] [EnumSelector] private LazyBot.Target.Data.TargetProperty mask;

        [SerializeField] private bool m_dataOnDetection;

        public bool DataOnDetection
        {
            get { return this.m_dataOnDetection; }
        }
        public string Name
        {
            get { return this.m_name; }
        }


        public bool Equals(TargetTypeSO other)
        {
            return (this.m_name == other.m_name);
        }
    }
}