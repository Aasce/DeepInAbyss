using Asce.Managers;
using Asce.Managers.Attributes;
using UnityEngine;

namespace Asce.Game.Enviroments.HiddenAreas
{
    public class HiddenAreaOpener : InteractiveObject, IEnviromentComponent
    {
        [SerializeField, Readonly] protected HiddenArea _controller;


        public HiddenArea Controller
        {
            get => _controller;
            set => _controller = value;
        }
    }
}
