using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using TechC.InGame.Map;

namespace TechC.Editor.InGame
{
    /// <summary>
    /// MapGeneratorのカスタムエディタ
    /// エディタ上でのマップ一括生成・削除を提供する
    /// </summary>
    [CustomEditor(typeof(MapGenerator))]
    public class MapGeneratorEditor : UnityEditor.Editor
    {
        private const string MenuGenerateMap = "TechC/Map/マップを生成 #&t";
        private const string MenuDeleteMap = "TechC/Map/マップを削除 #&d";

        [MenuItem(MenuGenerateMap)]
        private static void GenerateMapShortcut()
        {
            var generator = FindMapGeneratorInScene();
            if (generator == null) return;

            var editor = (MapGeneratorEditor)CreateEditor(generator);
            editor.GenerateMap();
            DestroyImmediate(editor);
        }

        [MenuItem(MenuDeleteMap)]
        private static void DeleteMapShortcut()
        {
            var generator = FindMapGeneratorInScene();
            if (generator == null) return;

            var editor = (MapGeneratorEditor)CreateEditor(generator);
            editor.DeleteMap();
            DestroyImmediate(editor);
        }

        private static MapGenerator FindMapGeneratorInScene()
        {
            var generator = Object.FindFirstObjectByType<MapGenerator>();
            if (generator == null)
                Debug.LogError("[MapGeneratorEditor] シーン上に MapGenerator が見つかりません。");
            return generator;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();

            if (GUILayout.Button("マップを生成"))
                GenerateMap();

            if (GUILayout.Button("マップを削除"))
                DeleteMap();
        }

        private void GenerateMap()
        {
            var mapData = serializedObject.FindProperty("_mapData").objectReferenceValue as MapDataSO;
            var tileParent = serializedObject.FindProperty("_tileParent").objectReferenceValue as Transform;

            if (mapData == null)
            {
                Debug.LogError("[MapGeneratorEditor] MapDataSO がアサインされていません。");
                return;
            }

            if (mapData.TilePrefab == null)
            {
                Debug.LogError("[MapGeneratorEditor] TilePrefab がアサインされていません。");
                return;
            }

            if (tileParent == null)
            {
                Debug.LogError("[MapGeneratorEditor] TileParent がアサインされていません。");
                return;
            }

            DeleteMap(tileParent);

            Undo.SetCurrentGroupName("マップを生成");
            int undoGroup = Undo.GetCurrentGroup();

            float offsetX = (mapData.Columns - 1) * mapData.TileSpacing / 2f;
            float offsetZ = (mapData.Rows - 1) * mapData.TileSpacing / 2f;

            for (int row = 0; row < mapData.Rows; row++)
            {
                for (int col = 0; col < mapData.Columns; col++)
                {
                    var worldPos = new Vector3(
                        col * mapData.TileSpacing - offsetX,
                        0f,
                        row * mapData.TileSpacing - offsetZ
                    );
                    var tileObject = (GameObject)PrefabUtility.InstantiatePrefab(mapData.TilePrefab, tileParent);
                    tileObject.transform.position = worldPos;
                    tileObject.name = $"Tile({col},{row})";
                    Undo.RegisterCreatedObjectUndo(tileObject, "Generate Tile");
                }
            }

            Undo.CollapseUndoOperations(undoGroup);
            EditorSceneManager.MarkSceneDirty(tileParent.gameObject.scene);
        }

        private void DeleteMap()
        {
            var tileParent = serializedObject.FindProperty("_tileParent").objectReferenceValue as Transform;
            DeleteMap(tileParent);
        }

        private void DeleteMap(Transform tileParent)
        {
            if (tileParent == null) return;

            var children = new List<GameObject>();
            foreach (Transform child in tileParent)
                children.Add(child.gameObject);

            Undo.SetCurrentGroupName("マップを削除");
            int undoGroup = Undo.GetCurrentGroup();

            foreach (var child in children)
                Undo.DestroyObjectImmediate(child);

            Undo.CollapseUndoOperations(undoGroup);
            EditorSceneManager.MarkSceneDirty(tileParent.gameObject.scene);
        }
    }
}
