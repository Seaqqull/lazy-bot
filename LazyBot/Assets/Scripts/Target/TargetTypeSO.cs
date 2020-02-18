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
#pragma warning disable 0649
        [SerializeField] private string _name;
        /// <summary>
        ///  We don't use checking the existence of these properties in EntityManager to prevent it's linking with TargetType.
        /// </summary>
        [Tooltip("Ids of target's properties which can be taken from the \"Entity manager\".\nDivided by \"/\" sign. Indices of wrong properties will return null.")]
        [SerializeField] private LazyBot.Entity.Data.IntMask _mask;

        [SerializeField] private bool _dataOnDetection;
#pragma warning restore 0649

        public LazyBot.Entity.Data.IntMask Mask
        {
            get { return this._mask; }
        }
        public bool DataOnDetection
        {
            get { return this._dataOnDetection; }
        }        
        public string Name
        {
            get { return this._name; }
        }


        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public bool Equals(TargetTypeSO obj)
        {
            return (this._name == obj._name);
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
