using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Sprite Renderer/Update Color")]
public class UpdateSpriteRendererColorActionSO : ActionSO
{
    public Color color;

    public override void Execute(GameObject gameObject)
    {
        gameObject.GetComponentInParent<SpriteRenderer>().color = color;
    }
}