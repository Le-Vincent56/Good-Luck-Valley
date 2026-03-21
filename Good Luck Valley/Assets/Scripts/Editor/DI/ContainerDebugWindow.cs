using System;
using System.Collections.Generic;
using GoodLuckValley.Core.DI.Core;
using GoodLuckValley.Core.DI.Interfaces;
using GoodLuckValley.Core.DI.Lifecycle;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace GoodLuckValley.Editor.DI
{
    /// <summary>
    /// Editor window that displays active DI containers, their registrations,
    /// imports, and scope hierarchy during play mode. Accessible
    /// via Window > Good Luck Valley > DI Debug.
    /// </summary>
    public class ContainerDebugWindow : EditorWindow
    {
        private const float TreePanelWidth = 250f;

        private VisualElement _notPlayingContainer;
        private TwoPaneSplitView _splitView;
        private ScrollView _treeScroll;
        private ScrollView _detailsScroll;
        private Label _containerCountLabel;
        private Container _selectedContainer;

        [MenuItem("Good Luck Valley/DI Debug Window")]
        public static void ShowWindow()
        {
            ContainerDebugWindow window = GetWindow<ContainerDebugWindow>("DI Debug Window");
            window.minSize = new Vector2(650, 300);
        }

        /// <summary>
        /// Initializes the user interface for the container debug window.
        /// Sets up the layout, toolbar, not-playing message, and split view for displaying container data and details.
        /// Handles play mode state changes by registering appropriate events and providing a periodic auto-refresh mechanism.
        /// </summary>
        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            root.style.flexGrow = 1;

            // Toolbar
            Toolbar toolbar = new Toolbar();

            ToolbarButton refreshButton = new ToolbarButton(Refresh) { text = "Refresh" };
            toolbar.Add(refreshButton);

            VisualElement toolbarSpacer = new VisualElement();
            toolbarSpacer.style.flexGrow = 1;
            toolbar.Add(toolbarSpacer);

            _containerCountLabel = new Label("0 containers");
            _containerCountLabel.style.unityTextAlign = TextAnchor.MiddleRight;
            _containerCountLabel.style.paddingRight = 8;
            toolbar.Add(_containerCountLabel);

            root.Add(toolbar);

            // Not Playing Message
            _notPlayingContainer = new VisualElement();
            _notPlayingContainer.style.flexGrow = 1;
            _notPlayingContainer.style.alignItems = Align.Center;
            _notPlayingContainer.style.justifyContent = Justify.Center;

            Label notPlayingLabel = new Label("Enter Play Mode to view active DI containers.");
            notPlayingLabel.style.color = new Color(0.6f, 0.6f, 0.6f);
            notPlayingLabel.style.fontSize = 13;
            _notPlayingContainer.Add(notPlayingLabel);

            root.Add(_notPlayingContainer);

            // Split View
            _splitView = new TwoPaneSplitView(0, TreePanelWidth, TwoPaneSplitViewOrientation.Horizontal);
            _splitView.style.flexGrow = 1;

            // Left pane: container tree
            VisualElement treePane = new VisualElement();
            treePane.style.minWidth = 150;

            Label treeHeader = new Label("Containers");
            treeHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
            treeHeader.style.paddingLeft = 8;
            treeHeader.style.paddingTop = 6;
            treeHeader.style.paddingBottom = 6;
            treePane.Add(treeHeader);

            _treeScroll = new ScrollView(ScrollViewMode.Vertical);
            _treeScroll.style.flexGrow = 1;
            treePane.Add(_treeScroll);

            _splitView.Add(treePane);

            // Right pane: details
            VisualElement detailsPane = new VisualElement();
            detailsPane.style.flexGrow = 1;

            _detailsScroll = new ScrollView(ScrollViewMode.Vertical);
            _detailsScroll.style.flexGrow = 1;
            _detailsScroll.style.paddingLeft = 12;
            _detailsScroll.style.paddingRight = 12;
            _detailsScroll.style.paddingTop = 8;
            detailsPane.Add(_detailsScroll);

            _splitView.Add(detailsPane);

            root.Add(_splitView);

            // Events
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            // Auto-refresh every second during play mode
            root.schedule.Execute(Refresh).Every(1000);

            Refresh();
        }

        private void OnDestroy()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        /// <summary>
        /// Handles changes in the play mode state of the Unity Editor.
        /// Resets the selected container and updates the debug window to reflect the current play mode state.
        /// </summary>
        /// <param name="state">The new play mode state of the Unity Editor.</param>
        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            _selectedContainer = null;
            Refresh();
        }

        /// <summary>
        /// Updates the container debug window based on the current application state.
        /// Adjusts the visibility of UI elements depending on whether the application is playing
        /// and refreshes the container count, tree view, and details display accordingly.
        /// </summary>
        private void Refresh()
        {
            bool isPlaying = Application.isPlaying;

            _notPlayingContainer.style.display = isPlaying
                ? DisplayStyle.None
                : DisplayStyle.Flex;

            _splitView.style.display = isPlaying
                ? DisplayStyle.Flex
                : DisplayStyle.None;

            if (!isPlaying)
            {
                _containerCountLabel.text = "0 containers";
                return;
            }

            int count = CountContainers();
            _containerCountLabel.text = count + (count == 1 ? " container" : "containers");

            RebuildTree();
            RebuildDetails();
        }

        /// <summary>
        /// Reconstructs the visual representation of the container hierarchy in the debug window.
        /// Clears the existing tree view and populates it with nodes representing the application
        /// container and all root scene containers that have no parent container.
        /// </summary>
        private void RebuildTree()
        {
            _treeScroll.Clear();

            // Application container
            IContainer appContainer = ContainerRegistry.ApplicationContainer;
            if (appContainer is Container appImpl)
            {
                _treeScroll.Add(CreateTreeNode(appImpl, 0));
            }

            // Root scene containers (no parent)
            IReadOnlyDictionary<Scene, IContainer> sceneContainers = ContainerRegistry.AllSceneContainers;

            foreach (KeyValuePair<Scene, IContainer> kvp in sceneContainers)
            {
                if (kvp.Value is not Container { Parent: null } sceneImpl) continue;

                _treeScroll.Add(CreateTreeNode(sceneImpl, 0));
            }
        }

        /// <summary>
        /// Creates a hierarchical tree node visual element for representing a container
        /// and its child containers in the debug window. Configures the node's layout,
        /// interactive elements such as toggle for expanding/collapsing, and a button
        /// for selecting the container.
        /// </summary>
        /// <param name="container">The container to represent as a tree node in the visual hierarchy.</param>
        /// <param name="depth">The depth level of the given container in the tree hierarchy,
        /// used to determine the padding and indentation of the node's UI representation.</param>
        /// <returns>A <see cref="VisualElement"/> representing the container and its child nodes in the tree structure.</returns>
        private VisualElement CreateTreeNode(Container container, int depth)
        {
            VisualElement node = new VisualElement();
            bool hasChildren = container.Children != null && container.Children.Count > 0;

            // Row
            VisualElement row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.alignItems = Align.Center;
            row.style.paddingLeft = depth * 16;
            row.style.height = 22;

            // Children container (declared before toggle so lambda can capture it)
            VisualElement childrenContainer = null;

            if (hasChildren)
            {
                childrenContainer = new VisualElement();
                bool expanded = true;

                Label toggleLabel = new Label("\u25BC");
                toggleLabel.style.width = 18;
                toggleLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
                toggleLabel.style.fontSize = 10;

                VisualElement capturedChildren = childrenContainer;
                toggleLabel.RegisterCallback<ClickEvent>(evt =>
                {
                    expanded = !expanded;
                    capturedChildren.style.display = expanded
                        ? DisplayStyle.Flex
                        : DisplayStyle.None;
                    toggleLabel.text = expanded ? "\u25BC" : "\u25B6";
                });

                row.Add(toggleLabel);
            }
            else
            {
                VisualElement spacer = new VisualElement();
                spacer.style.width = 18;
                row.Add(spacer);
            }

            // Name button
            bool isSelected = _selectedContainer == container;

            Button nameButton = new Button(() => SelectContainer(container));
            nameButton.text = FormatContainerLabel(container);
            ApplyTreeButtonStyle(nameButton, isSelected);
            row.Add(nameButton);

            node.Add(row);

            // Add children
            if (!hasChildren) return node;
            
            foreach (Container child in container.Children)
            {
                childrenContainer.Add(CreateTreeNode(child, depth + 1));
            }

            node.Add(childrenContainer);

            return node;
        }

        /// <summary>
        /// Sets the currently selected Dependency Injection (DI) container
        /// in the debug window, allowing detailed information about the container's
        /// registrations, imports, and hierarchy to be displayed.
        /// Clears the previous selection, updates the visual representation of the
        /// containers in the tree, and refreshes the details section to reflect
        /// the newly selected container.
        /// </summary>
        /// <param name="container">The Container instance to select and display
        /// details for in the debug window.</param>
        private void SelectContainer(Container container)
        {
            _selectedContainer = container;
            RebuildTree();
            RebuildDetails();
        }

        /// <summary>
        /// Updates the UI details panel within the debug window to reflect
        /// the details of the currently selected Dependency Injection (DI)
        /// container. Clears existing content in the details panel and
        /// populates it with the selected container's header, registration
        /// information, and imports.
        /// If no container is selected, a placeholder message is displayed in
        /// the details panel prompting the user to select a container.
        /// </summary>
        private void RebuildDetails()
        {
            _detailsScroll.Clear();

            if (_selectedContainer == null)
            {
                Label placeholder = new Label("Select a container to view details.");
                placeholder.style.color = new Color(0.6f, 0.6f, 0.6f);
                placeholder.style.paddingTop = 20;
                _detailsScroll.Add(placeholder);
                return;
            }

            _detailsScroll.Add(CreateContainerHeader());
            _detailsScroll.Add(CreateSpacer(12));
            _detailsScroll.Add(CreateRegistrationTable());
            _detailsScroll.Add(CreateSpacer(12));
            _detailsScroll.Add(CreateImportList());
        }

        /// <summary>
        /// Creates a visual element representing the header for the selected Dependency Injection (DI) container.
        /// The header includes the container's name, type (e.g., Root or Scoped), its parent container's name if applicable,
        /// and the number of child scopes associated with it. Displays hierarchical information about the container.
        /// </summary>
        /// <returns>
        /// A VisualElement containing detailed information about the selected DI container.
        /// </returns>
        private VisualElement CreateContainerHeader()
        {
            VisualElement header = new VisualElement();

            Label nameLabel = new Label("Container: " + _selectedContainer.Name);
            nameLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            nameLabel.style.fontSize = 14;
            nameLabel.style.paddingBottom = 4;
            header.Add(nameLabel);

            string containerType = _selectedContainer.Parent != null ? "Scoped" : "Root";
            header.Add(new Label("Type: " + containerType));

            if (_selectedContainer.Parent != null)
            {
                header.Add(new Label("Parent: " + _selectedContainer.Parent.Name));
            }

            int childCount = _selectedContainer.Children?.Count ?? 0;

            if (childCount > 0)
            {
                header.Add(new Label("Child Scopes: " + childCount));
            }

            return header;
        }

        /// <summary>
        /// Constructs a table representation of all registrations within the currently
        /// selected Dependency Injection (DI) container. Each registration is displayed
        /// with its corresponding interface, implementation, lifetime, and resolved state.
        /// If no registrations are available, a placeholder message is added indicating
        /// the absence of entries.
        /// </summary>
        /// <returns>
        /// A VisualElement representing a table containing the registration details
        /// of the selected DI container, or a placeholder message if no registrations
        /// exist.
        /// </returns>
        private VisualElement CreateRegistrationTable()
        {
            VisualElement table = new VisualElement();

            IReadOnlyDictionary<Type, Registration> registrations =
                _selectedContainer.Registrations;

            if (registrations == null || registrations.Count == 0)
            {
                Label empty = new Label("No registrations.");
                empty.style.color = new Color(0.6f, 0.6f, 0.6f);
                table.Add(empty);
                return table;
            }

            Label sectionHeader = new Label("Registrations (" + registrations.Count + ")");
            sectionHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
            sectionHeader.style.paddingBottom = 4;
            table.Add(sectionHeader);

            // Header row
            table.Add(CreateTableRow("Interface", "Implementation", "Lifetime", "Resolved", true));

            // Data rows
            foreach (KeyValuePair<Type, Registration> kvp in registrations)
            {
                Registration registration = kvp.Value;

                string interfaceName = FormatTypeName(registration.InterfaceType);
                string implName = FormatTypeName(registration.ImplementationType);

                string lifetime = registration.IsInstance
                    ? "Instance"
                    : registration.Lifetime.ToString();

                string resolved;
                if (registration.Lifetime == Lifetime.Transient && !registration.IsInstance)
                {
                    resolved = "N/A";
                }
                else
                {
                    resolved = registration.Instance != null ? "Yes" : "No";
                }

                table.Add(CreateTableRow(interfaceName, implName, lifetime, resolved, false));
            }

            return table;
        }

        /// <summary>
        /// Creates a table row element with the specified column values and styles.
        /// Each row is styled as either a header or a data row based on the <c>isHeader</c> parameter.
        /// The row contains labels for four columns, each with a width allocated according to its content.
        /// </summary>
        /// <param name="col1">The text to display in the first column.</param>
        /// <param name="col2">The text to display in the second column.</param>
        /// <param name="col3">The text to display in the third column.</param>
        /// <param name="col4">The text to display in the fourth column.</param>
        /// <param name="isHeader">A boolean value indicating whether the row is styled as a header.
        /// If true, the row uses bold styling and includes a bottom border; otherwise, it uses normal styling.</param>
        /// <returns>A <c>VisualElement</c> representing the constructed table row, containing the specified column values.</returns>
        private VisualElement CreateTableRow(
            string col1,
            string col2,
            string col3,
            string col4,
            bool isHeader
        )
        {
            VisualElement row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.paddingTop = 2;
            row.style.paddingBottom = 2;

            if (isHeader)
            {
                row.style.borderBottomWidth = 1;
                row.style.borderBottomColor = new Color(0.3f, 0.3f, 0.3f);
                row.style.paddingBottom = 4;
                row.style.marginBottom = 2;
            }

            FontStyle fontStyle = isHeader ? FontStyle.Bold : FontStyle.Normal;

            Label label1 = new Label(col1);
            label1.style.width = 180;
            label1.style.unityFontStyleAndWeight = fontStyle;
            row.Add(label1);

            Label label2 = new Label(col2);
            label2.style.width = 180;
            label2.style.unityFontStyleAndWeight = fontStyle;
            row.Add(label2);

            Label label3 = new Label(col3);
            label3.style.width = 80;
            label3.style.unityFontStyleAndWeight = fontStyle;
            row.Add(label3);

            Label label4 = new Label(col4);
            label4.style.width = 70;
            label4.style.unityFontStyleAndWeight = fontStyle;
            row.Add(label4);

            return row;
        }

        /// <summary>
        /// Creates a list of imported types for the selected Dependency Injection (DI)
        /// container. Displays the imports count and a list of each type that was
        /// imported, along with the originating container's name. The section is
        /// empty if there are no imports for the selected container.
        /// </summary>
        /// <returns>
        /// A <see cref="VisualElement"/> containing the imports list, including a
        /// section header and a list of imported types.
        /// </returns>
        private VisualElement CreateImportList()
        {
            VisualElement importSection = new VisualElement();

            IReadOnlyCollection<Type> imports = _selectedContainer.Imports;

            if (imports == null || imports.Count == 0) return importSection;

            Label sectionHeader = new Label("Imports (" + imports.Count + ")");
            sectionHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
            sectionHeader.style.paddingBottom = 4;
            importSection.Add(sectionHeader);

            string parentName = _selectedContainer.Parent != null
                ? _selectedContainer.Parent.Name
                : "unknown";

            foreach (Type importedType in imports)
            {
                Label importLabel = new Label(
                    FormatTypeName(importedType) + "  (from " + parentName + ")");
                importLabel.style.paddingLeft = 8;
                importLabel.style.paddingTop = 1;
                importLabel.style.paddingBottom = 1;
                importSection.Add(importLabel);
            }

            return importSection;
        }

        /// <summary>
        /// Applies styling to a tree node's button to visually indicate its state.
        /// Updates the button's appearance based on whether it is currently selected or not.
        /// </summary>
        /// <param name="button">The button to which the styling will be applied.</param>
        /// <param name="isSelected">A boolean value indicating whether the button corresponds to the selected item.</param>
        private void ApplyTreeButtonStyle(Button button, bool isSelected)
        {
            button.style.backgroundColor = isSelected
                ? new Color(0.24f, 0.38f, 0.57f)
                : Color.clear;
            
            button.style.borderTopWidth = 0;
            button.style.borderBottomWidth = 0;
            button.style.borderLeftWidth = 0;
            button.style.borderRightWidth = 0;
            button.style.unityTextAlign = TextAnchor.MiddleLeft;
            button.style.paddingLeft = 4;
            button.style.flexGrow = 1;

            if (!isSelected) return;
            
            button.style.color = Color.white;
        }

        /// <summary>
        /// Formats the label for a given DI container by appending the count of its
        /// registrations within parentheses after the container's name.
        /// </summary>
        /// <param name="container">The container whose label should be formatted, including its registration count.</param>
        /// <returns>A string representing the formatted label of the container, including its name and registration count.</returns>
        private string FormatContainerLabel(Container container)
        {
            int registrationCount = 0;

            if (container.Registrations != null)
            {
                registrationCount = container.Registrations.Count;
            }

            return container.Name + " (" + registrationCount + ")";
        }

        /// <summary>
        /// Formats the name of a given <see cref="Type"/> to produce a more human-readable
        /// representation. If the type is generic, appends the generic type arguments in
        /// angle brackets. For non-generic types, simply returns the type's name. Handles
        /// null types by returning "null".
        /// </summary>
        /// <param name="type">The type to format, potentially including generic arguments.</param>
        /// <returns>A string representation of the formatted type name. For generic types,
        /// includes the type arguments enclosed in angle brackets.</returns>
        private string FormatTypeName(Type type)
        {
            if (type == null) return "null";

            if (!type.IsGenericType) return type.Name;
            
            string name = type.Name;
            int backtickIndex = name.IndexOf('`');

            if (backtickIndex > 0)
            {
                name = name.Substring(0, backtickIndex);
            }

            Type[] genericArgs = type.GetGenericArguments();
            string[] argNames = new string[genericArgs.Length];

            for (int i = 0; i < genericArgs.Length; i++)
            {
                argNames[i] = FormatTypeName(genericArgs[i]);
            }

            return name + "<" + string.Join(", ", argNames) + ">";

        }

        /// <summary>
        /// Counts the total number of Dependency Injection (DI) containers currently active
        /// in the application, including the application container and all scene containers.
        /// Also includes child containers by recursively traversing the container hierarchy.
        /// </summary>
        /// <returns>The total number of active DI containers.</returns>
        private int CountContainers()
        {
            int count = 0;

            if (ContainerRegistry.ApplicationContainer != null)
                count++;

            IReadOnlyDictionary<Scene, IContainer> sceneContainers = ContainerRegistry.AllSceneContainers;

            foreach (KeyValuePair<Scene, IContainer> kvp in sceneContainers)
            {
                count++;

                if (kvp.Value is not Container containerImpl) continue;
                
                count += CountChildrenRecursive(containerImpl);
            }

            return count;
        }

        /// <summary>
        /// Recursively counts all child containers of the specified DI container,
        /// including the children of its children, and so on.
        /// </summary>
        /// <param name="container">The container whose child hierarchy is to be counted.</param>
        /// <returns>The total number of child containers in the hierarchy, starting from the specified container.</returns>
        private int CountChildrenRecursive(Container container)
        {
            if (container.Children == null) return 0;

            int count = 0;

            foreach (Container child in container.Children)
            {
                count++;
                count += CountChildrenRecursive(child);
            }

            return count;
        }

        /// <summary>
        /// Creates a spacer element of the specified height to separate UI components
        /// in the details panel of the editor window.
        /// </summary>
        /// <param name="height">The height of the spacer in pixels.</param>
        /// <returns>A <see cref="VisualElement"/> configured as a spacer with the specified height.</returns>
        private VisualElement CreateSpacer(float height)
        {
            VisualElement spacer = new VisualElement();
            spacer.style.height = height;
            return spacer;
        }
    }
}