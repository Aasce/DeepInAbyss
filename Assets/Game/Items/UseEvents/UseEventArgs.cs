using Asce.Game.Entities;
using System;
using UnityEngine;

namespace Asce.Game.Items
{
    public class UseEventArgs : EventArgs
    {
        [SerializeField] private ICreature _user;
        [SerializeField] private Item _item;

        public ICreature User
        {
            get => _user;
            set => _user = value;
        }

        public Item Item
        {
            get => _item;
            set => _item = value;
        }
    }
}
