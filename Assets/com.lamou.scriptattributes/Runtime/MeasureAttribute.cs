using System;
using UnityEngine;

namespace Attribute.Measure
{
    public enum MeasureType
    {
        Centimeter,
        Meter,

        Hour,
        Minutes,
        Seconds,
        Milliseconds,

        Acceleration,
        Speed,

        Custom
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public class MeasureAttribute : PropertyAttribute
    {
        public string CustomType { get; set; }

        public readonly MeasureType type;
        public MeasureAttribute(MeasureType type) { this.type = type; }
    }
}