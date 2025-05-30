using UnityEngine;

namespace Asce.Game.Players
{
    public class PlayerUI : MonoBehaviour, IPlayerComponent
    {
        // Ref
        [SerializeField, HideInInspector] private Player _player;

        [SerializeField] private Canvas _canvas;

        public Player Player
        {
            get => _player;
            set => _player = value;
        }

        public Canvas Canvas => _canvas;

    }
}