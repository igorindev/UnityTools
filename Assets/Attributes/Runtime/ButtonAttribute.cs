using System;
using System.Reflection;
using UnityEngine;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
public class ButtonAttribute : PropertyAttribute 
{
    readonly Delegate buttonCallback;
    readonly bool validationExists = false;
    readonly Delegate validationDelegate;

    /// <summary>
    /// Runs the callback method.
    /// </summary>
    /// <returns>The return value of the callback method.</returns>
    public object Callback()
    {
#if UNITY_EDITOR
        return buttonCallback?.DynamicInvoke();
#else
        return null;
#endif
    }

    /// <summary>
    /// Checks if the validation method exists and returns the value from the method if it does.
    /// </summary>
    /// <returns>The value of the validation method if it exists, otherwise it returns true.</returns>
    public bool Validate()
    {
#if UNITY_EDITOR
        if (validationExists)
        {
            // If the validation method exists use that.
            return (bool)validationDelegate.DynamicInvoke();
        }
        else
        {
            // If the validation method does not exist return true.
            return true;
        }
#else
         return true;
#endif

    }

    /// <summary>
    /// Creates a button in the inspector on a bool field.
    /// </summary>
    /// <param name="callbackType">The Type of class the callback method is in.</param>
    /// <param name="callbackMethodName">The name of the callback method. Must be a static method that returns an object.</param>
    /// <param name="validationType">The Type of class the validation method is in. If it is null the type defaults to callbackType.</param>
    /// <param name="validationMethodName">The name of the validation method. Must be a static method that returns a bool. If it is null the button is always enabled.</param>

}