using Asce.Game.Inventories;
using Asce.Game.UIs.Inventories;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Asce.Game.UIs
{
    public interface IUISlotController
    {
        public event Action<object, int> OnFocusAt;

        public void FocusAt(int index);
        public void ShowMenuContextAt(int index);

        public void BeginDragItem(UIItem sender, PointerEventData eventData);
        public void EndDragItem(UIItem sender, PointerEventData eventData);
    }
}
