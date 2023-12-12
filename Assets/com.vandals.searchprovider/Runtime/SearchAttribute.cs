using System;
using UnityEngine;

public class SearchAttribute : PropertyAttribute
{
    public Type searchObjectType;
    public SearchAttribute(Type searchObjectType)
    {
        this.searchObjectType = searchObjectType;
    }
}
