using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

public class SpriteName
{
    static string[] keyboardKeys = {
                "0", "1","2", "3", "4", "5", "6", "7", "8", "9",
                "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "ç",
                "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "-", "_", "=", "+", "[", "{", "]", "}", "|", "\\", ";", ":", "\"", "\"", ",", "<", ".", ">", "/", "?",
                "spacebar",
                "backspace",
                "tab",
                "enter",
                "esc",
                "delete"
            };

    [MenuItem("Assets/Update Sprite Name")]
    static void UpdateName()
    {
        foreach (var obj in Selection.objects)
        {
            if (obj is Texture2D)
            {
                var factory = new SpriteDataProviderFactories();
                factory.Init();
                var dataProvider = factory.GetSpriteEditorDataProviderFromObject(obj);
                dataProvider.InitSpriteEditorDataProvider();

                SetSpriteName(dataProvider);

                dataProvider.Apply();

                var assetImporter = dataProvider.targetObject as AssetImporter;
                assetImporter.SaveAndReimport();
            }
        }
    }

    static void SetSpriteName(ISpriteEditorDataProvider dataProvider)
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
