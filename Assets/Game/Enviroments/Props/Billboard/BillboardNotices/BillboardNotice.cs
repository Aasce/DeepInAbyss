using Asce.Managers;
using Asce.Managers.Attributes;
using Asce.Managers.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.Enviroments
{
    public class BillboardNotice : GameComponent, IEnviromentComponent
    {
        [SerializeField, Readonly] protected SpriteRenderer _renderer;
        [SerializeField] protected List<Sprite> _sprite = new();

        protected override void RefReset()
        {
            base.RefReset();
            this.LoadComponent(out _renderer);
        }

        public virtual void SetSprite(int index)
        {
            if (index < 0 || index >= _sprite.Count) return;
            if (_renderer == null) return;

            Sprite sprite = _sprite[index];
            if (sprite == null) return;

            _renderer.sprite = sprite;
        }

        public virtual void SetRandomSprite()
        {
            if (_sprite.Count <= 0) return;
            int index = Random.Range(0, _sprite.Count);
            this.SetSprite(index);
        }
    }
}