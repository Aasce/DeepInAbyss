using TMPro;
using UnityEngine;

namespace Asce.Game.UIs.Stats
{
    public interface IUIStatBarHasText
    {
        public TextMeshProUGUI TextMesh { get; }
        public bool IsUseText { get; set; }
        public void TriggerText();
    }
}
