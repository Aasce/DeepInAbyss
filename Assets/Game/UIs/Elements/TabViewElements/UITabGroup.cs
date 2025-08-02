using Asce.Managers.UIs;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.UIs
{
    public class UITabGroup : UIObject
    {
        [SerializeField] protected List<TabPair> _tabs = new();
        protected TabPair _currentTab;

        public event Action<object, TabPair> OnTabSelected;

        protected virtual void Start()
        {
            for (int i = 0; i < _tabs.Count; i++) 
            {
                TabPair pair = _tabs[i];
                if (pair == null) continue;
                if (pair.Tab == null) continue;
                pair.Tab.TabGroup = this;
                pair.Tab.Index = i;
                if (pair.Viewport != null)
                {
                    pair.Viewport.TabGroup = this;
                }
            }

            this.Select(0); // Default
        }

        public virtual void Select(int index)
        {
            if (index < 0 || index >= _tabs.Count) return;
            TabPair pair = _tabs[index];
            if (pair == null) return;
            if (pair == _currentTab) return;

            foreach (TabPair tabPair in _tabs)
            {
                if (tabPair == null) continue;
                if (tabPair.Tab == null) continue;

                if (tabPair.Viewport == null) continue;
                if (tabPair == pair)
                {
                    tabPair.Viewport.Show();
                    _currentTab = tabPair;
                }
                else tabPair.Viewport.Hide();
            }

            OnTabSelected?.Invoke(this, _currentTab);
        }

        public virtual void Select(UITabButton tabButton)
        {
            if (tabButton == null) return;
            this.Select(tabButton.Index);
        }
    }
}
