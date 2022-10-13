using UnityEditor;

public static class CompilationOptions
{
    //Auto Refresh

    //kAutoRefresh has two posible values
    //0 = Auto Refresh Disabled
    //1 = Auto Refresh Enabled

    public const string BASE_PATH = "Compilation/";
    public const string PATH = "Auto Refresh";

    //This is called when you click on the 'Editor Config/Auto Refresh' and toggles its value
    [MenuItem(BASE_PATH + PATH)]
    public static void AutoRefreshToggle()
    {
        var status = EditorPrefs.GetInt("kAutoRefresh");
        if (status == 1)
            EditorPrefs.SetInt("kAutoRefresh", 0);
        else
            EditorPrefs.SetInt("kAutoRefresh", 1);
    }

    //This is called before 'Editor Config/Auto Refresh' is shown to check the current value
    //of kAutoRefresh and update the checkmark
    [MenuItem(BASE_PATH + PATH, true)]
    public static bool AutoRefreshToggleValidation()
    {
        var status = EditorPrefs.GetInt("kAutoRefresh");
        if (status == 1)
            Menu.SetChecked(BASE_PATH + PATH, true);
        else
            Menu.SetChecked(BASE_PATH + PATH, false);
        return true;
    }

    //Script Compilation During Play
    //ScriptCompilationDuringPlay has three posible values
    //0 = Recompile And Continue Playing
    //1 = Recompile After Finished Playing
    //2 = Stop Playing And Recompile

    //The following methods assing the three possible values to ScriptCompilationDuringPlay
    //depending on the option you selected
    [MenuItem(BASE_PATH + "Script Compilation During Play/Recompile And Continue Playing")]
    public static void ScriptCompilationToggleOption0()
    {
        EditorPrefs.SetInt("ScriptCompilationDuringPlay", 0);
    }
    [MenuItem(BASE_PATH + "Script Compilation During Play/Recompile After Finished Playing")]
    public static void ScriptCompilationToggleOption1()
    {
        EditorPrefs.SetInt("ScriptCompilationDuringPlay", 1);
    }
    [MenuItem(BASE_PATH + "Script Compilation During Play/Stop Playing And Recompile")]
    public static void ScriptCompilationToggleOption2()
    {
        EditorPrefs.SetInt("ScriptCompilationDuringPlay", 2);
    }

    //This is called before 'Editor Config/Script Compilation During Play/Recompile And Continue Playing'
    //is shown to check for the current value of ScriptCompilationDuringPlay and update the checkmark
    [MenuItem(BASE_PATH + "Script Compilation During Play/Recompile And Continue Playing", true)]
    public static bool ScriptCompilationValidation()
    {
        //Here, we uncheck all options before we show them
        Menu.SetChecked(BASE_PATH + "Script Compilation During Play/Recompile And Continue Playing", false);
        Menu.SetChecked(BASE_PATH + "Script Compilation During Play/Recompile After Finished Playing", false);
        Menu.SetChecked(BASE_PATH + "Script Compilation During Play/Stop Playing And Recompile", false);

        var status = EditorPrefs.GetInt("ScriptCompilationDuringPlay");

        //Here, we put the checkmark on the current value of ScriptCompilationDuringPlay
        switch (status)
        {
            case 0:
                Menu.SetChecked(BASE_PATH + "Script Compilation During Play/Recompile And Continue Playing", true);
                break;
            case 1:
                Menu.SetChecked(BASE_PATH + "Script Compilation During Play/Recompile After Finished Playing", true);
                break;
            case 2:
                Menu.SetChecked(BASE_PATH + "Script Compilation During Play/Stop Playing And Recompile", true);
                break;
        }
        return true;
    }
}