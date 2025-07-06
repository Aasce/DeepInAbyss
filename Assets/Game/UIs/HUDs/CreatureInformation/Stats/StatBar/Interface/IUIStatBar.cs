using Asce.Game.Stats;
using System;
using UnityEngine;

namespace Asce.Game.UIs.Stats
{
    public interface IUIStatBar<T> where T : Stat
    {
        public event Action<object, StatTargetChangedEventArgs<T>> OnStatTargetChanged;
        public void SetStat(T stat);
    }

    public interface IUIStatBar : IUIStatBar<Stat>
    {

    }
}
