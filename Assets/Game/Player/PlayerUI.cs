using Asce.Game.UIs.Characters;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Asce.Game.Players
{
    public class PlayerUI : MonoBehaviour, IPlayerComponent
    {
        // Ref
        [SerializeField, HideInInspector] private Player _player;

        [SerializeField] private Canvas _canvas;
        [SerializeField] private UICharacterInformation _characterInformation;

        private readonly List<RaycastResult> _raycastResults = new();

        public Player Player
        {
            get => _player;
            set => _player = value;
        }

        public Canvas Canvas => _canvas;
        public UICharacterInformation CharacterInformation => _characterInformation;


        private void Start()
        {
            this.SetCharacterInformation();
            Player.OnCharacterChanged += Player_OnCharacterChanged;
        }

        public bool IsPointerOverScreenSpaceUI(Vector2 pointer)
        {
            if (EventSystem.current == null) return false;

            PointerEventData pointerData = new (EventSystem.current)
            {
                position = pointer
            };

            _raycastResults.Clear();
            EventSystem.current.RaycastAll(pointerData, _raycastResults);

            foreach (RaycastResult result in _raycastResults)
            {
                Canvas canvas = result.gameObject.GetComponentInParent<Canvas>();
                if (canvas != null && canvas.renderMode != RenderMode.WorldSpace)
                {
                    return true; // Pointer is over Screen Space UI
                }
            }

            return false; // Pointer is not over Screen Space UI
        }

        private void SetCharacterInformation()
        {
            if (CharacterInformation == null) return;
            if (CharacterInformation.ResourceStats == null) return;

            if (Player.Character == null) return;
            if (Player.Character.Stats == null) return;

            CharacterInformation.ResourceStats.SetStats(
                    Player.Character.Stats.HealthGroup.Health,
                    Player.Character.Stats.DefenseGroup.Shield,
                    Player.Character.Stats.Stamina,
                    Player.Character.Stats.SustenanceGroup
                );

            CharacterInformation.Stats.AddStat(Player.Character.Stats.HealthGroup.HealScale);
            CharacterInformation.Stats.AddStat(Player.Character.Stats.Strength);
            CharacterInformation.Stats.AddStat(Player.Character.Stats.DefenseGroup.Armor);
            CharacterInformation.Stats.AddStat(Player.Character.Stats.DefenseGroup.Resistance);
            CharacterInformation.Stats.AddStat(Player.Character.Stats.Speed);
            CharacterInformation.Stats.AddStat(Player.Character.Stats.JumpForce);
            CharacterInformation.Stats.AddStat(Player.Character.Stats.ViewRadius);
        }

        private void Player_OnCharacterChanged(object sender, Entities.Character character) => this.SetCharacterInformation();
    }
}