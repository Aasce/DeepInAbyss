using Asce.Game.Entities.Enemies.Category;
using UnityEditor;

namespace Asce.Editors
{
    [CustomEditor(typeof(Spider_Enemy))]
    public class Spider_EnemyEditor : EnemyEditor
    {
        protected Spider_Enemy _spiderEnemy;

        protected override void OnEnable()
        {
            base.OnEnable();
            _spiderEnemy = (Spider_Enemy)target;
        }

    }
}