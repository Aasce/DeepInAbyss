using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Asce.Managers.UIs
{
    public class UIAlert : UIObject, IPointerClickHandler
    {
        [SerializeField] protected TextButton _buttonA;
        [SerializeField] protected TextButton _buttonB;

        [Space]
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _descriptionText;

        protected Action _onButtonAClick;
        protected Action _onButtonBClick;

        private void Start()
        {
            if (_buttonA != null) _buttonA.Button.onClick.AddListener(ButtonA_OnClick);
            if (_buttonB != null) _buttonB.Button.onClick.AddListener(ButtonB_OnClick);
        }


        public void Set(
            string title = null, string description = "Are you ok?", 
            string buttonATitle = null, string buttonBTitle = null,
            Action onButtonAClick = null, Action onButtonBClick = null)
        {
            if (_titleText != null) _titleText.text = title ?? "Alert";
            if (_descriptionText != null) _descriptionText.text = description ?? "Are you ok?";

            if (_buttonA != null)
            {
                if (buttonATitle == null) _buttonA.gameObject.SetActive(false);
                else
                {
                    _buttonA.gameObject.SetActive(true);
                    _buttonA.Text.text = buttonATitle;
                    _onButtonAClick = onButtonAClick;
                }
            }

            if (_buttonB != null)
            {
                if (buttonBTitle == null) _buttonB.gameObject.SetActive(false);
                else
                {
                    _buttonB.gameObject.SetActive(true);
                    _buttonB.Text.text = buttonBTitle;
                    _onButtonBClick = onButtonBClick;
                }
            }
        }

        protected void ButtonA_OnClick()
        {
            _onButtonAClick?.Invoke();
            Destroy(gameObject);
        }

        protected void ButtonB_OnClick()
        {
            _onButtonBClick?.Invoke();
            Destroy(gameObject);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_buttonA == null || _buttonA.gameObject.activeSelf) return;
            if (_buttonB == null || _buttonB.gameObject.activeSelf) return;

            Destroy(gameObject);
        }
    }
}
