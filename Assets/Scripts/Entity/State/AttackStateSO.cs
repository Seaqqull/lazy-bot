using LazyBot.Entity;
using UnityEngine;

[CreateAssetMenu(menuName = "State/Attack")]
public class AttackStateSO : EntityStateSO
{
    public override void Excute(EntityController controller)
    {
        Debug.Log("See the target");
    }

    public override void OnStateEnter(EntityController controller)
    {
        Debug.Log("Saw target");
    }

    public override void OnStateExit(EntityController controller)
    {
        Debug.Log("Don't see target");
    }

    public override bool Validate(EntityController controller)
    {
        return (controller.Targets.Count != 0);
    }
}
