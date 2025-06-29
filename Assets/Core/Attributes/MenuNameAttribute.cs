using System;
using UnityEngine;

namespace Asce.Managers.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MenuNameAttribute : PropertyAttribute
    {
        public string DisplayName { get; }

        public MenuNameAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }
}