using Asce.Managers.Attributes;
using UnityEngine;

namespace Asce.Game.StatusEffects
{
    [CreateAssetMenu(menuName = "Asce/Status Effects/Information", fileName = "Status Effect Information")]
    public class SO_StatusEffectInformation : ScriptableObject
    {
        [SerializeField] protected string _name;
        [SerializeField, TextArea] protected string _description;
        [SerializeField, SpritePreview] protected Sprite _icon;

        [Space]
        [SerializeField] protected EffectType _type;
        [SerializeField] protected EffectApplyType _applyType;


        public string Name => _name;
        public string Description => _description;
        public Sprite Icon => _icon;

        public EffectType Type => _type;
        public EffectApplyType ApplyType => _applyType;
    }
}
