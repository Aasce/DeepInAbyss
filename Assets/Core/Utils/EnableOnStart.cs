using System.Collections.Generic;
using UnityEngine;

namespace Asce.Managers.Utils
{
    public class EnableOnStart : MonoBehaviour
    {
        [SerializeField] private List<Behaviour> _behaviours = new();

        public List<Behaviour> Behaviours => _behaviours;


        private void Start()
        {
            foreach (Behaviour behaviour in Behaviours)
            {
                if (behaviour == null) continue;
                if (behaviour.enabled) continue;

                behaviour.enabled = true;
            }
        }
    }
}
