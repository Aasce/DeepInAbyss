using Asce.Managers;
using Asce.Managers.Attributes;
using Asce.Managers.SaveLoads;
using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Enviroments.HiddenAreas
{
    public class HiddenArea : GameComponent, IUniqueIdentifiable, IReceiveData<bool>
    {
        // Ref
        [SerializeField, Readonly] protected HiddenAreaOpener _opener;
        [SerializeField] protected GameObject _coveredObject;
        [SerializeField] protected VFXs.VFXObject _vfxObject;

        [Space]
        [SerializeField, Readonly] protected string _id;

        [Space]
        [SerializeField] protected bool _isOpened = false;

        public HiddenAreaOpener Opener => _opener;
        public GameObject CoveredObject => _coveredObject;

        public string ID => _id;
        public bool IsOpened => _isOpened;

        protected override void RefReset()
        {
            base.RefReset();
            if (this.LoadComponent(out _opener)) _opener.Controller = this;
        }

        protected void Start()
        {
            if (_opener != null) _opener.OnInteract += Opener_OnInteract;
        }

        protected void Opener_OnInteract(object sender, GameObject args)
        {
            if (_isOpened) return;
            if (_coveredObject == null) return;
            _coveredObject.SetActive(false);
            if (_vfxObject != null) VFXs.VFXsManager.Instance.Spawn(_vfxObject, transform.position);
            _isOpened = true;
        }

        void IReceiveData<bool>.Receive(bool isOpened)
        {
            _isOpened = isOpened;
            if (_coveredObject == null) return;
            _coveredObject.SetActive(!isOpened);
        }
    }
}
