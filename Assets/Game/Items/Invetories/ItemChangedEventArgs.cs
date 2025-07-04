using System;
using UnityEngine;

namespace Asce.Game.Items
{
    [Serializable]
    public class ItemChangedEventArgs : EventArgs
    {
        [SerializeField] protected int _index;

        public int Index => _index;

        public ItemChangedEventArgs(int index) 
        {
            _index = index;
        }
    }
}