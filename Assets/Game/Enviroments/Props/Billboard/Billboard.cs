using Asce.Managers.Attributes;
using Asce.Managers.Pools;
using Asce.Managers.SaveLoads;
using Asce.Managers.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Asce.Game.Enviroments
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Billboard : InteractiveObject, IUniqueIdentifiable, IEnviromentComponent, IInteractableObject
    {
        [SerializeField, Readonly] protected string _id;

        // Ref
        [SerializeField, Readonly] protected BoxCollider2D _collider;
        [SerializeField] protected Pool<BillboardNotice> _pool = new();
        [SerializeField] protected List<Notice> _notices = new();

        [Header("Configs")]
        [SerializeField, Range(0, 20)] protected int _maxAttempts = 20;

        [Tooltip("Minimum distance between notifications")]
        [SerializeField, Min(0f)] protected float _minNotifiesDistance = 0.5f;

        protected ReadOnlyCollection<BillboardNotice> _readonlyNotifyObjects;

        public string ID => _id;
        public BoxCollider2D Collider => _collider;
        public List<Notice> Notices => _notices;

        protected override void RefReset()
        {
            base.RefReset();
            this.LoadComponent(out  _collider);
        }

        public override void Interact(GameObject interactor)
        {

        }

        public override void Focus()
        {
            base.Focus();

        }

        public override void Unfocus()
        {
            base.Unfocus();

        }

        public virtual void AddNotice(Notice notice)
        {
            if (notice == null) return;
            _notices.Add(notice);

            BillboardNotice notifyObject = _pool.Activate();
            if (notifyObject == null) return;

            notifyObject.SetRandomSprite();
            this.SetNotifyPosition(notifyObject);
        }

        public virtual void RemoveNotice(Notice notice)
        {
            if (notice == null) return;
            if (_notices.Remove(notice))
            {
                _pool.DeactivateAt(0);
            }
        }

        public virtual void ClearNotices()
        {
            _notices.Clear();
            _pool.Clear(isDeactive: true);
        }

        protected virtual void SetNotifyPosition(BillboardNotice notifyObject)
        {
            if (notifyObject == null || _collider == null) return;

            Bounds bounds = _collider.bounds;
            bounds.Expand(-1f);

            for (int attempt = 0; attempt < _maxAttempts; attempt++)
            {
                float x = Random.Range(bounds.min.x, bounds.max.x);
                float y = Random.Range(bounds.min.y, bounds.max.y);
                Vector3 candidatePos = new(x, y, transform.position.z);

                // Check distance with active notify
                bool tooClose = false;
                foreach (BillboardNotice existing in _pool.Activities)
                {
                    if (existing == null) continue;
                    if (Vector3.Distance(existing.transform.position, candidatePos) < _minNotifiesDistance)
                    {
                        tooClose = true;
                        break;
                    }
                }

                if (!tooClose)
                {
                    notifyObject.transform.position = candidatePos;
                    return;
                }
            }

            // If you try several times and still get duplicates, put it in the middle.
            notifyObject.transform.position = bounds.center;
        }

    }
}
