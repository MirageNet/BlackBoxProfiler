using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mirage.Profiler
{
    public class NetworkProfilerWindow : EditorWindow
    {
        #region Fields

        private VisualElement _rootElement;
        private Profiler _networkProfiler;
        private static string _rootElementsPath;

        #endregion

        #region Unity Methods

        public void OnEnable()
        {
            this.SetAntiAliasing(4);

            _rootElementsPath =
                AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:VisualTreeAsset Profiler")[0]);

            _rootElement = rootVisualElement;

            DrawProfiler();
        }

        public void Update()
        {
            _networkProfiler?.Update();
        }

        #endregion

        #region Misc

        [MenuItem("Window/Mirage/Network Profiler", false, 0)]
        public static void ShowWindow()
        {
            NetworkProfilerWindow window = GetWindow<NetworkProfilerWindow>("Network Profiler");
            window.titleContent.image = AssetDatabase.LoadAssetAtPath<Texture2D>("./Mirage/Icon/MirageIcon.png");
        }

        #endregion

        #region Drawing

        private void DrawProfiler()
        {
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(_rootElementsPath);

            VisualElement mainElement = visualTree.CloneTree();

            _rootElement.Add(mainElement);

            VisualTreeAsset refData = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:VisualTreeAsset ReferencesData")[0]));

            VisualElement refElement = refData.CloneTree();

            _rootElement.Q<VisualElement>("References").Add(refElement);

            _rootElement.Q<VisualElement>("Graph").Add(new DrawGraph(_rootElement.Q<VisualElement>().contentContainer));

            IEnumerable<VisualElement> children = refElement.Q<VisualElement>("ReferenceData").Children();

            foreach (VisualElement childElement in children)
            {
                childElement.Q<VisualElement>("ColumnName").Q<Label>("corner")
                    .AddManipulator(new ColumnManipulator(childElement));
            }

            _networkProfiler = new Profiler();

            _rootElement.Q<ToolbarToggle>("Record").RegisterCallback<ChangeEvent<bool>>(OnRecordToggle);
        }

        #endregion

        #region Event Callbacks

        /// <summary>
        ///     When user toggles record on and off we will update profiler to reflect the change here.
        /// </summary>
        /// <param name="toggled">The current state of the record button toggle.</param>
        private void OnRecordToggle(ChangeEvent<bool> toggled)
        {
            _networkProfiler.Record = toggled.newValue;
        }

        #endregion
    }

    public class DrawGraph : VisualElement
    {
        private VisualElement _worldElement;

        public DrawGraph(VisualElement element)
        {
            this.StretchToParentSize();
            generateVisualContent += OnGenerateVisualContent;
            _worldElement = element;
        }

        void OnGenerateVisualContent(MeshGenerationContext mgc)
        {
            Circle(_worldElement.worldBound.center, 1, 0, Mathf.PI * 2, Color.white, mgc);
            //MeshWriteData mesh = mgc.Allocate(3, 3);

            //Vector2 worldCenter = _worldElement.worldBound.center;

            //mesh.SetNextVertex(new Vertex { position = new Vector3(0, 0, Vertex.nearZ), tint = Color.white });
            //mesh.SetNextVertex(new Vertex { position = new Vector3(worldCenter.x, worldCenter.y, Vertex.nearZ), tint = Color.white });
            //mesh.SetNextVertex(new Vertex { position = new Vector3(-worldCenter.x, -worldCenter.y, Vertex.nearZ), tint = Color.white });

            //mesh.SetNextIndex(0);
            //mesh.SetNextIndex(1);
            //mesh.SetNextIndex(2);
        }

        public void Circle(Vector2 pos, float radius, float startAngle, float endAngle, Color color,
            MeshGenerationContext context)
        {
            color.a = 1;
            var segments = 50;
            var mesh = context.Allocate(segments + 1, (segments - 1) * 3);
            mesh.SetNextVertex(new Vertex() { position = new Vector2(pos.x, pos.y), tint = color });
            var angle = startAngle;
            var range = endAngle - startAngle;
            var step = range / (segments - 1);

            // store off the first position
            Vector2 offset = new Vector2(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle));
            mesh.SetNextVertex(new Vertex() { position = new Vector2(pos.x, pos.y) + offset, tint = color });

            // calculate the rest of the arc/circle
            for (var i = 0; i < segments - 1; i++)
            {
                angle += step;
                offset = new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle));
                mesh.SetNextVertex(new Vertex() { position = new Vector2(pos.x, pos.y) + offset, tint = color });
            }

            for (ushort i = 1; i < segments; i++)
            {
                mesh.SetNextIndex(0);
                mesh.SetNextIndex(i);
                mesh.SetNextIndex((ushort)(i + 1));
            }
        }
    }
}
