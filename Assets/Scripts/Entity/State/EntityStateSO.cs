using UnityEngine;

/// <summary>
/// Entity state methods.
/// </summary>
public abstract class EntityStateSO : ScriptableObject
{
    /// <summary>
    /// Performs action of specific state, when it's activated.
    /// </summary>
    /// <param name="controller">Reference on state listener.</param>
    public abstract void Excute(LazyBot.Entity.EntityController controller);
    /// <summary>
    /// Checks is current state can be activated.
    /// </summary>
    /// <param name="controller">Reference on state listener.</param>
    /// <returns>Is validation successful.</returns>
    public abstract bool Validate(LazyBot.Entity.EntityController controller);
    /// <summary>
    /// Executes on state exit.
    /// </summary>
    /// <param name="controller">Reference on state listener.</param>
    public abstract void OnStateExit(LazyBot.Entity.EntityController controller);
    /// <summary>
    /// Executes on state enter.
    /// </summary>
    /// <param name="controller">Reference on state listener.</param>
    public abstract void OnStateEnter(LazyBot.Entity.EntityController controller);
}
