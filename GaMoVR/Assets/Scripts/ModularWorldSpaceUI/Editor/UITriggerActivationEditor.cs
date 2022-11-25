using UnityEditor;
using UnityEngine;

namespace ModularWorldSpaceUI.Editor
{
    [CustomEditor(typeof(UITriggerActivation))]
    public class UITriggerActivationEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            UITriggerActivation uiTriggerActivation = (UITriggerActivation)target;
            uiTriggerActivation.toggleOnEnterAndLeave = EditorGUILayout.Toggle("Toggle and enter and leave",
                uiTriggerActivation.toggleOnEnterAndLeave);
            if (uiTriggerActivation.toggleOnEnterAndLeave)
            {
                uiTriggerActivation.toggleTrigger =
                    EditorGUILayout.ObjectField("Toggle trigger", uiTriggerActivation.toggleTrigger, typeof(UITrigger),
                        true) as UITrigger;
            }
            else
            {
                uiTriggerActivation.enterTrigger = EditorGUILayout.ObjectField("Enter trigger",
                    uiTriggerActivation.enterTrigger, typeof(UITrigger), true) as UITrigger;
                uiTriggerActivation.leaveTrigger = EditorGUILayout.ObjectField("Leave trigger",
                    uiTriggerActivation.leaveTrigger, typeof(UITrigger), true) as UITrigger;
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}