namespace Asce.Game
{
    /// <summary>
    ///     Defines how an optimized component should behave 
    ///     when it is inside or outside the camera's view.
    /// </summary>
    public enum OptimizeBehavior
    {
        /// <summary>
        ///     Do nothing when the component enters or leaves the camera's view.
        /// </summary>
        None,

        /// <summary>
        ///     Automatically disable the component when it is outside the camera's view.
        /// </summary>
        DeactivateOutsideView,

        /// <summary>
        ///     Automatically enable the component when it is outside the camera's view.
        /// </summary>
        ActivateOutsideView
    }
}