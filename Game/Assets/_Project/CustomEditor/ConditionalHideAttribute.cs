using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using Unity.VisualScripting;

public enum AttributeShowMode
{
    READ_ONLY,
    HIDE,
    EDIT
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
    AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]

public class ConditionalHideAttribute : PropertyAttribute
{


    //The name of the bool field that will be in control
    public string ConditionalSourceField = "";
    public AttributeShowMode showModeIfTrue = AttributeShowMode.EDIT;
    public AttributeShowMode showModeIfFalse = AttributeShowMode.READ_ONLY;

    public ConditionalHideAttribute(string conditionalSourceField)
    {
        this.ConditionalSourceField = conditionalSourceField;
    }

    public ConditionalHideAttribute(string conditionalSourceField, AttributeShowMode showModeIfTrue, AttributeShowMode showModeIfFalse)
    {
        this.ConditionalSourceField = conditionalSourceField;
        this.showModeIfTrue = showModeIfTrue;
        this.showModeIfFalse = showModeIfFalse;
    }
}

