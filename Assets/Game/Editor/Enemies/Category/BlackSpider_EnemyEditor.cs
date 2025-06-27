using Asce.Game.Entities.Enemies.Category;
using UnityEditor;
using UnityEngine;

namespace Asce.Editors
{
    [CustomEditor(typeof(BlackSpider_Enemy))]
    public class BlackSpider_EnemyEditor : Spider_EnemyEditor
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