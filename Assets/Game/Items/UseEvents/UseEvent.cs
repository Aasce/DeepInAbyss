using Asce.Managers;
using UnityEngine;

namespace Asce.Game.Items
{
    public abstract class UseEvent : GameComponent
    {
        [SerializeField, TextArea(2, 6)] protected string _description = string.Empty;

        public string Description => _description;

        public abstract bool OnUse(object sender, UseEventArgs args);
    }
}
