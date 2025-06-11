using Asce.Game.Entities;
using Asce.Game.Entities.Enemies;
using Asce.Game.UIs.Creatures;
using Asce.Game.UIs.Stats;
using UnityEngine;

namespace Asce.Game.UIs
{
    public static class WorldUIExtension
    {
        public static void SetUIForPlayer(this UICreatureCanvas creatureCanvas, ICreature creature)
        {
            if (creatureCanvas == null) return;
            if (creature == null) return;

            if (creature.IsControlByPlayer())
            {
                creatureCanvas.NameText.color = UIManager.Instance.TextData.CharacterNameColor;
                creatureCanvas.SetShowName("You");
                creatureCanvas.HealthBar.FillImage.color = UIStatManager.Instance.Data.HealthBarCharacterColor;
                return;
            }

            Color nameColor;
            Color healthBarColor;
            if (creature is Enemy)
            {
                nameColor = UIManager.Instance.TextData.EnemyNameColor;
                healthBarColor = UIStatManager.Instance.Data.HealthBarEnemyColor;
            }
            else
            {
                nameColor = UIManager.Instance.TextData.TextColor;
                healthBarColor = UIStatManager.Instance.Data.HealthBarNPCColor;
            }

            creatureCanvas.ResetBaseName();
            creatureCanvas.NameText.color = nameColor;
            creatureCanvas.HealthBar.FillImage.color = healthBarColor;
        }
    }
}