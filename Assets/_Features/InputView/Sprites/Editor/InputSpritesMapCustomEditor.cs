using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

namespace Lamou.InputSystem.SpriteMap.EditorUtilities
{
    [CustomEditor(typeof(InputSpritesMap))]
    public class InputSpritesMapCustomEditor : Editor
    {
        InputSpritesMap _inputSpritesMap;

        private void OnEnable()
        {
            _inputSpritesMap = target as InputSpritesMap;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Create Sprite Assets"))
            {
                Create();
            }
        }

        private void Create()
        {
            for (int i = 0; i < _inputSpritesMap._inputMaps.Length; i++)
            {
                ControlSchemeSpriteData element = _inputSpritesMap._inputMaps[i];

                UpdateName(element.BindingsPreset.Bindings, element.SpriteAtlas);

                string filePathWithName = AssetDatabase.GetAssetPath(element.SpriteAtlas);
                string fileNameWithExtension = Path.GetFileName(filePathWithName);
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePathWithName);
                string filePath = filePathWithName.Replace(fileNameWithExtension, "");

                string fromPath = filePath + $"{fileNameWithoutExtension}.asset";
                string toPath = $"Assets/Resources/Sprite Assets/{fileNameWithoutExtension}.asset";
                string newName = $"SpriteAsset_{element.DeviceLayoutName}";
                string newPath = $"Assets/Resources/Sprite Assets/{newName}.asset";

                Selection.activeObject = element.SpriteAtlas;

                if (AssetDatabase.LoadMainAssetAtPath(fromPath))
                {
                    AssetDatabase.DeleteAsset(toPath);
                }

                TMPro.EditorUtilities.TMP_SpriteAssetMenu.CreateSpriteAsset();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                if (!AssetDatabase.IsValidFolder("Assets/Resources/Sprite Assets"))
                {
                    if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                    {
                        AssetDatabase.CreateFolder("Assets", "Resources");
                    }

                    AssetDatabase.CreateFolder("Assets/Resources", "Sprite Assets");
                }

                if (AssetDatabase.LoadMainAssetAtPath(toPath))
                {
                    AssetDatabase.DeleteAsset(toPath);
                }

                if (AssetDatabase.LoadMainAssetAtPath(newPath))
                {
                    AssetDatabase.DeleteAsset(newPath);
                }

                AssetDatabase.MoveAsset(fromPath, toPath);
                AssetDatabase.RenameAsset(toPath, $"{newName}.asset");

                UnityEngine.Object asset = AssetDatabase.LoadMainAssetAtPath(newPath);
                element.GeneratedSpriteAsset = asset as TMP_SpriteAsset;

                Selection.activeObject = target;
            }

            serializedObject.Update();
        }

        private static void UpdateName(string[] keyboardKeys, params UnityEngine.Object[] objects)
        {
            foreach (UnityEngine.Object obj in objects)
            {
                if (obj is Texture2D)
                {
                    SpriteDataProviderFactories factory = new SpriteDataProviderFactories();
                    factory.Init();
                    ISpriteEditorDataProvider dataProvider = factory.GetSpriteEditorDataProviderFromObject(obj);
                    dataProvider.InitSpriteEditorDataProvider();

                    SetSpriteName(dataProvider, keyboardKeys);

                    dataProvider.Apply();

                    var assetImporter = dataProvider.targetObject as AssetImporter;
                    assetImporter.SaveAndReimport();
                }
            }
        }

        private static void SetSpriteName(ISpriteEditorDataProvider dataProvider, string[] keyboardKeys)
        {
            SpriteRect[] spriteRects = dataProvider.GetSpriteRects();
            for (int i = 0; i < spriteRects.Length; ++i)
            {
                if (i >= keyboardKeys.Length)
                {
                    spriteRects[i].name = i.ToString();
                    Debug.Log(spriteRects[i].name);
                    continue;
                }

                spriteRects[i].name = keyboardKeys[i];
                Debug.Log(spriteRects[i].name);
            }
            dataProvider.SetSpriteRects(spriteRects);

            ISpriteNameFileIdDataProvider nameFileIdDataProvider = dataProvider.GetDataProvider<ISpriteNameFileIdDataProvider>();
            IEnumerable<SpriteNameFileIdPair> pairs = nameFileIdDataProvider.GetNameFileIdPairs();
            foreach (SpriteNameFileIdPair pair in pairs)
            {
                SpriteRect spriteRect = Array.Find(spriteRects, x => x.spriteID == pair.GetFileGUID());
                pair.name = spriteRect.name;
            }

            nameFileIdDataProvider.SetNameFileIdPairs(pairs);
        }
    }
}
