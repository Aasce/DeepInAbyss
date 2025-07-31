using Asce.Game.Entities;
using Asce.Game.Spawners;
using Asce.Managers;
using Asce.Managers.Utils;
using System.Collections;
using UnityEngine;

namespace Asce.Game.Enviroments.Zones
{
    public class CommingSoonZone : GameComponent
    {
        [SerializeField] protected LayerMask _entityLayer;
        [SerializeField] protected float _teleportDelay = 5f;

        protected void OnTriggerEnter2D(Collider2D collision)
        {
            if (!LayerUtils.IsInLayerMask(collision.gameObject.layer, _entityLayer)) return;
            if (!collision.TryGetComponent(out ICreature creature)) return;

            if (!creature.IsControlByPlayer()) return;

            StartCoroutine(this.TeleportCreatureToSpawnPoint(creature));
        }

        protected IEnumerator TeleportCreatureToSpawnPoint(ICreature creature)
        {
            yield return new WaitForSeconds(_teleportDelay);

            var spawnPoint = SavePointManager.Instance.GetPointNearest(creature.gameObject.transform.position);
            Vector2 spawnPosition = spawnPoint != null ? spawnPoint.Position : creature.gameObject.transform.position;
            creature.gameObject.transform.position = spawnPosition;
        }
    }
}
