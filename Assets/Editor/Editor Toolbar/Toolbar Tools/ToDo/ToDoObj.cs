using UnityEngine;

[CreateAssetMenu(fileName = "To Do Obj", menuName = "ScriptableObjects/Toolbar/To Do/To Do")]
public class ToDoObj : ToolbarTool
{
    public override void Perform()
    {
        ToDoEditorWindow.ShowWindow();
    }    
}
