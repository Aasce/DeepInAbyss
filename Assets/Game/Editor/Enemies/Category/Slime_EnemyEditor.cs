using Asce.Game.Entities.Enemies.Category;
using UnityEditor;

namespace Asce.Editors
{
    [CustomEditor(typeof(Slime_Enemy))]
    public class Slime_EnemyEditor : EnemyEditor
    {
        protected Slime_Enemy _slimeEnemy;

        protected override void OnEnable()
        {
            base.OnEnable();
            _slimeEnemy = (Slime_Enemy)target;
        }

    }
}