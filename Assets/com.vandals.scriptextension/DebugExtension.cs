using System.Diagnostics;
using UnityEngine;

public struct DebugExtension
{
    public static void Log(string message = "", Object context = null)
    {
        var caller = new StackTrace().GetFrame(1).GetMethod().Name;
        var go = context != null ? context.name + ":" : "";
        var hue = (caller.GetHashCode() / (float)int.MaxValue) * 0.5f + 0.5f;
        var callerColor = Color.HSVToRGB(hue, 0.6f, 0.8f, false).ToHex();
        UnityEngine.Debug.Log($"<b>F:{ Time.frameCount } {go} <color={callerColor}>{caller}()</color></b> {message}", context);
    }
}