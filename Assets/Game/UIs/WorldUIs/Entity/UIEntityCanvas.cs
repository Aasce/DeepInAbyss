using Asce.Game.Entities;
using Asce.Game.UIs.Stats;
using Asce.Managers.Attributes;
using Asce.Managers.UIs;
using Asce.Managers.Utils;
using TMPro;
using UnityEngine;

namespace Asce.Game.UIs
{
    public class UIEntityCanvas : UIObject, IWorldUI
    {
        [SerializeField, Readonly] protected Canvas _canvas;

        [Header("Elements")]
        [SerializeField] protected TextMeshProUGUI _nameText;

        protected string _baseName = string.Empty;


        public Canvas Canvas => _canvas;
        public TextMeshProUGUI NameText => _nameText;


        public string BaseName
        {
            get => _baseName;
            set => _baseName = value;
        }

        protected override void RefReset()
        {
            base.RefReset();
            this.LoadComponent(out _canvas);
        }

        public virtual void SetShowName(string name)
        {
            if (string.IsNullOrEmpty(name)) name = BaseName;
            if (string.IsNullOrEmpty(name)) name = SO_EntityInformation.noName;
            NameText.text = name;
        }

        public virtual void ResetBaseName()
        {
            NameText.text = string.IsNullOrEmpty(BaseName) ? SO_EntityInformation.noName : BaseName;
        }

        public virtual void SetVerticalPosition(float height)
        {
            transform.localPosition = Vector2.up * height;
        }
    }
}
