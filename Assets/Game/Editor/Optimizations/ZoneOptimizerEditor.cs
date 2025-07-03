using Asce.Game;
using Asce.Managers.Utils;
using Asce.Game.Enviroments.Zones;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Asce.Editors
{

    [CustomEditor(typeof(ZoneOptimizer))]
    public class ZoneOptimizerEditor : Editor
    {
        private static FieldInfo _mainCameraField;
        private static FieldInfo _allZonesField;

        private ZoneOptimizer _optimizer;

        private void OnEnable()
        {
            _optimizer = (ZoneOptimizer)target;

            if (_mainCameraField == null)
                _mainCameraField = typeof(ZoneOptimizer).GetField("_mainCamera", BindingFlags.NonPublic | BindingFlags.Instance);

            if (_allZonesField == null)
                _allZonesField = typeof(ZoneOptimizer).GetField("_allZones", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private void OnSceneGUI()
        {
            if (_optimizer == null || _mainCameraField == null || _allZonesField == null)
                return;

            Camera cam = _mainCameraField.GetValue(_optimizer) as Camera;
            if (cam == null) return;

            Bounds cameraBounds = cam.GetBounds();
            cameraBounds.Expand(_optimizer.BoundsAdding);

            SceneEditorUtils.DrawBounds(cameraBounds, Color.white, Color.white.WithAlpha(0.05f));

            if (_allZonesField.GetValue(_optimizer) is not List<Zone> allZones) return;
            foreach (Zone zone in allZones)
            {
                if (zone == null || zone.SubZones == null) continue;

                foreach (SubZone subZone in zone.SubZones)
                {
                    if (subZone == null || subZone.ZoneCollider == null) continue;

                    Bounds subZoneBounds = subZone.ZoneCollider.bounds;
                    bool isVisible = cameraBounds.Intersects(subZoneBounds);

                    Color color = isVisible ? Color.green : Color.blue;
                    SceneEditorUtils.DrawBounds(subZoneBounds, color, color.WithAlpha(0.05f));
                }
            }
        }

    }
}
