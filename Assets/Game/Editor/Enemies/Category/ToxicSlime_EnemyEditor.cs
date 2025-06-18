using Asce.Game.Combats;
using Asce.Game.Entities.Enemies.Category;
using UnityEditor;
using UnityEngine;

namespace Asce.Editors
{
    [CustomEditor(typeof(ToxicSlime_Enemy))]
    public class ToxicSlime_EnemyEditor : Slime_EnemyEditor
    {
        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnSceneGUI()
        {
            base.OnSceneGUI();
        }
    }
}