using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.TextCore;
using UnityEngine.U2D;
using Object = UnityEngine.Object;

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

                CreateSpriteAsset();
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

        private static void UpdateName(IReadOnlyList<string> keyboardKeys, params UnityEngine.Object[] objects)
        {
            foreach (UnityEngine.Object obj in objects)
            {
                if (obj is Texture2D)
                {
                    SpriteDataProviderFactories factory = new SpriteDataProviderFactories();
                    factory.Init();
                    ISpriteEditorDataProvider dataProvider = factory.GetSpriteEditorDataProviderFromObject(obj);
                    dataProvider.InitSpriteEditorDataProvider();

                    SetSpriteName(dataProvider, keyboardKeys.ToArray());

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

        static void CreateSpriteAsset()
        {
            Object[] targets = Selection.objects;

            if (targets == null)
            {
                Debug.LogWarning("A Sprite Texture must first be selected in order to create a Sprite Asset.");
                return;
            }

            // Make sure TMP Essential Resources have been imported in the user project.
            if (TMP_Settings.instance == null)
            {
                Debug.Log("Unable to create sprite asset. Please import the TMP Essential Resources.");

                // Show Window to Import TMP Essential Resources
                return;
            }

            for (int i = 0; i < targets.Length; i++)
            {
                Object target = targets[i];

                // Make sure the selection is a font file
                if (target == null || target.GetType() != typeof(Texture2D))
                {
                    Debug.LogWarning("Selected Object [" + target.name + "] is not a Sprite Texture. A Sprite Texture must be selected in order to create a Sprite Asset.", target);
                    continue;
                }

                CreateSpriteAssetFromSelectedObject(target);
            }
        }

        static void CreateSpriteAssetFromSelectedObject(Object target)
        {
            // Get the path to the selected asset.
            string filePathWithName = AssetDatabase.GetAssetPath(target);
            string fileNameWithExtension = Path.GetFileName(filePathWithName);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePathWithName);
            string filePath = filePathWithName.Replace(fileNameWithExtension, "");
            string uniquePath = AssetDatabase.GenerateUniqueAssetPath(filePath + fileNameWithoutExtension + ".asset");

            // Create new Sprite Asset
            TMP_SpriteAsset spriteAsset = ScriptableObject.CreateInstance<TMP_SpriteAsset>();
            AssetDatabase.CreateAsset(spriteAsset, uniquePath);

            ModifyTMPAssetVersion(spriteAsset, "1.1.0");

            // Compute the hash code for the sprite asset.
            spriteAsset.hashCode = TMP_TextUtilities.GetSimpleHashCode(spriteAsset.name);

            List<TMP_SpriteGlyph> spriteGlyphTable = new List<TMP_SpriteGlyph>();
            List<TMP_SpriteCharacter> spriteCharacterTable = new List<TMP_SpriteCharacter>();

            if (target.GetType() == typeof(Texture2D))
            {
                Texture2D sourceTex = target as Texture2D;

                // Assign new Sprite Sheet texture to the Sprite Asset.
                spriteAsset.spriteSheet = sourceTex;

                PopulateSpriteTables(sourceTex, ref spriteCharacterTable, ref spriteGlyphTable);

                ModifyTMPAssetSprites(spriteAsset, spriteGlyphTable, spriteCharacterTable);

                // Add new default material for sprite asset.
                AddDefaultMaterial(spriteAsset);
            }
            else if (target.GetType() == typeof(SpriteAtlas))
            {
                //SpriteAtlas spriteAtlas = target as SpriteAtlas;

                //PopulateSpriteTables(spriteAtlas, ref spriteCharacterTable, ref spriteGlyphTable);

                //spriteAsset.spriteCharacterTable = spriteCharacterTable;
                //spriteAsset.spriteGlyphTable = spriteGlyphTable;

                //spriteAsset.spriteSheet = spriteGlyphTable[0].sprite.texture;

                //// Add new default material for sprite asset.
                //AddDefaultMaterial(spriteAsset);
            }

            // Update Lookup tables.
            spriteAsset.UpdateLookupTables();

            // Get the Sprites contained in the Sprite Sheet
            EditorUtility.SetDirty(spriteAsset);

            //spriteAsset.sprites = sprites;

            // Set source texture back to Not Readable.
            //texImporter.isReadable = false;

            AssetDatabase.SaveAssets();

            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(spriteAsset));  // Re-import font asset to get the new updated version.

            //AssetDatabase.Refresh();
        }

        static void PopulateSpriteTables(Texture source, ref List<TMP_SpriteCharacter> spriteCharacterTable, ref List<TMP_SpriteGlyph> spriteGlyphTable)
        {
            //Debug.Log("Creating new Sprite Asset.");

            string filePath = AssetDatabase.GetAssetPath(source);

            // Get all the Sprites sorted by Index
            Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(filePath).Select(x => x as Sprite).Where(x => x != null).OrderByDescending(x => x.rect.y).ThenBy(x => x.rect.x).ToArray();

            for (int i = 0; i < sprites.Length; i++)
            {
                Sprite sprite = sprites[i];

                TMP_SpriteGlyph spriteGlyph = new TMP_SpriteGlyph();
                spriteGlyph.index = (uint)i;
                spriteGlyph.metrics = new GlyphMetrics(sprite.rect.width, sprite.rect.height, -sprite.pivot.x, sprite.rect.height - sprite.pivot.y, sprite.rect.width);
                spriteGlyph.glyphRect = new GlyphRect(sprite.rect);
                spriteGlyph.scale = 1.0f;
                spriteGlyph.sprite = sprite;

                spriteGlyphTable.Add(spriteGlyph);

                TMP_SpriteCharacter spriteCharacter = new TMP_SpriteCharacter(0xFFFE, spriteGlyph);

                // Special handling for .notdef sprite name.
                string fileNameToLowerInvariant = sprite.name.ToLowerInvariant();
                if (fileNameToLowerInvariant == ".notdef" || fileNameToLowerInvariant == "notdef")
                {
                    spriteCharacter.unicode = 0;
                    spriteCharacter.name = fileNameToLowerInvariant;
                }
                else
                {
                    if (!string.IsNullOrEmpty(sprite.name) && sprite.name.Length > 2 && sprite.name[0] == '0' && (sprite.name[1] == 'x' || sprite.name[1] == 'X'))
                    {
                        spriteCharacter.unicode = (uint)TMP_TextUtilities.StringHexToInt(sprite.name.Remove(0, 2));
                    }
                    spriteCharacter.name = sprite.name;
                }

                spriteCharacter.scale = 1.0f;

                spriteCharacterTable.Add(spriteCharacter);
            }
        }

        static void AddDefaultMaterial(TMP_SpriteAsset spriteAsset)
        {
            Shader shader = Shader.Find("TextMeshPro/Sprite");
            Material material = new Material(shader);
            material.SetTexture(ShaderUtilities.ID_MainTex, spriteAsset.spriteSheet);

            spriteAsset.material = material;
            material.name = spriteAsset.name + " Material";
            AssetDatabase.AddObjectToAsset(material, spriteAsset);
        }

        private static void ModifyTMPAssetVersion(TMP_SpriteAsset targetAsset, string v)
        {
            // 1. Get the Type of the asset instance.
            Type assetType = targetAsset.GetType();
            PropertyInfo versionProperty = assetType.GetProperty("version", BindingFlags.Instance | BindingFlags.NonPublic);

            // 2. Verify that the property was found.
            if (versionProperty == null)
            {
                Debug.LogError("Failed to find the 'version' property using reflection.");
                return;
            }

            // 3. Set the new value using the SetValue method.
            // The first argument is the object instance to modify (targetAsset).
            // The second argument is the new value you want to assign.
            string oldVersion = versionProperty.GetValue(targetAsset) as string;
            string newVersion = v; // Define your new version string here.

            versionProperty.SetValue(targetAsset, newVersion, null);
        }

        private static void ModifyTMPAssetSprites(TMP_SpriteAsset targetAsset, List<TMP_SpriteGlyph> spriteGlyphTable, List<TMP_SpriteCharacter> spriteCharacterTable)
        {
            Type assetType = targetAsset.GetType();
            FieldInfo versionProperty = assetType.GetField("m_SpriteCharacterTable", BindingFlags.Instance | BindingFlags.NonPublic);
            FieldInfo versionProperty2 = assetType.GetField("m_GlyphTable", BindingFlags.Instance | BindingFlags.NonPublic);

            if (versionProperty == null)
            {
                Debug.LogError("Failed to find the 'm_SpriteCharacterTable' property using reflection.");
                return;
            }

            if (versionProperty2 == null)
            {
                Debug.LogError("Failed to find the 'm_GlyphTable' property using reflection.");
                return;
            }

            versionProperty.SetValue(targetAsset, spriteCharacterTable);
            versionProperty2.SetValue(targetAsset, spriteGlyphTable);
        }
    }
}
