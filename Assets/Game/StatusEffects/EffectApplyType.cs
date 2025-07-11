namespace Asce.Game.StatusEffects
{
    public enum EffectApplyType
    {
        Default = 0,                // Use system default behavior
        StackSameSender = 1,        // Stack only if Sender is the same
        StackAnySender = 2,         // Stack even if Sender is different
        Reset = 3,                  // Reset the effect duration, do not stack
    }
}