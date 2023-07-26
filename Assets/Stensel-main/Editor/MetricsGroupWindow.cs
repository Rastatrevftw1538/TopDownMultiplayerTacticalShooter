using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Stensel.Configuration;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Attribute = Stensel.Configuration.Attribute;

namespace Stensel.Editor {
    public class MetricsGroupWindow : EditorWindow {
        private static MetricsGroup _mG;
        private static bool _temp = true;
        
        private enum SelectionMode {
            Data,
            Tables,
            TimeTables
        }

        private Texture AddIcon;
        private Texture RemoveIcon;

        private readonly int[] _removeSelection = Array.Empty<int>();
        private readonly List<Table> _viewableTables = new();
        private readonly List<TimeTable> _viewableTimeTables = new();
        private ListView _tablesView;
        private ListView _timeTablesView;
        private Label _dataLabel;
        private ToolbarSearchField _search;

        private VisualElement _editorView;

        private ListView _selectedView;
        private int _selectedIndex;
        private IMetric _selected;
        private bool _reloaded = false;
        private string path;
        
        [MenuItem("Window/Stensel/Metrics Group")]
        private static MetricsGroupWindow Open() {
            var window = GetWindow<MetricsGroupWindow>();
            
            window.titleContent = new GUIContent("Metrics Editor");
            window.Show();

            return window;
        }

        [OnOpenAsset]
        public static bool OpenMetricsGroup(int groupID, int line) {
            var obj = EditorUtility.InstanceIDToObject(groupID);
            if (obj is not MetricsGroup mg) return false;

            _mG = CreateInstance<MetricsGroup>();
            var assetPath = AssetDatabase.GetAssetPath(groupID);
            JsonUtility.FromJsonOverwrite(File.ReadAllText(assetPath), _mG);
            _mG.hideFlags = HideFlags.HideAndDontSave;
            _temp = false;

            var window = Open();
            window.path = assetPath;
            window._reloaded = false;
            
            return true;
        }
        
        private void OnEnable() {
            _reloaded = false;
        }

        public void Update() {
            if (_reloaded) return;
            LoadUI();
            _reloaded = true;
        }

        protected void OnDestroy() {
            if (_temp) DestroyImmediate(_mG);
            else _mG = null;
        }

        public void LoadUI() {
            if (_mG == null) {
                _mG = CreateInstance<MetricsGroup>();
                _mG.hideFlags = HideFlags.HideAndDontSave;
                _temp = true;
                path = null;
            }
            rootVisualElement.Clear();
            
            AddIcon = Utils.GetIcon("Toolbar Plus@2x");
            RemoveIcon = Utils.GetIcon("Toolbar Minus@2x");
            var split = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal) {
                style = {
                    width = new StyleLength(new Length(100, LengthUnit.Percent)),
                    maxHeight = new StyleLength(new Length(100, LengthUnit.Percent)),
                    flexGrow = new StyleFloat(1f),
                    flexShrink = new StyleFloat(0f),
                }
            };
            rootVisualElement.Add(split);

            var metricsList = MetricsList();
            _editorView = new VisualElement();
            
            split.Add(metricsList);
            split.Add(_editorView);
        }

        private VisualElement MetricsList() {
            var main = new ScrollView(ScrollViewMode.Vertical) {
                style = {
                    flexDirection = FlexDirection.Row,
                    minWidth = new StyleLength(new Length(250, LengthUnit.Pixel)),
                },
                horizontalScrollerVisibility = ScrollerVisibility.Hidden
            };

            var toolbar = new Toolbar {
                style = {
                    width = new StyleLength(new Length(100, LengthUnit.Percent)),
                    maxWidth = new StyleLength(new Length(100, LengthUnit.Percent))
                }
            };

            var loadButton = new ToolbarButton(() => {
                var loadPath = EditorUtility.OpenFilePanelWithFilters(
                    "Open Metrics Group",
                    "Assets/",
                    new[] {"Metrics Group", "stensel"}
                );

                if (loadPath.Length <= 0) return;
                
                JsonUtility.FromJsonOverwrite(File.ReadAllText(loadPath), _mG);
                _mG.hideFlags = HideFlags.HideAndDontSave;
                path = loadPath;
                
                LoadUI();
            }) {
                text = "Load"
            };
            
            var saveButton = new ToolbarButton(() => {
                if (string.IsNullOrEmpty(path)) {
                    var savePath = EditorUtility.SaveFilePanelInProject(
                        "Save Metrics Group",
                        "Metrics",
                        "stensel",
                        "Enter a filename to save the Metrics Group"
                    );
                    
                    if (savePath.Length <= 0) return;
                    
                    File.WriteAllText(savePath, JsonUtility.ToJson(_mG, true));
                    path = savePath;
                }
                else File.WriteAllText(path, JsonUtility.ToJson(_mG, true));

                AssetDatabase.Refresh();
            }) {
                text = "Save"
            };

            _search = new ToolbarSearchField() {
                style = {
                    flexGrow = 1f,
                    flexShrink = 1f,
                },
            };

            var addButton = new ToolbarMenu() {
                style = {
                    flexShrink = 0,
                    flexGrow = 0
                }
            };
            addButton.Insert(0, new Image {
                image = AddIcon,
                scaleMode = ScaleMode.ScaleToFit,
                style = { width = EditorGUIUtility.singleLineHeight * 0.8f }
            });

            addButton.menu.AppendAction("Add Table", AddTable);
            addButton.menu.AppendAction("Add Time Table", AddTimeTable);

            var removeButton = new ToolbarButton(() => {
                if (_selectedView == null || _selectedIndex < 0) return;

                IList source;
                ListView lv;
                
                if (_selectedView == _tablesView) {
                    lv = _tablesView;
                    source = _tablesView.itemsSource;
                    var table = (Table)source[_selectedIndex];
                    _mG.tables.Remove(table);
                }
                else {
                    lv = _timeTablesView;
                    source = _timeTablesView.itemsSource;
                    var timeTable = (TimeTable)source[_selectedIndex];
                    _mG.timeTables.Remove(timeTable);
                }
                
                source.RemoveAt(_selectedIndex);
                lv.Rebuild();
            }) {
                style = {
                    flexShrink = 0,
                    flexGrow = 0
                }
            };
            removeButton.Add(new Image {
                image = RemoveIcon,
                scaleMode = ScaleMode.ScaleToFit,
                style = { width = EditorGUIUtility.singleLineHeight * 0.8f }
            });
            
            toolbar.Add(loadButton);
            toolbar.Add(saveButton);
            toolbar.Add(_search);
            toolbar.Add(addButton);
            toolbar.Add(removeButton);
            
            main.Add(toolbar);
            
            //_dataView = SetupList("Data", _mG.data, _viewableData, SelectionMode.Data);
            
            _tablesView = SetupList(
                "Tables",
                _mG.tables,
                _viewableTables,
                SelectionMode.Tables,
                LoadTableEditor
            );
            
            _timeTablesView = SetupList(
                "Time Tables",
                _mG.timeTables,
                _viewableTimeTables,
                SelectionMode.TimeTables,
                LoadTimeTableEditor
            );

            _search.RegisterValueChangedCallback(e => {
                FilterList(e.newValue, _mG.tables, _viewableTables, _tablesView);
                FilterList(e.newValue, _mG.timeTables, _viewableTimeTables, _timeTablesView);
            });

            _dataLabel = new Label("Data");
            _dataLabel.RegisterCallback<ClickEvent>(e => {
                _dataLabel.style.backgroundColor = GUI.skin.settings.selectionColor;
                if (_selectedIndex == -2) return;
                _selectedIndex = -2;
                LoadDataEditor();
                UpdateMajorSelection(SelectionMode.Data);
            });
            
            main.Add(_dataLabel);
            main.Add(_tablesView);
            main.Add(_timeTablesView);

            return main;
        }

        private void AddTable(DropdownMenuAction action) {
            var newTable = new Table() {
                name = "Table" + (_mG.tables.Count > 0 ? (" " + (_mG.tables.Count + 1)) : ""),
            };

            _mG.tables.Add(newTable);
            if (string.IsNullOrEmpty(_search.value)) {
                _tablesView.itemsSource.Add(newTable);
                _tablesView.Rebuild();
            }
            else {
                _search.SetValueWithoutNotify("");
                FilterList("", _mG.tables, _viewableTables, _tablesView);
                FilterList("", _mG.timeTables, _viewableTimeTables, _timeTablesView);
            }
        }

        private void AddTimeTable(DropdownMenuAction action) {
            var newTimeTable = new TimeTable() {
                name = "Time Table" + (_mG.timeTables.Count > 0 ? (" " + (_mG.timeTables.Count + 1)) : "")
            };

            _mG.timeTables.Add(newTimeTable);
            if (string.IsNullOrEmpty(_search.value)) {
                _timeTablesView.itemsSource.Add(newTimeTable);
                _timeTablesView.Rebuild();
            }
            else {
                _search.SetValueWithoutNotify("");
                FilterList("", _mG.tables, _viewableTables, _tablesView);
                FilterList("", _mG.timeTables, _viewableTimeTables, _timeTablesView);
            }
        }

        private ListView SetupList<T>(string listName, List<T> reference, List<T> viewable, SelectionMode mode, Action<T> viewEditor) where T : IMetric {
            viewable.Clear();
            viewable.AddRange(reference);

            Action<VisualElement, int> bind = (e, i) => ((Label) e).text = viewable[i].Name;

            var lv = new ListView(viewable, EditorGUIUtility.singleLineHeight, MakeLabel, bind) {
                selectionType = SelectionType.Single,
                showFoldoutHeader = true,
                headerTitle = listName,
                reorderable = false,
                showBoundCollectionSize = false,
                showAddRemoveFooter = false,
            };

            lv.onSelectedIndicesChange += (indices) => {
                var index = -1;
                foreach (var idx in indices) {
                    index = idx;
                    break;
                }

                _selected = index < 0 ? null : viewable[index];
                _selectedView = lv;
                _selectedIndex = index;
                UpdateMajorSelection(mode);
                
                if (_selectedIndex >= 0) viewEditor(viewable[index]);
            };

            return lv;
        }

        private void FilterList<T>( string filter, List<T> reference, List<T> viewable, ListView view) where T : IMetric {
            viewable.Clear();

            view.itemsSource = viewable;
            view.SetSelectionWithoutNotify(_removeSelection);

            filter = filter.ToLowerInvariant();
            
            if (string.IsNullOrEmpty(filter)) viewable.AddRange(reference);
            else {
                for (var i = 0; i < reference.Count; i++) {
                    if (reference[i].Name.ToLower().Contains(filter))
                        viewable.Add(reference[i]);
                }
            }
            
            view.Rebuild();
        }

        private void UpdateMajorSelection(SelectionMode mode) {
            if (mode is not SelectionMode.Data) {
                _dataLabel.style.backgroundColor = new StyleColor(Color.clear);
            }
            
            if (mode is not SelectionMode.Tables) {
                _tablesView.SetSelectionWithoutNotify(_removeSelection);
            }
            
            if (mode is not SelectionMode.TimeTables) {
                _timeTablesView.SetSelectionWithoutNotify(_removeSelection);
            }
        }

        private VisualElement MakeLabel() {
            return new Label();
        }

        private void LoadDivider() {
            var background = EditorGUIUtility.isProSkin ? 0.15f : 0.7f;
            var divColor = new Color(background, background, background, 1f);
            var divider = new VisualElement {
                style = {
                    width = new StyleLength(new Length(8, LengthUnit.Pixel)),
                    height = new StyleLength(new Length(100, LengthUnit.Percent)),
                    backgroundColor = new StyleColor(divColor)
                }
            };
            _editorView.Add(divider);
        }

        private void LoadDataEditor() {
            _editorView.Clear();
            _editorView.style.flexDirection = FlexDirection.Row;

            LoadDivider();
            
            _editorView.Add(new AttributeList<Datum>(_mG.data));
        }
        
        private void LoadTableEditor(Table table) {
            _editorView.Clear();
            _editorView.style.flexDirection = FlexDirection.Row;

            LoadDivider();

            _editorView.Add(new TableEditor(table, () => {
                _tablesView.RefreshItem(_viewableTables.IndexOf(table));
            }));
        }
        
        private void LoadTimeTableEditor(TimeTable table) {
            _editorView.Clear();
            _editorView.style.flexDirection = FlexDirection.Row;

            LoadDivider();

            _editorView.Add(new TimeTableEditor(table, () => {
                _timeTablesView.RefreshItem(_viewableTimeTables.IndexOf(table));
            }));
        }
    }
}