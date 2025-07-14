using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.Enviroments
{
    [System.Serializable]
    public class Notice
    {
        [SerializeField] protected string _name = string.Empty;
        [SerializeField, TextArea] protected string _description = string.Empty;

        public string Name 
        { 
            get => _name; 
            set => _name = value;
        }

        public string Description
        {
            get => _description;
            set => _description = value;
        }
    }
}