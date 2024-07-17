using UnityEditor;
using GoodLuckValley.UI.Editor;

namespace GoodLuckValley.UI.Notifications.Editor
{
    [CustomEditor(typeof(NotificationPanel))]
    public class NotificationPanelEditor : FadePanelEditorBase
    {
        public override void OnInspectorGUI()
        {
            ShowFadePanelUI();
            EditorGUILayout.Space();
            base.OnInspectorGUI();
        }
    }
}