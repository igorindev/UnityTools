using UnityEditor;

public static class NoDomainPlayMode
{
    public static int count = 0;
    public const string BASE_PATH = "Compilation/";
    public const string PATH = "Fast PlayMode";
    
    [MenuItem(BASE_PATH + PATH, false, 1200)]
    public static void AutoRefreshToggle()
    {
        var status = EditorSettings.enterPlayModeOptionsEnabled;

        if (status)
        {
            EditorSettings.enterPlayModeOptionsEnabled = false;
            EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.None;
        }
        else
        {
            EditorSettings.enterPlayModeOptionsEnabled = true;
            EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.DisableDomainReload | EnterPlayModeOptions.DisableSceneReload;
        }
    }

    [MenuItem(BASE_PATH + PATH, true, 1200)]
    public static bool AutoRefreshToggleValidation()
    {
        var status = EditorSettings.enterPlayModeOptionsEnabled;
        if (status)
            Menu.SetChecked(BASE_PATH + PATH, true);
        else
            Menu.SetChecked(BASE_PATH + PATH, false);
        return true;
    }

    [MenuItem(BASE_PATH + "Reload Domain", false, 1200)]
    public static void ReloadDomain()
    {
        EditorUtility.RequestScriptReload();
    }
}
