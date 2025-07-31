using Asce.Managers;
using Asce.Managers.Attributes;
using Asce.Managers.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Asce.Game.Spawners
{
    public class SavePointManager : MonoBehaviourSingleton<SavePointManager>
    {
        [SerializeField, Readonly] protected List<ISavePoint> _savePoints = new();
        protected ReadOnlyCollection<ISavePoint> _readonlySavePoints;


        public ReadOnlyCollection<ISavePoint> SavePoints => _readonlySavePoints ??= _savePoints.AsReadOnly();

        protected override void Awake()
        {
            base.Awake();
            _savePoints.Clear();
            _savePoints.AddRange(ComponentUtils.FindAllComponentsInScene<ISavePoint>());
        }

        public ISavePoint GetPointNearest(Vector2 position)
        {
            float minDistance = float.MaxValue;
            ISavePoint nearestPoint = null;

            foreach (ISavePoint savePoint in _savePoints)
            {
                if (!savePoint.IsActive) continue;
                float distance = Vector2.Distance(savePoint.Position, position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestPoint = savePoint;
                }
            }

            return nearestPoint;
        }
    }
}
