using Asce.Game.Entities;
using Asce.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Asce.Game.UIs.Panels
{
    public class UIDeathPanel : UIPanel
    {
        [SerializeField] protected Button _reviveButton;
        [SerializeField] protected Button _menuButton;
        protected ICreature _creature;

        protected virtual void Start()
        {
            if (_reviveButton != null) _reviveButton.onClick.AddListener(ReviveButton_OnClick);
            if (_menuButton != null) _menuButton.onClick.AddListener(MenuButton_OnClick);
        }

        public void SetCreature(ICreature creature)
        {
            if (_creature == creature) return;
            this.Unregister();
            _creature = creature;
            this.Register();
        }

        protected virtual void Register()
        {
            if (_creature == null) return;
            _creature.Status.OnDeath += Creature_OnDeath;
            _creature.Status.OnRevive += Creature_OnRevive;
        }

        protected virtual void Unregister()
        {
            if (_creature == null) return;
            _creature.Status.OnDeath -= Creature_OnDeath;
            _creature.Status.OnRevive -= Creature_OnRevive;
        }


        protected virtual void ReviveButton_OnClick()
        {
            if (_creature == null) return;
            _creature.Status.SetStatus(EntityStatusType.Alive);

            var spawnPoint = SavePointManager.Instance.GetPointNearest(_creature.gameObject.transform.position);
            Vector2 spawnPosition = spawnPoint != null ? spawnPoint.Position : _creature.gameObject.transform.position;
            _creature.gameObject.transform.position = spawnPosition;
        }

        protected virtual void MenuButton_OnClick()
        {
            Game.SaveLoads.SaveLoadManager.Instance.SaveAll();
            SceneLoader.Instance.LoadScene("MainMenu", doneDelay: 0.5f);
        }

        private void Creature_OnDeath(object sender)
        {
            ICreature creature = (ICreature)sender;
            if (creature != _creature) return;

            this.Show();
        }

        private void Creature_OnRevive(object sender)
        {
            ICreature creature = (ICreature)sender;
            if (creature != _creature) return;

            this.Hide();
        }
    }
}
