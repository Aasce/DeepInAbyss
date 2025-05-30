using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Entities
{
    public class Character : Creature, IControllableCharacter, IHasView<CharacterView>, IHasAction<CharacterAction>, IHasStats<CharacterStats, SO_CharacterBaseStats>, IHasUI<CharacterUI>
    {
        [SerializeField, HideInInspector] private CharacterUI _ui;

        public new CharacterPhysicController PhysicController => base.PhysicController as CharacterPhysicController;
        public new CharacterView View => base.View as CharacterView;
        public new CharacterAction Action => base.Action as CharacterAction;
        public new CharacterStats Stats => base.Stats as CharacterStats;
        public CharacterUI UI => _ui;


        protected override void Reset()
        {
            base.Reset();
            if (transform.LoadComponent(out _ui))
            {
                UI.Owner = this;
            }
        }

        protected override void Awake()
        {
            base.Awake();
        }


        protected override void Start()
        {
            base.Start();
            if (UI.HealthBar != null) UI.HealthBar.SetStat(Stats.HealthGroup.Health, Stats.DefenseGroup.Shield);
        }

    }
}