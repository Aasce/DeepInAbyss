using UnityEngine;

namespace Asce.Game.UIs
{
    [CreateAssetMenu(menuName = "Asce/UIs/Text Data", fileName = "Text Data")]
    public class SO_UITextData : ScriptableObject
    {
        [SerializeField] private Color _textColor;
        [SerializeField] private Color _characterNameColor;
        [SerializeField] private Color _enemyNameColor;


        public Color TextColor => _textColor;
        public Color CharacterNameColor => _characterNameColor;
        public Color EnemyNameColor => _enemyNameColor;
    }
}