using UnityEngine;

namespace Asce.Managers.UIs
{
    public abstract class UIObject : MonoBehaviour, IUIObject
    {
        public RectTransform RectTransform => transform as RectTransform;
    }
}