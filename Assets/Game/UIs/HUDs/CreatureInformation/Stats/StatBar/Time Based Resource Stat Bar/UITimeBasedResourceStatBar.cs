using Asce.Game.Stats;
using Asce.Managers;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Asce.Game.UIs.Stats
{
    public class UITimeBasedResourceStatBar : UIResourceStatBar, IUIStatBar<TimeBasedResourceStat>, IUIStatBarHasText, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] protected TextMeshProUGUI _changedValueTextMesh;
        [SerializeField] protected bool _isShowChangedValueOnHover = true;

        public new event Action<object, StatTargetChangedEventArgs<TimeBasedResourceStat>> OnStatTargetChanged;
        
        
        public new TimeBasedResourceStat Stat
        {
            get => base.Stat as TimeBasedResourceStat;
            protected set
            {
                TimeBasedResourceStat oldStat = this.Stat;
                _stat = value;
                OnStatTargetChanged?.Invoke(this, new StatTargetChangedEventArgs<TimeBasedResourceStat>(this.Stat, oldStat));
            }
        }
        public TextMeshProUGUI ChangedValueTextMesh => _changedValueTextMesh;

        public bool IsShowChangedValueOnHover
        {
            get => _isShowChangedValueOnHover;
            set => _isShowChangedValueOnHover = value;
        }


        protected override void Start()
        {
            base.Start();
            if (ChangedValueTextMesh != null) ChangedValueTextMesh.gameObject.SetActive(false);
        }

        public virtual void SetStat(TimeBasedResourceStat stat)
        {
            this.Unregister();

            Stat = stat;

            this.Register();
        }

        protected override void Register()
        {
            base.Register();
            if (Stat != null) Stat.ChangeStat.OnValueChanged += Stat_OnChangeValueChanged;
        }

        protected override void Unregister()
        {
            if (Stat == null) return;
            base.Unregister();

            Stat.ChangeStat.OnValueChanged -= Stat_OnChangeValueChanged;
        }

        protected virtual void Stat_OnChangeValueChanged(object sender, ValueChangedEventArgs args)
        {
            if (Stat == null) return;
            if (ChangedValueTextMesh == null) return;

            ChangedValueTextMesh.text = Stat.ChangeStat.Value.ToString("+0.#;-0.#;0");
        }

        protected override void ResetStatBar()
        {
            base.ResetStatBar();
            ChangedValueTextMesh.text = string.Empty;
        }

        protected override void SyncStatbar()
        {
            base.SyncStatbar();
            ChangedValueTextMesh.text = Stat.ChangeStat.Value.ToString("+0.#;-0.#;0");
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            if (!IsShowChangedValueOnHover) return;
            if (ChangedValueTextMesh != null) ChangedValueTextMesh.gameObject.SetActive(true);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            if (!IsShowChangedValueOnHover) return;
            if (ChangedValueTextMesh != null) ChangedValueTextMesh.gameObject.SetActive(false);
        }

    }
}