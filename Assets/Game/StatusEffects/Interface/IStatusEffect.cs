using Asce.Game.Entities;
using Asce.Managers.Utils;

namespace Asce.Game.StatusEffects
{
    public interface IStatusEffect
    {
        public string Name { get; }

        public ICreature Sender { get; }
        public ICreature Target { get; }

        public Cooldown Duration { get; }
        public int Level { get; }

        public void Apply();
        public void Tick(float deltaTime);
        public void Unapply();
    }
}