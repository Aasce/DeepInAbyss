namespace Asce.Game.Entities.Characters
{
    public class CharacterInventory : CreatureInventory, IHasOwner<Character>
    {
        public new Character Owner
        {
            get => base.Owner as Character;
            set => base.Owner = value;
        }
    }
}