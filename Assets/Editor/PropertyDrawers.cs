using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(FloorEvent))]
public class FloorEventPropertyDrawer : PropertyDrawer
{
    int line = 0;
    float linePosition = 0;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        line = 1;
        linePosition = (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * line;
        float propertyIndent = 0;

        EditorGUI.BeginProperty(position, label, property);

        // Get properties by exactly passing the names of the interval's attributes
        SerializedProperty useCommonEventProp = property.FindPropertyRelative("useCommonEvent");
        SerializedProperty commonEventProp = property.FindPropertyRelative("commonEvent");
        SerializedProperty typeOfEventProp = property.FindPropertyRelative("typeOfEvent");
        SerializedProperty stairParamProp = property.FindPropertyRelative("stairParams");

        Rect rectPosition = new Rect(position.x+propertyIndent,
                                position.y,
                                position.width,
                                EditorGUIUtility.singleLineHeight);
        //Rect line2 = new Rect(position.x,
        //                                position.y + (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing),
        //                                position.width,
        //                                EditorGUIUtility.singleLineHeight);

        //Rect line3 = new Rect(position.x,
        //                                position.y + (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2,
        //                                position.width,
        //                                EditorGUIUtility.singleLineHeight);

        void ResetRectPosition()
        {
            SetRectPosition(position.x + propertyIndent,
                position.y + ((EditorGUIUtility.singleLineHeight * (line - 1)) + (EditorGUIUtility.standardVerticalSpacing * (line - 1))),
                position.width,
                EditorGUIUtility.singleLineHeight);
        }
        void SetRectPosition(float x, float y, float width, float height)
        {
            rectPosition = new Rect(x,
                                y,
                                width,        
                                height);            
        }
        void UpdateLinePosition(float additionalHeight)
        {
            linePosition += additionalHeight;
        }

        void IncrementLine()
        {
            line++;
        }

        EditorGUI.LabelField(rectPosition, "Use Common Event");
        SetRectPosition(rectPosition.x+130, rectPosition.y, 20, rectPosition.height);
        EditorGUI.PropertyField(rectPosition, useCommonEventProp, GUIContent.none);
        SetRectPosition(rectPosition.x + 20, rectPosition.y, 160, rectPosition.height);
        if (useCommonEventProp.boolValue)
        {
            EditorGUI.PropertyField(rectPosition, commonEventProp, GUIContent.none);
        }
        else
        {
            // if not using a common event, the necessary properties of the event inspector are drawn here
            EditorGUI.LabelField(rectPosition, "Event Type");
            SetRectPosition(rectPosition.x + 80, rectPosition.y, 160, rectPosition.height);
            EditorGUI.PropertyField(rectPosition, typeOfEventProp, GUIContent.none);

            if (typeOfEventProp.enumNames[typeOfEventProp.enumValueIndex] != "Blank")
            {
                IncrementLine();
                UpdateLinePosition((EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing));
                ResetRectPosition();
            }

            switch (typeOfEventProp.enumNames[typeOfEventProp.enumValueIndex])
            {
                default:
                    break;
                case "Move":
                    EditorGUI.LabelField(rectPosition, "Move Parameters", EditorStyles.boldLabel);
                    break;
                case "Wait":
                    EditorGUI.LabelField(rectPosition, "Wait Parameters", EditorStyles.boldLabel);
                    break;
                case "Stairs":
                    EditorGUI.LabelField(rectPosition, "Stairs Parameters", EditorStyles.boldLabel);
                    break;
                case "Ladder":
                    EditorGUI.LabelField(rectPosition, "Ladder Parameters", EditorStyles.boldLabel);
                    break;
                case "Rope":
                    EditorGUI.LabelField(rectPosition, "Rope Parameters", EditorStyles.boldLabel);
                    break;
                case "Warp":
                    EditorGUI.LabelField(rectPosition, "Warp Parameters", EditorStyles.boldLabel);
                    break;
                case "Door":
                    EditorGUI.LabelField(rectPosition, "Door Parameters", EditorStyles.boldLabel);
                    break;
                case "DialogScene":
                    EditorGUI.LabelField(rectPosition, "Dialog Scene Parameters", EditorStyles.boldLabel);
                    break;
                case "Battle":
                    EditorGUI.LabelField(rectPosition, "Battle Parameters", EditorStyles.boldLabel);
                    break;
                case "Item":
                    EditorGUI.LabelField(rectPosition, "Item Parameters", EditorStyles.boldLabel);
                    break;
                case "Conditional":
                    EditorGUI.LabelField(rectPosition, "Conditional Parameters", EditorStyles.boldLabel);
                    break;
                case "SetSwitch":
                    EditorGUI.LabelField(rectPosition, "Set Switch Parameters", EditorStyles.boldLabel);
                    break;
                case "SetVar":
                    EditorGUI.LabelField(rectPosition, "Set Variable Parameters", EditorStyles.boldLabel);
                    break;
            }
        }
        //UpdateLinePosition(EditorGUI.GetPropertyHeight(useCommonEventProp));

        EditorGUI.EndProperty();
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        //return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) + linePosition;
        return linePosition;
    }
}