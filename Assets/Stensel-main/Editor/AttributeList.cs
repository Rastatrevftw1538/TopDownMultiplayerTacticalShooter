using System;
using System.Collections.Generic;
using Stensel.Configuration;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Stensel.Editor {
    public class AttributeList<T> : VisualElement where T : IAttributeWrapper, new() {
        private static Texture _addIcon;
        private static Texture _removeIcon;

        private readonly ListView _list;
        private List<T> _viewable = new();

        private readonly int[] _removeSelection = Array.Empty<int>();
        private int _selectedIndex = -1;
        
        public AttributeList(List<T> attributes) {
            if (_addIcon == null) _addIcon = Utils.GetIcon("Toolbar Plus@2x");
            if (_removeIcon == null) _removeIcon = Utils.GetIcon("Toolbar Minus@2x");
            
            style.maxWidth = new StyleLength(new Length(100, LengthUnit.Percent));
            style.maxHeight = new StyleLength(new Length(100, LengthUnit.Percent));
            style.flexGrow = new StyleFloat(1f);
            style.flexShrink = new StyleFloat(0f);

            _viewable.AddRange(attributes);
            
            _list = new ListView(
                _viewable,
                Utils.lineHeight,
                () => new AttributeElement(),
                (v, i) => ((AttributeElement) v).Bind(attributes[i].Attr)
            ){
                selectionType = SelectionType.Single,
                showFoldoutHeader = false,
                reorderable = false,
                showBoundCollectionSize = false,
                showAddRemoveFooter = false,
            };
            
            _list.onSelectedIndicesChange += (indices) => {
                var index = -1;
                foreach (var idx in indices) {
                    index = idx;
                    break;
                }
                
                _selectedIndex = index;
            };
            
            var toolbar = new Toolbar() {
                style = {
                    width = new StyleLength(new Length(100, LengthUnit.Percent))
                }
            };

            var search = new ToolbarSearchField() {
                style = {
                    flexGrow = 1f,
                    flexShrink = 1f,
                },
            };

            var addButton = new ToolbarButton(() => {
                var newItem = new T();
                newItem.Attr.name = "Attribute" + (attributes.Count > 0 ? (" " + (attributes.Count + 1)) : "");
                
                attributes.Add(newItem);
                if (string.IsNullOrEmpty(search.value)) {
                    _list.itemsSource.Add(newItem);
                    _list.Rebuild();
                }
                else {
                    search.SetValueWithoutNotify("");
                    FilterList("", attributes, _viewable);
                }
            }) {
                style = {
                    flexShrink = 0,
                    flexGrow = 0
                }
            };
            addButton.Insert(0, new Image() {
                image = _addIcon,
                scaleMode = ScaleMode.ScaleToFit,
                style = { width = EditorGUIUtility.singleLineHeight * 0.8f }
            });

            var removeButton = new ToolbarButton(() => {
                if (_selectedIndex < 0) return;

                var item = _viewable[_selectedIndex];
                attributes.Remove(item);
                _viewable.RemoveAt(_selectedIndex);
                _list.Rebuild();
            }) {
                style = {
                    flexShrink = 0,
                    flexGrow = 0
                }
            };
            removeButton.Add(new Image() {
                image = _removeIcon,
                scaleMode = ScaleMode.ScaleToFit,
                style = { width = EditorGUIUtility.singleLineHeight * 0.8f }
            });
            
            toolbar.Add(search);
            toolbar.Add(addButton);
            toolbar.Add(removeButton);
            
            Add(toolbar);
            Add(_list);
        }

        private void FilterList( string filter, List<T> reference, List<T> viewable) {
            viewable.Clear();

            _list.itemsSource = viewable;
            _list.SetSelectionWithoutNotify(_removeSelection);

            filter = filter.ToLowerInvariant();
            
            if (string.IsNullOrEmpty(filter)) viewable.AddRange(reference);
            else {
                for (var i = 0; i < reference.Count; i++) {
                    if (reference[i].Attr.name.ToLower().Contains(filter))
                        viewable.Add(reference[i]);
                }
            }
            
            _list.Rebuild();
        }
    }
}