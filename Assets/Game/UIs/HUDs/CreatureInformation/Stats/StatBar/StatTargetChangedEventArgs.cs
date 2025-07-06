using Asce.Game.Stats;
using System;

namespace Asce.Game.UIs.Stats
{
    public class StatTargetChangedEventArgs<T> : EventArgs where T : Stat
    {
        private T _oldStat;
        private T _newStat;

        public T OldStat => _oldStat;
        public T NewStat => _newStat;

        public StatTargetChangedEventArgs() : this(null, null) { }
        public StatTargetChangedEventArgs(T newStat, T oldStat = null) 
        {
            _newStat = newStat;
            _oldStat = oldStat;
        }
    }
}