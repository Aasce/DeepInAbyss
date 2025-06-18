namespace Asce.Game.Equipments
{ 
    public interface IEquipmentSlot
    {
        IEquipmentController EquipmentOwner { get; set; }
    }
}