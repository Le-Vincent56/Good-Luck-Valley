/*******************************************************************************
The content of this file includes portions of the proprietary AUDIOKINETIC Wwise
Technology released in source code form as part of the game integration package.
The content of this file may not be used without valid licenses to the
AUDIOKINETIC Wwise Technology.
Note that the use of the game engine is subject to the Unity(R) Terms of
Service at https://unity3d.com/legal/terms-of-service

License Usage

Licensees holding valid licenses to the AUDIOKINETIC Wwise Technology may use
this file in accordance with the end user license agreement provided with the
software or, alternatively, in accordance with the terms contained
in a written agreement between you and Audiokinetic Inc.
Copyright (c) 2025 Audiokinetic Inc.
*******************************************************************************/

using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class AkWwiseBrowser : UnityEditor.EditorWindow
{
	[UnityEngine.SerializeField] UnityEditor.IMGUI.Controls.TreeViewState m_treeViewState;

	public static AkWwiseTreeView m_treeView;
	UnityEditor.IMGUI.Controls.SearchField m_SearchField;

	[UnityEditor.MenuItem("Window/Wwise Browser", false, (int)AkWwiseWindowOrder.WwisePicker)]
	public static void InitPickerWindow()
	{
		GetWindow<AkWwiseBrowser>("Wwise Browser", true,
		   typeof(UnityEditor.EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow"));
	}

	public void OnEnable()
	{
		if (m_treeViewState == null)
		{
			m_treeViewState = new UnityEditor.IMGUI.Controls.TreeViewState();
		}

		var multiColumnHeaderState = AkWwiseTreeView.CreateDefaultMultiColumnHeaderState();
		var multiColumnHeader = new UnityEditor.IMGUI.Controls.MultiColumnHeader(multiColumnHeaderState);
		m_treeView = new AkWwiseTreeView(m_treeViewState, multiColumnHeader, AkWwiseProjectInfo.GetTreeData());
		m_treeView.SetDoubleClickFunction(OnDoubleClick);

		m_treeView.dirtyDelegate = RequestRepaint;

		if (m_treeView.dataSource.Data.ItemDict.Count == 0)
		{
			Refresh();
			RequestRepaint();
		}
		
		m_SearchField = new UnityEditor.IMGUI.Controls.SearchField();
		m_SearchField.downOrUpArrowKeyPressed += m_treeView.SetFocusAndEnsureSelectedItem;
		m_SearchField.SetFocus();
	}

	public void OnDisable()
	{
		m_treeView.SaveExpansionStatus();
	}

	public static void Refresh(bool ignoreIfWaapi = false)
	{
		if (m_treeView != null)
		{
			m_treeView.dataSource.FetchData();
		};
	}

	private void OnDoubleClick(AkWwiseTreeViewItem item)
	{
		if (item == null)
		{
			return;
		}

		if (item.objectType == WwiseObjectType.Event)
		{
			PlayPauseItem(item);
			return;
		}

		if (item.hasChildren)
		{
			m_treeView.SetExpanded(item.id, !m_treeView.IsExpanded(item.id));
		}
	}

	private void PlayPauseItem(AkWwiseTreeViewItem item)
	{
		if (m_treeView != null && m_treeView.CheckWaapi())
		{
			AkWaapiUtilities.TogglePlayEvent(item.objectType, item.objectGuid);
		}
	}

	private bool isDirty;
	public void RequestRepaint()
	{
		isDirty = true;
	}

	void Update()
	{
		if (isDirty)
		{
			Repaint();
			m_treeView.Reload();
			isDirty = false;
		}

		if (AkWwiseEditorSettings.Instance.UseWaapi)
		{
			AkWwiseProjectInfo.WaapiPickerData.Update();
		}
	}

	public void OnGUI()
	{
		const int buttonWidth = 150;
		const int iconButtonWidth = 30;
		using (new UnityEngine.GUILayout.HorizontalScope("box"))
		{
			UnityEngine.GUILayout.Space(5);

			BrowserFilter OldFilter = m_treeView.Filters;
			m_treeView.Filters = (BrowserFilter)UnityEditor.EditorGUILayout.EnumFlagsField(m_treeView.Filters);
			if (OldFilter != m_treeView.Filters)
			{
				m_treeView.FiltersChanged = true;
			}
			

			var search_width = System.Math.Max(position.width / 3, buttonWidth * 2);
			
			m_treeView.StoredSearchString = m_SearchField.OnGUI(UnityEngine.GUILayoutUtility.GetRect(search_width, 30), m_treeView.StoredSearchString);
			UnityEngine.GUILayout.FlexibleSpace();

			var RefreshContent = new GUIContent(EditorGUIUtility.IconContent("Refresh"));
			RefreshContent.tooltip = "Refresh the Wwise Browser";
			if (UnityEngine.GUILayout.Button(RefreshContent, UnityEngine.GUILayout.Width(iconButtonWidth)))
			{
				Refresh();
			}


			if (UnityEngine.GUILayout.Button("Generate SoundBanks", UnityEngine.GUILayout.Width(buttonWidth), UnityEngine.GUILayout.Height(20)))
			{
				if (AkUtilities.IsSoundbankGenerationAvailable() && !AkUtilities.GeneratingSoundBanks)
				{
					AkUtilities.GenerateSoundbanks();
				}
				else if(!AkUtilities.GeneratingSoundBanks)
				{
					UnityEngine.Debug.LogError("Access to Wwise is required to generate the SoundBanks. Please go to Edit > Project Settings... and set the Wwise Application Path found in the Wwise Integration view.");
				}
			}
		}

		UnityEngine.GUILayout.Space(UnityEditor.EditorGUIUtility.standardVerticalSpacing);


		UnityEngine.GUILayout.FlexibleSpace();
		UnityEngine.Rect lastRect = UnityEngine.GUILayoutUtility.GetLastRect();
		m_treeView.OnGUI(new UnityEngine.Rect(lastRect.x, lastRect.y, position.width, lastRect.height));

		using (new UnityEngine.GUILayout.HorizontalScope("box"))
		{
			bool bConnected;
			string tooltip = AkWaapiUtilities.GetStatusString(out bConnected);
			
			var ConnectionContent = new GUIContent(EditorGUIUtility.IconContent(bConnected ? "Linked" : "Unlinked"));
			ConnectionContent.tooltip = tooltip;
			UnityEngine.GUILayout.Label(ConnectionContent, UnityEngine.GUILayout.Width(iconButtonWidth));
			UnityEngine.GUILayout.Label("Root Output Path: " + AkUtilities.GetRootOutputPath());

			var HelpContent = new GUIContent(EditorGUIUtility.IconContent("_Help"));
			HelpContent.tooltip = "Open the Wwise Unity Integration Documentation";
			if (UnityEngine.GUILayout.Button(HelpContent, UnityEngine.GUILayout.Width(iconButtonWidth)))
			{
				Application.OpenURL("https://www.audiokinetic.com/library/edge/?source=Unity&id=index.html");
			}

			var ProjectSettingsContent = new GUIContent(EditorGUIUtility.IconContent("_Popup"));
			ProjectSettingsContent.tooltip = "Open the Wwise Unity Project Settings";
			if (UnityEngine.GUILayout.Button(ProjectSettingsContent, UnityEngine.GUILayout.Width(iconButtonWidth)))
			{
				OpenProjectSettings();
			}
		}
	}

	void OpenProjectSettings()
	{
		SettingsService.OpenProjectSettings("Project/Wwise Integration");
	}

	static void SelectInWwisePicker(System.Guid guid)
	{
		InitPickerWindow();
		m_treeView.SelectItem(guid);
	}

	[UnityEditor.MenuItem("CONTEXT/AkBank/Select in Wwise Browser")]
	[UnityEditor.MenuItem("CONTEXT/AkAmbient/Select in Wwise Browser")]
	[UnityEditor.MenuItem("CONTEXT/AkEvent/Select in Wwise Browser")]
	[UnityEditor.MenuItem("CONTEXT/AkState/Select in Wwise Browser")]
	[UnityEditor.MenuItem("CONTEXT/AkSwitch/Select in Wwise Browser")]
	[UnityEditor.MenuItem("CONTEXT/AkWwiseTrigger/Select in Wwise Browser")]
	static void SelectItemInWwisePicker(UnityEditor.MenuCommand command)
	{
		AkTriggerHandler component = (AkTriggerHandler)command.context;
		try
		{
			var data = component.GetType().GetField("data");
			var guid = (data.GetValue(component) as AK.Wwise.BaseType).ObjectReference.Guid;
			SelectInWwisePicker(guid);
		}
		catch { }
	}

	[UnityEditor.MenuItem("CONTEXT/AkRoom/Select Aux Bus in Wwise Picker")]
	static void SelectAkRoomAuxBusInWwisePicker(UnityEditor.MenuCommand command)
	{
		AkRoom component = (AkRoom)command.context;
		SelectInWwisePicker(component.reverbAuxBus.ObjectReference.Guid);
	}

	[UnityEditor.MenuItem("CONTEXT/AkRoom/Select Event in Wwise Picker")]
	static void SelectAkRoomEventInWwisePicker(UnityEditor.MenuCommand command)
	{
		AkRoom component = (AkRoom)command.context;
		SelectInWwisePicker(component.roomToneEvent.ObjectReference.Guid);
	}

	[UnityEditor.MenuItem("CONTEXT/AkSurfaceReflector/Select in Wwise Browser")]
	static void SelectReflectorTextureItemInWwisePicker(UnityEditor.MenuCommand command)
	{
		AkSurfaceReflector component = (AkSurfaceReflector)command.context;
		if (component.AcousticTextures.Length >0 && component.AcousticTextures[0].ObjectReference !=null)
		{
			SelectInWwisePicker(component.AcousticTextures[0].ObjectReference.Guid);
		}
	}

	[UnityEditor.MenuItem("CONTEXT/AkEnvironment/Select in Wwise Browser")]
	static void SelectEnvironmentItemInWwisePicker(UnityEditor.MenuCommand command)
	{
		AkEnvironment component = (AkEnvironment)command.context;
		SelectInWwisePicker(component.data.ObjectReference.Guid);
	}

	[UnityEditor.MenuItem("CONTEXT/AkEarlyReflections/Select in Wwise Browser")]
	static void SelectReflectionsItemInWwisePicker(UnityEditor.MenuCommand command)
	{
		AkEarlyReflections component = (AkEarlyReflections)command.context;
		SelectInWwisePicker(component.reflectionsAuxBus.ObjectReference.Guid);
	}
}
#endif