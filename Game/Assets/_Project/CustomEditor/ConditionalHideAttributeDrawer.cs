using UnityEngine;
using UnityEditor;


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ConditionalHideAttribute))]
public class ConditionalHidePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;
        bool enabled = GetConditionalHideAttributeResult(condHAtt, property);

        bool wasEnabled = GUI.enabled;

        AttributeShowMode currentShowMode = enabled ? condHAtt.showModeIfTrue : condHAtt.showModeIfFalse;

        switch (currentShowMode)
        {
            case AttributeShowMode.EDIT:
                GUI.enabled = true;
                EditorGUI.PropertyField(position, property, label, true);
                break;
            case AttributeShowMode.READ_ONLY:
                GUI.enabled = false;
                EditorGUI.PropertyField(position, property, label, true);
                break;
            case AttributeShowMode.HIDE:
                GUI.enabled = true;
                break;
            default:
                break;
        }

        GUI.enabled = wasEnabled;
    }

    //public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    //{
    //    ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;
    //    bool enabled = GetConditionalHideAttributeResult(condHAtt, property);

    //    if (condHAtt.showModeIfFalse != AttributeShowMode.HIDE || enabled)
    //    {
    //        return EditorGUI.GetPropertyHeight(property, label);
    //    }
    //    else
    //    {
    //        return -EditorGUIUtility.standardVerticalSpacing;
    //    }
    //}

    private bool GetConditionalHideAttributeResult(ConditionalHideAttribute condHAtt, SerializedProperty property)
    {
        bool enabled = true;
        string propertyPath = property.propertyPath; //returns the property path of the property we want to apply the attribute to
        string conditionPath = propertyPath.Replace(property.name, condHAtt.ConditionalSourceField); //changes the path to the conditionalsource property path
        SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);

        if (sourcePropertyValue != null)
        {
            enabled = sourcePropertyValue.boolValue;
        }
        else
        {
            Debug.LogWarning("Attempting to use a ConditionalHideAttribute but no matching SourcePropertyValue found in object: " + condHAtt.ConditionalSourceField);
        }

        return enabled;
    }
}
#endif