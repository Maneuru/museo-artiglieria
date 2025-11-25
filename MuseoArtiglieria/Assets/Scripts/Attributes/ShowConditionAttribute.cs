using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomPropertyDrawer(typeof(ShowConditionAttribute))]
public class ShowConditionAttributeDrawer : PropertyDrawer
{
    private const System.Reflection.BindingFlags _bindingFlags =
        System.Reflection.BindingFlags.NonPublic
        | System.Reflection.BindingFlags.Public;
    private float _propertyHeight;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => _propertyHeight;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        bool isConditionMet = GetConditionValue(property);
        if (isConditionMet)
        {
            EditorGUI.indentLevel++;
            EditorGUI.PropertyField(position, property, label, true);
            _propertyHeight = base.GetPropertyHeight(property, label);
            EditorGUI.indentLevel--;
        }
        else
        {
            _propertyHeight = 0f;
        }
    }

    private bool GetConditionValue(SerializedProperty property)
    {
        string showConditionName = (attribute as ShowConditionAttribute).showConditionName;
        bool isMethod = (attribute as ShowConditionAttribute).isMethod;

        if (isMethod)
        {
            var method = property.serializedObject.targetObject.GetType().GetMethod(showConditionName, _bindingFlags);
            if (method.ReturnType != typeof(bool))
            {
                Debug.LogError($"ShowCondition method {showConditionName} must return a boolean value.");
                return false;
            }

            return (bool)method.Invoke(property.serializedObject.targetObject, null);
        }
        else
        {
            var field = property.serializedObject.FindProperty(showConditionName);
            if (field.propertyType != SerializedPropertyType.Boolean)
            {
                Debug.LogError($"ShowCondition field {showConditionName} must be of type boolean.");
                return false;
            }

            return field.boolValue;
        }
    }
}
#endif

public class ShowConditionAttribute : PropertyAttribute
{
    public string showConditionName { get; private set; }
    public bool isMethod { get; private set; }

    public ShowConditionAttribute(string showConditionName, bool isMethod = false)
    {
        this.showConditionName = showConditionName;
        this.isMethod = isMethod;
    }
}
