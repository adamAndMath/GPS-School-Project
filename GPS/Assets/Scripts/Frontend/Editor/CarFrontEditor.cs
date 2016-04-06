using UnityEditor;
using UnityEngine;

namespace Frontend
{
    [CustomEditor(typeof(CarFront))]
    public class CarFrontEditor : Editor
    {
        SerializedProperty propID;
        SerializedProperty propAcceleration;
        SerializedProperty propDeceleration;
        SerializedProperty propNiceDeceleration;

        void OnEnable()
        {
            propID = serializedObject.FindProperty("identifier");
            propAcceleration = serializedObject.FindProperty("acceleration");
            propDeceleration = serializedObject.FindProperty("deceleration");
            propNiceDeceleration = serializedObject.FindProperty("niceDeceleration");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(propID);
            EditorGUILayout.PropertyField(propAcceleration);
            EditorGUILayout.PropertyField(propDeceleration);
            EditorGUILayout.PropertyField(propNiceDeceleration);

            serializedObject.ApplyModifiedProperties();

            var carFront = (CarFront) serializedObject.targetObject;

            if (carFront.car == null) return;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Complete", carFront.car.Complete ? "True" : "False");
        }
    }
}