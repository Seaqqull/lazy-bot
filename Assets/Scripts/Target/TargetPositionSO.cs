using LazyBot.Entity;
using UnityEngine;

namespace LazyBot.Target.Property
{
    [CreateAssetMenu(menuName = "Target/Propery/Position")]
    public class TargetPositionSO : TargetPropertySO
    {
        public override dynamic GetProperty(EntityController entity)
        {
            return entity.Transform.position;
        }
    }
}
