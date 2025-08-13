using Asce.Game.Items;
using Asce.Managers.UIs;
using TMPro;
using UnityEngine;

namespace Asce.Game.UIs.Inventories
{
    public class UIItemDescriptionInformation : UIObject
    {
        [SerializeField] protected TextMeshProUGUI _useOrEquipDescription;
        [SerializeField] protected TextMeshProUGUI _description;

        [SerializeField] protected RectTransform _divider;


        public virtual void Set(SO_ItemInformation information)
        {
            if (information == null)
            {
                if (_useOrEquipDescription != null) _useOrEquipDescription.gameObject.SetActive(false);
                if (_description != null) _description.gameObject.SetActive(false);
                return;
            }

            if (_useOrEquipDescription != null)
            {
                if (information.HasProperty(ItemPropertyType.Equippable))
                {
                    if (information.GetPropertyByType(ItemPropertyType.Equippable) is EquippableItemProperty equipProperty)
                    {
                        _useOrEquipDescription.gameObject.SetActive(true);
                        if (_divider != null) _divider.gameObject.SetActive(true);

                        string descriptionText = equipProperty.EquipEvent != null ? equipProperty.EquipEvent.GetDescription(isPretty: true) : string.Empty;
                        _useOrEquipDescription.text = descriptionText;
                    }
                }
                else if (information.HasProperty(ItemPropertyType.Usable))
                {
                    if (information.GetPropertyByType(ItemPropertyType.Usable) is UsableItemProperty useProperty)
                    {
                        _useOrEquipDescription.gameObject.SetActive(true);
                        if (_divider != null) _divider.gameObject.SetActive(true);

                        string descriptionText = useProperty.UseEvent != null ? useProperty.UseEvent.Description : string.Empty;
                        _useOrEquipDescription.text = descriptionText;
                    }
                }
                else
                {
                    _useOrEquipDescription.gameObject.SetActive(false);
                    if (_divider != null) _divider.gameObject.SetActive(false);
                }
            }

            if (_description != null)
            {
                _description.gameObject.SetActive(true);
                _description.text = information.Description;
            }
        }
    }
}