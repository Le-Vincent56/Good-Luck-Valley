using UnityEditor;
using GoodLuckValley.UI.Editor;
using GoodLuckValley.Journal.UI;

namespace GoodLuckValley.Journal.Editor
{
    [CustomEditor(typeof(JournalUI))]
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