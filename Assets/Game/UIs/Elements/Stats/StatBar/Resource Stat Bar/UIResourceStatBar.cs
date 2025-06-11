using Asce.Game.Stats;
using Asce.Managers;
using Asce.Managers.UIs;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Asce.Game.UIs.Stats
{
    public class UIResourceStatBar : UIObject, IUIStatBar<ResourceStat>, IUIStatBarHasText, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] protected Slider _slider;
        [SerializeField] protected TextMeshProUGUI _textMesh;
        protected ResourceStat _stat;

        private Image _fillImage;

        public event Action<object, StatTargetChangedEventArgs<ResourceStat>> OnStatTargetChanged;


        public Slider Slider => _slider;
        public TextMeshProUGUI TextMesh => _textMesh;
        public Image FillImage => (_fillImage != null) ? _fillImage : _fillImage = Slider.fillRect.GetComponent<Image>();

        public ResourceStat Stat
        {
            get => _stat;
            protected set 
            {
                ResourceStat oldStat = _stat;
                _stat = value;
                OnStatTargetChanged?.Invoke(this, new StatTargetChangedEventArgs<ResourceStat>(_stat, oldStat));
            }
        }

        public virtual bool IsUseText
        {
            get => TextMesh != null && TextMesh.gameObject.activeSelf;
            set { if (TextMesh != null) TextMesh.gameObject.SetActive(value); }
        }
        public virtual float TotalResource => Stat == null ? 0f : Stat.CurrentValue;


        protected virtual void Reset()
        {

        }

        protected virtual void Start()
        {

        }

        public virtual void SetStat(ResourceStat stat)
        {
            this.Unregister();

            Stat = stat;
            Debug.Log($"[UIResourceStatBar] Stat is {(Stat == null ? "" : "not")} null", this);

            this.Register();
        }

        protected virtual void Register()
        {
            if (Stat == null)
            {
                this.ResetStatBar();
                return;
            }

            this.SyncStatbar();

            Stat.OnValueChanged += Stat_OnValueChanged;
            Stat.OnCurrentValueChanged += Stat_OnCurrentValueChanged;
        }

        protected virtual void Unregister()
        {
            if (Stat == null) return;
            Stat.OnValueChanged -= Stat_OnValueChanged;
            Stat.OnCurrentValueChanged -= Stat_OnCurrentValueChanged;
        }

        protected virtual void Stat_OnValueChanged(object sender, ValueChangedEventArgs args) 
        {
            SetMaxValue(Mathf.Max(TotalResource, Stat.Value));
            this.TriggerText();
        }

        protected virtual void Stat_OnCurrentValueChanged(object sender, ValueChangedEventArgs args)
        {
            this.SetMaxValue(Mathf.Max(TotalResource, Stat.Value));
            Slider.value = Stat.CurrentValue;
            this.TriggerText();
        }

        protected virtual void SetMaxValue(float value)
        {
            Slider.maxValue = value;
        }

        public virtual void TriggerText()
        {
            if (!IsUseText) return;
            if (Stat == null) return;

            TextMesh.text = $"{Mathf.Round(TotalResource)}/{Mathf.Round(Stat.Value)}";
        }

        protected virtual void ResetStatBar()
        {
            SetMaxValue(0f);
            Slider.value = 0f;
            this.TriggerText();
        }

        protected virtual void SyncStatbar()
        {
            this.SetMaxValue(Mathf.Max(TotalResource, Stat.Value));
            Slider.value = Stat.CurrentValue;
            this.TriggerText();
        }


        public virtual void OnPointerEnter(PointerEventData eventData)
        {

        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {

        }
    }
}