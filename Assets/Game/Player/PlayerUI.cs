using Asce.Game.Stats;
using Asce.Game.UIs;
using Asce.Game.UIs.Characters;
using Asce.Managers;
using Asce.Managers.Attributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Asce.Game.Players
{
    public class PlayerUI : MonoBehaviour, IPlayerComponent
    {
        // Ref
        [SerializeField, Readonly] private Player _player;

        [SerializeField] private Canvas _canvas;
        [SerializeField] private UICreatureInformation _characterInformation;

        private readonly List<RaycastResult> _raycastResults = new();

        public Player Player
        {
            get => _player;
            set => _player = value;
        }

        public Canvas Canvas => _canvas;
        public UICreatureInformation CharacterInformation => _characterInformation;


        private void Start()
        {
            CharacterInformation.SetCreature(Player.ControlledCreature);
            this.SetInventoryWindow();
            Player.OnControlledCreatureChanged += Player_OnCharacterChanged;
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

        public bool ToggleInventory()
        {
            UIs.Inventories.UIInventoryWindow inventoryWindow = UIScreenCanvasManager.Instance.WindowsController.GetWindow<UIs.Inventories.UIInventoryWindow>();
            if (inventoryWindow != null)
            {
                inventoryWindow.Toggle();
                return inventoryWindow.IsShow;
            }
            return false;
        }

        private void Player_OnCharacterChanged(object sender, ValueChangedEventArgs<Entities.ICreature> args)
        {
            CharacterInformation.SetCreature(Player.ControlledCreature);
            this.SetInventoryWindow();
        }

        private void SetInventoryWindow()
        {
            UIs.Inventories.UIInventoryWindow inventoryWindow = UIScreenCanvasManager.Instance.WindowsController.GetWindow<UIs.Inventories.UIInventoryWindow>();
            if (inventoryWindow != null) inventoryWindow.SetCreature(Player.ControlledCreature);
        }
    }
}