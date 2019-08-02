using LazyBot.Entity;
using UnityEngine;

[CreateAssetMenu(menuName = "State/Patrol")]
public class PatrolStateSO : EntityStateSO
{    
    public override void Excute(EntityController controller) // patrolling the path
    {
        if(controller.IsSleep) return;

        float speed = 0.0f;

        if (controller.Distance() > 
            controller.NavigationPath.GetPoint(controller.NavigationIndex).AccuracyRadius)
        {
            speed = controller.NavigationPath.GetPoint(controller.NavigationIndex).MovementSpeed;
            controller.NavigationPath.CalculateSpeedOnPath(ref speed, controller.Transform.position);
        }        

        controller.UpdateSpeed(speed);
        controller.Move();

        if (controller.Distance() > 
            controller.NavigationPath.GetPoint(controller.NavigationIndex).AccuracyRadius)
            return;

        // if reached destination point
        switch (controller.NavigationPath.GetPoint(controller.NavigationIndex).Action)
        {
            case LazyBot.Navigation.Data.PointAction.Continue:
                // set nearest navigation path point as destination point
                controller.OnUpdatePathDestinationNext();
                break;
            case LazyBot.Navigation.Data.PointAction.Stop:
                // reset destination speed
                controller.UpdateSpeed(0.0f);
                // set controller to sleep
                controller.TurnSleep(controller.NavigationPath.GetPoint(controller.NavigationIndex).TransferDelay);
                // change destination point to next
                controller.OnUpdatePathDestinationNext();
                break;
        }
    }

    public override bool Validate(EntityController controller)
    {
        return ((controller.NavigationPath.Length != 0) && (controller.Targets.Count == 0));
    }
    
    public override void OnStateExit(EntityController controller) // reset navigation path point
    {
        controller.NavigationIndex = -1;
    }

    public override void OnStateEnter(EntityController controller) // set nearest navigation path point as target point
    {
        controller.OnUpdatePathDestination();
    }
}
