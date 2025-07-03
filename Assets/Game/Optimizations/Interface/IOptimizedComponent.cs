namespace Asce.Game
{
    public interface IOptimizedComponent
    {
        public bool IsActive { get; }

        public OptimizeBehavior OptimizeBehavior { get; }

        public void SetActivate(bool state);
    }
}