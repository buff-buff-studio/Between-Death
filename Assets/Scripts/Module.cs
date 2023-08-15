using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Rendering;
using IProvider = UnityEditor.Rendering.FilterWindow.IProvider;
using Element = UnityEditor.Rendering.FilterWindow.Element;
using GroupElement = UnityEditor.Rendering.FilterWindow.GroupElement;
#endif

namespace Refactor
{
    [Serializable]
    public abstract class Module 
    {
        public bool enabled
        {
            get => _enabled;
            set 
            {
                _enabled = value;
                
                #if UNITY_EDITOR
                            if (!Application.isPlaying) return;
                            if(value)
                                OnEnable();
                            else
                                OnDisable();
                #else
                            if(value)
                                    OnEnable();
                                else
                                    OnDisable();
                #endif
            }
        }

        [SerializeField, HideInInspector]
        // ReSharper disable once InconsistentNaming
        private bool _enabled = true;

        public void SetEnabled(bool value) => this.enabled = value;
        
        public virtual void OnEnable() {}
        public virtual void OnDisable() {}

#if UNITY_EDITOR
        [SerializeField, HideInInspector]
        public bool editorOpen = true;
    #endif
        public virtual Color GetColor() => new Color(1f, 0.25f, 0f, 0.1f);
    }

    public interface IModuleSet
    {
        // ReSharper disable once InconsistentNaming
        public int ModuleCount { get; }
        public TM AddModule<TM>() where TM : Module;
        public Module AddModule(Type type);

        public bool RemoveModule(Module module);
        public void RemoveModuleAt(int index);

        public TM GetModule<TM>() where TM : Module;
        public IEnumerable<TM> GetModules<TM>() where TM : Module;
        public Module GetModuleAt(int index);

        public IEnumerable<Module> GetModules();
        public int IndexOf(Module module);
        
        #if UNITY_EDITOR
        public void _m_SwapModules(int a, int b);
        public IEnumerable<Type> _m_GetModuleTypes();
        #endif
    }

    [Serializable]
    public class ModuleSet<T> : IEnumerable, IModuleSet where T : Module
    {
        #region Fields
    #if UNITY_EDITOR
        [SerializeField, HideInInspector]
        public bool editorOpen = true;
    #endif
        
        [SerializeReference] 
        // ReSharper disable once InconsistentNaming
        private Module[] _modules = Array.Empty<Module>();
        #endregion

        #region Callbacks
        public Action<T> onAddModule;
        public Action<T> onRemoveModule;
        #endregion

        #region Methods
        public int ModuleCount => _modules.Length;
        public TM AddModule<TM>() where TM : Module
        {
            if (!typeof(T).IsAssignableFrom(typeof(TM))) return default(TM);
            var module = Activator.CreateInstance<TM>();
            return AddModule(module) ? module : null;
        }

        public Module AddModule(Type type)
        {
            if (!typeof(T).IsAssignableFrom(type)) return (Module) type.Default();
            var module = (Module) Activator.CreateInstance(type);
            return AddModule(module) ? module : null;
        }
        
        public bool AddModule(Module module)
        {
            if (module is not T m) return false;
            
            Array.Resize(ref _modules, _modules.Length + 1);
            _modules[^1] = m;
            onAddModule?.Invoke(m);
            return true;
        }

        public bool RemoveModule(Module module)
        {
            var i = Array.IndexOf(_modules, module);
            if (i == -1) return false;
            RemoveModuleAt(i);
            return true;
        }

        public void RemoveModuleAt(int index)
        {
            var module = _modules[index];
            
            for (var j = index + 1; j < _modules.Length; j++)
                _modules[j - 1] = _modules[j];
            
            Array.Resize(ref _modules, _modules.Length - 1);
            onRemoveModule?.Invoke((T) module);
        }

        public TM GetModule<TM>() where TM : Module
        {
            foreach (var m in _modules)
                if (m is TM tm)
                    return tm;

            return null;
        }

        public IEnumerable<TM> GetModules<TM>() where TM : Module
        {
            return _modules.OfType<TM>();
        }

        public Module GetModuleAt(int index)
        {
            return _modules[index];
        }

        public IEnumerable<Module> GetModules()
        {
            return _modules;
        }

        public int IndexOf(Module module)
        {
            return Array.IndexOf(_modules, module);
        }
        
    #if UNITY_EDITOR
        public void _m_SwapModules(int a, int b)
        {
            (_modules[a], _modules[b]) = (_modules[b], _modules[a]);
        }

        public IEnumerable<Type> _m_GetModuleTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(domainAssembly => domainAssembly.GetTypes())
                .Where(type =>
                    typeof(T).IsAssignableFrom(type)
                    && !type.IsAbstract
                    && type.IsSerializable
                );
        }
    #endif
        
        public IEnumerator GetEnumerator() => _modules.GetEnumerator();
        #endregion
    }

    public abstract class ModularBehaviour<T> : MonoBehaviour, IEnumerable where T : Module
    {
        [SerializeField]
        // ReSharper disable once InconsistentNaming
        private ModuleSet<T> _modules = new ModuleSet<T>();
        
        // ReSharper disable once InconsistentNaming
        public int ModuleCount => _modules.ModuleCount;
        public TM AddModule<TM>() where TM : Module => _modules.AddModule<TM>();
        public Module AddModule(Type type) => _modules.AddModule(type);

        public bool RemoveModule(Module module) => _modules.RemoveModule(module);
        public void RemoveModuleAt(int index) => _modules.RemoveModuleAt(index);

        public TM GetModule<TM>() where TM : Module => _modules.GetModule<TM>();
        public IEnumerable<TM> GetModules<TM>() where TM : Module => _modules.GetModules<TM>();
        public Module GetModuleAt(int index) => _modules.GetModuleAt(index);

        public IEnumerable<Module> GetModules() => _modules.GetModules();
        public int IndexOf(Module module) => _modules.IndexOf(module);
        
        public IEnumerator GetEnumerator() => _modules.GetEnumerator();
        
    #if UNITY_EDITOR
        public void _m_SwapModules(int a, int b) => _modules._m_SwapModules(a, b);
        public IEnumerable<Type> _m_GetModuleTypes() => _modules._m_GetModuleTypes();
    #endif

        public virtual void OnValidate()
        {
            _modules.onAddModule -= OnAddModule;
            _modules.onAddModule += OnAddModule;
            _modules.onRemoveModule -= OnRemoveModule;
            _modules.onRemoveModule += OnRemoveModule;
        }
        
        public virtual void OnEnable()
        {
            foreach (T module in _modules)
            {
                if (module.enabled)
                    module.OnEnable();
            }
        }
        
        public virtual void OnDisable()
        {
            foreach (T module in _modules)
                if(module.enabled)
                    module.OnDisable();
        }

        #region Callbacks
        protected virtual void OnAddModule(T module) {}
        protected virtual void OnRemoveModule(T module) {}
        #endregion
    }

    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ModuleSet<>), true)]
    public class ModuleSetDrawer : PropertyDrawer
    {
        public static SerializedProperty Clipboard;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            GUILayout.Space(-20);
            var boxStyle = new GUIStyle( EditorStyles.helpBox )
            {
                normal =
                {
                    background = MakeTex( 8, 8, new Color(1f, 0.25f, 0f, 0.1f), new Color(0, 0, 0, 0.5f))
                }
            };
            GUILayout.BeginVertical(boxStyle);
            
            var items = property.FindPropertyRelative("_modules");
            var editorOpen = property.FindPropertyRelative("editorOpen");
            var length = items?.arraySize ?? 0;
            
            //Remove null items
            for (var i = 0; i < length; i++)
            {
                var item = items!.GetArrayElementAtIndex(i);
                var module = ((Module)item.managedReferenceValue);

                if (module is not null) continue;
                items.DeleteArrayElementAtIndex(i);
                length--;
                i --;
            }

            editorOpen.boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(editorOpen.boolValue, label);
            EditorGUILayout.EndFoldoutHeaderGroup();

            var toEnable = new List<Module>();
            var toDisable = new List<Module>();
            
            if (editorOpen.boolValue)
            {
                for (var i = 0; i < length; i++)
                {
                    var item = items!.GetArrayElementAtIndex(i);
                    var module = ((Module)item.managedReferenceValue);
                    
                    var itemEnabled = item.FindPropertyRelative("_enabled");
                    var itemEditorOpen = item.FindPropertyRelative("editorOpen");
                    
                    var moduleBoxStyle = new GUIStyle( EditorStyles.helpBox )
                    {
                        normal =
                        {
                            background = MakeTex( 8, 8, module.GetColor(), new Color(0, 0, 0, 0.5f))
                        }
                    };

                    GUILayout.BeginVertical(moduleBoxStyle);

                    #region Header
                    var index = i;
                    var isOpen =
                        EditorGUILayout.BeginFoldoutHeaderGroup(itemEditorOpen.boolValue, GUIContent.none, null, (rect) =>
                        {
                            _ShowDropdown(rect, property, index, length);
                        });
                    EditorGUILayout.EndFoldoutHeaderGroup();
                    
                    var headerRect = GUILayoutUtility.GetLastRect();
                    headerRect.x += 20;
                    GUI.Label(headerRect, EditorGUIUtility.FindTexture("cs Script Icon"));

                    var rectToggle = new Rect(headerRect.x + 20, headerRect.y, 20, headerRect.height);
                    EditorGUI.Toggle(rectToggle, GUIContent.none, itemEnabled.boolValue);
                    
                    var rectLabel = new Rect(headerRect.x + 38, headerRect.y - 1, headerRect.width - 60, headerRect.height);
                    GUI.Label(rectLabel, item.managedReferenceValue.GetType().Name, EditorStyles.boldLabel);
                    
                    if (isOpen != itemEditorOpen.boolValue)
                    {
                        var mousePosition = Event.current.mousePosition;
                        if (rectToggle.x <= mousePosition.x && rectToggle.x + rectToggle.width >= mousePosition.x)
                        {
                            Undo.RecordObject(property.serializedObject.targetObject, "Toggle Module Enabled");
                            if(!itemEnabled.boolValue) toEnable.Add(module); else toDisable.Add(module);
                        }
                        else
                        {
                            itemEditorOpen.boolValue = isOpen;
                        }
                    }
                    #endregion

                    #region Body
                    if (itemEditorOpen.boolValue)
                    {
                        var enumerator = item.GetEnumerator();
                        var dots = item.propertyPath.Count(c => c == '.');
                        while (enumerator.MoveNext())
                        {
                            if (enumerator.Current is not SerializedProperty prop) continue;
                            if (prop.propertyPath.Count(c => c == '.') != dots + 1) continue;
                            EditorGUILayout.PropertyField(prop);
                        }
                    }
                    #endregion

                    GUILayout.EndVertical();
                    GUILayout.Space(5);
                }
                
                using var hScope = new EditorGUILayout.HorizontalScope();
                
                if (GUILayout.Button(EditorGUIUtility.TrTextContent("Add Module"), EditorStyles.miniButton))
                {
                    var r = hScope.rect;
                    var pos = new Vector2(r.x + r.width / 2f, r.yMax + 18f);
                    FilterWindow.Show(pos, new ModuleSetProvider((IModuleSet) property.GetUnderlyingValue(), property.serializedObject));
                }
            }
            
            GUILayout.EndVertical();
            EditorGUI.EndProperty();
            
            foreach (var module in toEnable)
                module.enabled = true;
            
            foreach (var module in toDisable)
                module.enabled = false;
        }
        
        // ReSharper disable once MemberCanBeMadeStatic.Local
        private void _ShowDropdown(Rect position, SerializedProperty property, int index, int length)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Remove"), false, () =>
            {
                //property.serializedObject.CopyFromSerializedProperty();
                Undo.RecordObject(property.serializedObject.targetObject, "Remove Module");
                        
                ((IModuleSet) property.GetUnderlyingValue()).RemoveModuleAt(index);
            });
            
            if (index == 0)
                menu.AddDisabledItem(new GUIContent("Move Up"));
            else
                menu.AddItem(new GUIContent("Move Up"), false, () =>
                {
                    Undo.RecordObject(property.serializedObject.targetObject, "Swap Module Order");
                            
                    ((IModuleSet) property.GetUnderlyingValue())._m_SwapModules(index, index - 1);
                });
            
            if (index == length - 1)
                menu.AddDisabledItem(new GUIContent("Move Down"));
            else
                menu.AddItem(new GUIContent("Move Down"), false, () =>
                {
                    Undo.RecordObject(property.serializedObject.targetObject, "Swap Module Order");
                            
                    ((IModuleSet) property.GetUnderlyingValue())._m_SwapModules(index, index + 1);
                });
            
            menu.DropDown(position);
        }
        
        private Texture2D MakeTex( int width, int height, Color color, Color border)
        {
            Color[] pix = new Color[width * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if(x == 0 || x == width - 1 || y == 0 || y == height - 1)
                        pix[y * width + x] = border;
                    else
                        pix[y * width + x] = color;
                }
            }
            
            Texture2D result = new Texture2D( width, height );
            result.SetPixels( pix );
            result.Apply();
            return result;
        }
    }

    public class ModuleSetProvider : IProvider
    {
        private class ModuleSetElement : Element
        {
            public readonly Type Type;

            public ModuleSetElement(int level, string label, Type type)
            {
                this.level = level;
                this.Type = type;

                content = new GUIContent(label);
            }
        }

        private class PathNode : IComparable<PathNode>
        {
            public readonly List<PathNode> Nodes = new List<PathNode>();
            public string Name;
            public Type Type;

            public int CompareTo(PathNode other)
            {
                return string.Compare(Name, other.Name, StringComparison.Ordinal);
            }
        }

        public Vector2 position { get; set; }
        private readonly IModuleSet _moduleSet;
        private readonly SerializedObject _serializedObject;

        public void CreateComponentTree(List<Element> tree)
        {
            tree.Add(new GroupElement(0, "Module List"));

            var rootNode = new PathNode();

            foreach (var t in _moduleSet._m_GetModuleTypes())
                AddNode(rootNode, t.Name, t);
            
            Traverse(rootNode, 1, tree);
        }
        
        public ModuleSetProvider(IModuleSet moduleSet, SerializedObject serializedObject)
        {
            this._moduleSet = moduleSet;
            this._serializedObject = serializedObject;
        }

        public bool GoToChild(Element element, bool addIfComponent)
        {
            if (element is not ModuleSetElement setElement) return false;

            Undo.RecordObject(_serializedObject.targetObject, "Add Module");
            _moduleSet.AddModule(setElement.Type);
                
            return true;
        }

        private static void AddNode(PathNode root, string path, Type type)
        {
            var current = root;
            var parts = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in parts)
            {
                var child = current.Nodes.Find(x => x.Name == part);

                if (child == null)
                {
                    child = new PathNode { Name = part, Type = type };
                    current.Nodes.Add(child);
                }

                current = child;
            }
        }

        private static void Traverse(PathNode node, int depth, List<Element> tree)
        {
            node.Nodes.Sort();

            foreach (var n in node.Nodes)
            {
                if (n.Nodes.Count > 0)
                {
                    tree.Add(new GroupElement(depth, n.Name));
                    Traverse(n, depth + 1, tree);
                }
                else
                {
                    tree.Add(new ModuleSetElement(depth, n.Name, n.Type));
                }
            }
        }
    }
    #endif
}
