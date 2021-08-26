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

        private static string rootElementsPath;

        #endregion

        #region Unity Methods

        public void OnEnable()
        {
            rootElementsPath = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:VisualTreeAsset Profiler")[0]);

            _rootElement = rootVisualElement;

            DrawProfiler();
        }

        #endregion

        #region Misc

        [MenuItem("Window/Analysis/Mirage Network Profiler", false, 0)]
        public static void ShowWindow()
        {
            NetworkProfilerWindow window = GetWindow<NetworkProfilerWindow>("Mirage Network Profiler");
            window.titleContent.image = AssetDatabase.LoadAssetAtPath<Texture2D>("./Mirage/Icon/MirageIcon.png");
        }

        #endregion

        #region Drawing

        private void DrawProfiler()
        {
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(rootElementsPath);

            VisualElement mainElement = visualTree.CloneTree();

            _rootElement.Add(mainElement);

            VisualTreeAsset refData = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:VisualTreeAsset ReferencesData")[0]));

            VisualElement refElement = refData.CloneTree();

            _rootElement.Q<VisualElement>("References").Add(refElement);

            IEnumerable<VisualElement> children = refElement.Q<VisualElement>("ReferenceData").Children();

            foreach (VisualElement childElement in children)
            {
                childElement.Q<VisualElement>("ColumnName").Q<Label>("corner").AddManipulator(new ColumnManipulator(childElement));
            }
        }

        #endregion
    }
}
