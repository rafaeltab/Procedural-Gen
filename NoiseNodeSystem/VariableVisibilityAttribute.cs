using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class VariableVisibilityAttribute : Attribute {

    public enum VisibilityType
    {
        Input,
        Mask,
        Property,
        InspectOnly
    }

    public VisibilityType visibilityType = VisibilityType.Input;

    public VariableVisibilityAttribute(VisibilityType visibilityType)
    {
        this.visibilityType = visibilityType;
    }
}

