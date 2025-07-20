namespace Asce.Editors.Templates
{
    public static class EnemyScriptTemplate
    {
        public static string GetEnemyScript(string enemyClassName)
        {
            return $@"using Asce.Game.Combats;
using Asce.Game.Entities.Characters;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.Entities.Enemies.Category
{{
    public class {enemyClassName}_Enemy : Enemy
    {{

    }}
}}
";
        }

        public static string GetEnemyAIScript(string enemyClassName)
        {
            return $@"using Asce.Game.Entities.AIs;

namespace Asce.Game.Entities.Enemies.Category
{{
    public class {enemyClassName}_EnemyAI : EnemyAI
    {{

    }}
}}
";
        }

    }
}