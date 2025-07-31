namespace Asce.Game.Quests
{
    public static class QuestExtension
    {
        public static bool IsNull(this Quest quest)
        {
            if (quest == null) return true;
            if (quest.Information == null) return true;
            return false;
        }
    }
}