using UnityEditor;

namespace ModularWorldSpaceUI.Editor
{
    [CustomEditor(typeof(UIActionActivation))]
    public class UIActionActivationEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            base.OnInspectorGUI();
/*
            UIActionActivation uiActionActivation = (UIActionActivation)target;
            uiActionActivation.ToggleOnEnterAndLeave = EditorGUILayout.Toggle("Toggle and enter and leave",
                uiActionActivation.ToggleOnEnterAndLeave);
            if (uiActionActivation.ToggleOnEnterAndLeave)
            {
                uiActionActivation.ToggleTrigger =
                    EditorGUILayout.ObjectField("Toggle trigger", uiActionActivation.ToggleTrigger,typeof(UITrigger),true) as UITrigger;
            }
            else
            {
                uiActionActivation.EnterTrigger =EditorGUILayout.ObjectField("Enter trigger", uiActionActivation.ToggleTrigger,typeof(UITrigger),true) as UITrigger;
                uiActionActivation.LeaveTrigger =EditorGUILayout.ObjectField("Leave trigger", uiActionActivation.LeaveTrigger,typeof(UITrigger),true) as UITrigger;
            }*/
        
        }
    }
}
