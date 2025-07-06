using Asce.Game.UIs.Stats;
using Asce.Managers.UIs;
using Asce.Managers.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Asce.Game.UIs.Characters
{
    public class UICreatureInformation : UIObject
    {
        [SerializeField, HideInInspector] protected UICreatureAvatar _avatar;
        [SerializeField, HideInInspector] protected UIResourceStatsInfoController _resourceStats;
        [SerializeField, HideInInspector] protected UIStatsInfoController _stats;

        [SerializeField] protected Button _toggleStatsInfoButton;


        public UICreatureAvatar Avatar => _avatar;
        public UIResourceStatsInfoController ResourceStats => _resourceStats;
        public UIStatsInfoController Stats => _stats;

        public Button ToggleStatsInfoButton => _toggleStatsInfoButton;


        protected override void RefReset()
        {
            base.RefReset();
            this.LoadComponent(out  _avatar);
            this.LoadComponent(out _resourceStats);
            this.LoadComponent(out _stats);
        }

        protected virtual void Awake()
        {
            
        }

        protected virtual void Start()
        {
            this.RegisterToggleStatsInfoButton();
        }

        protected virtual void RegisterToggleStatsInfoButton()
        {
            if (ToggleStatsInfoButton == null) return;
            if (Stats == null) return;

            ToggleStatsInfoButton.onClick.AddListener(() => Stats.Toggle());
        }
    }
}