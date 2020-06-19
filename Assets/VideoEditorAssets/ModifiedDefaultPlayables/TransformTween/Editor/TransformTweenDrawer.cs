using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(TransformTweenBehaviour))]
public class TransformTweenDrawer : PropertyDrawer
{
    GUIContent m_TweenPositionContent = new GUIContent("Tween Position", "This should be true if the transformToMove to change position.  This causes recalulations each frame which are more CPU intensive.");
    GUIContent m_TweenRotationContent = new GUIContent("Tween Rotation", "This should be true if the transformToMove to change rotation.");
    GUIContent m_TweenTypeContent = new GUIContent("Tween Type", "Linear - the transform moves the same amount each frame (assuming static start and end locations).\n"
        + "Deceleration - the transform moves slower the closer to the end location it is.\n"
        + "Harmonic - the transform moves faster in the middle of its tween.\n"
        + "Custom - uses the customStartingSpeed and customEndingSpeed to create a curve for the desired tween.");
    GUIContent m_CustomCurveContent = new GUIContent("Custom Curve", "This should be a normalised curve (between 0,0 and 1,1) that represents how the tweening object accelerates at different points along the clip.");

    public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
    {
        int fieldCount = property.FindPropertyRelative ("tweenType").enumValueIndex == (int)TransformTweenBehaviour.TweenType.Custom ? 5 : 3;
        return fieldCount * (EditorGUIUtility.singleLineHeight);
    }

    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty tweenPositionProp = property.FindPropertyRelative ("tweenPosition");
        SerializedProperty tweenRotationProp = property.FindPropertyRelative("tweenRotation");
        SerializedProperty tweenTypeProp = property.FindPropertyRelative ("tweenType");
        
        Rect singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField (singleFieldRect, tweenPositionProp, m_TweenPositionContent);
        
        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField (singleFieldRect, tweenRotationProp, m_TweenRotationContent);

        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(singleFieldRect, tweenTypeProp, m_TweenTypeContent);

        if (tweenTypeProp.enumValueIndex == (int)TransformTweenBehaviour.TweenType.Custom)
        {
            SerializedProperty customCurveProp = property.FindPropertyRelative ("customCurve");
            
            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField (singleFieldRect, customCurveProp, m_CustomCurveContent);
        }
    }
}
