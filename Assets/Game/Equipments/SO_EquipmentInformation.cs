using Asce.Game.Items;
using UnityEngine;

namespace Asce.Game.Equipments
{
    [CreateAssetMenu(menuName = "Asce/Items/Equipment Information", fileName = "Equipment Information")]
    public class SO_EquipmentInformation : SO_ItemInformation
    {
        [SerializeField] protected EquipmentType _type = EquipmentType.None;

        public EquipmentType Type => _type;
    }
}