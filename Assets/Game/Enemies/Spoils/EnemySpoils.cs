
namespace Asce.Game.Entities.Enemies
{
    public class EnemySpoils : CreatureSpoils, IHasOwner<Enemy>
    {
        public new Enemy Owner 
        { 
            get => base.Owner as Enemy; 
            set => base.Owner = value; 
        }
    }
}
