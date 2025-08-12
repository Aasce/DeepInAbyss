using Asce.Game.Enviroments;
using Asce.Managers.Pools;
using UnityEngine;

namespace Asce.Game.UIs.Billboards
{
    public class UIBillboardWindow : UIWindow
    {
        [SerializeField] protected Pool<UIBillboardNotice> _noticePool = new();
        [SerializeField] protected Billboard _billboard;


        public override void Show()
        {
            if (_billboard == null) return;
            base.Show();
            foreach (UIBillboardNotice notice in _noticePool.Activities)
            {
                if (notice == null) continue;
                notice.Refresh();
            }
        }

        public void SetBillboard(Billboard billboard)
        {
            if (_billboard == billboard) return;

            this.Unregister();
            _billboard = billboard;
            this.Register();
        }

        protected virtual void Register()
        {
            if (_billboard == null) return;

            _noticePool.Clear(isDeactive: true);
            foreach (Notice notice in _billboard.Notices) 
            {
                if (notice == null) continue;

                UIBillboardNotice uiNotice = _noticePool.Activate();
                if (uiNotice == null) continue;

                uiNotice.SetNotice(notice);
                uiNotice.Show();
            }
        }

        protected virtual void Unregister()
        {
            if (_billboard == null) return;
        }
    }
}
