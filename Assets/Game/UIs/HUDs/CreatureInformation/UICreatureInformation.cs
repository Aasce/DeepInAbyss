using Asce.Game.Entities;
using Asce.Game.Stats;
using Asce.Game.UIs.Stats;
using Asce.Game.UIs.StatusEffects;
using Asce.Managers.Attributes;
using Asce.Managers.UIs;
using Asce.Managers.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Asce.Game.UIs.Characters
{
    public class UICreatureInformation : UIObject
    {
        [SerializeField, Readonly] protected UICreatureAvatar _avatar;
        [SerializeField, Readonly] protected UIResourceStatsInfoController _resourceStats;
        [SerializeField, Readonly] protected UIStatsInfoController _stats;
        [SerializeField, Readonly] protected UIStatusEffectController _statusEffect;

        [SerializeField] protected Button _toggleStatsInfoButton;

        protected ICreature _creature;

        public UICreatureAvatar Avatar => _avatar;
        public UIResourceStatsInfoController ResourceStats => _resourceStats;
        public UIStatsInfoController Stats => _stats;
        public UIStatusEffectController StatusEffect => _statusEffect;

        public Button ToggleStatsInfoButton => _toggleStatsInfoButton;


        protected override void RefReset()
        {
            base.RefReset();
            this.LoadComponent(out  _avatar);
            this.LoadComponent(out _resourceStats);
            this.LoadComponent(out _stats);
            this.LoadComponent(out _statusEffect);
        }

        protected virtual void Awake()
        {
            
        }

        protected virtual void Start()
        {
            this.RegisterToggleStatsInfoButton();
        }

        public virtual void SetCreature(ICreature creature)
        {
            this.Unregister();
            _creature = creature;
            this.Register();
        }

        protected virtual void Register()
        {
            if (_creature == null) return;

            ResourceStats.SetStats(
                _creature.Stats.HealthGroup.Health,
                _creature.Stats.DefenseGroup.Shield,
                _creature.Stats.Stamina,
                _creature.Stats.SustenanceGroup
            );

            Stats.AddStat(_creature.Stats.HealthGroup.HealScale);
            Stats.AddStat(_creature.Stats.Strength);
            Stats.AddStat(_creature.Stats.DefenseGroup.Armor);
            Stats.AddStat(_creature.Stats.DefenseGroup.Resistance);
            Stats.AddStat(_creature.Stats.Speed);
            if (_creature.Stats is IHasJumpForce hasJumpForce) 
                Stats.AddStat(hasJumpForce.JumpForce);
            Stats.AddStat(_creature.Stats.ViewRadius);

            if (_creature.StatusEffect != null)
                StatusEffect.SetStatusEffectController(_creature.StatusEffect.Controller);
        }

        protected virtual void Unregister()
        {
            if (_creature == null) return;
            Stats.ClearStats();
        }

        protected virtual void RegisterToggleStatsInfoButton()
        {
            if (ToggleStatsInfoButton == null) return;
            if (Stats == null) return;

            ToggleStatsInfoButton.onClick.AddListener(() => Stats.Toggle());
        }
    }
}