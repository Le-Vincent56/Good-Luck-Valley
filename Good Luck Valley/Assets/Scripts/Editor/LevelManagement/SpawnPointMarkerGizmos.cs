using GoodLuckValley.World.LevelManagement.Adapters;
using UnityEditor;
using UnityEngine;

namespace GoodLuckValley.Editor.LevelManagement
{
    /// <summary>
    /// Draws scene-view gizmos for <see cref="SpawnPointMarker"/>: a colored
    /// sphere at the spawn position, a direction arrow indicating facing, and
    /// a label showing the spawn point ID.
    /// </summary>
    public static class SpawnPointMarkerGizmos
    {
        private static readonly Color _spawnColor = new Color(0.2f, 0.9f, 0.3f, 0.8f);
        private static readonly Color _arrowColor = new Color(0.2f, 0.7f, 1.0f, 0.9f);

        [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
        private static void DrawSpawnPointGizmo(SpawnPointMarker marker, GizmoType gizmoType)
        {
            Vector3 position = marker.transform.position;
            bool isSelected = (gizmoType & GizmoType.Selected) != 0;
            float sphereRadius = isSelected ? 0.35f : 0.25f;

            // Spawn position sphere
            Gizmos.color = _spawnColor;
            Gizmos.DrawWireSphere(position, sphereRadius);

            if (isSelected)
            {
                Color fillColor = _spawnColor;
                fillColor.a = 0.15f;
                Gizmos.color = fillColor;
                Gizmos.DrawSphere(position, sphereRadius);
            }

            // Facing direction arrow
            float direction = marker.FaceRight ? 1f : -1f;
            Vector3 arrowStart = position + Vector3.right * direction * 0.3f;
            Vector3 arrowEnd = position + Vector3.right * direction * 0.8f;
            Vector3 arrowHead1 = arrowEnd + new Vector3(-direction * 0.15f, 0.1f, 0f);
            Vector3 arrowHead2 = arrowEnd + new Vector3(-direction * 0.15f, -0.1f, 0f);

            Gizmos.color = _arrowColor;
            Gizmos.DrawLine(arrowStart, arrowEnd);
            Gizmos.DrawLine(arrowEnd, arrowHead1);
            Gizmos.DrawLine(arrowEnd, arrowHead2);

            // Spawn point ID label
            string label = string.IsNullOrEmpty(marker.SpawnPointID)
                ? "(no ID)"
                : marker.SpawnPointID;

            GUIStyle style = new GUIStyle();
            style.normal.textColor = isSelected ? Color.white : _spawnColor;
            style.fontSize = isSelected ? 12 : 10;
            style.fontStyle = isSelected ? FontStyle.Bold : FontStyle.Normal;
            style.alignment = TextAnchor.MiddleCenter;

            Handles.Label(position + Vector3.up * 0.5f, label, style);
        }
    }
}