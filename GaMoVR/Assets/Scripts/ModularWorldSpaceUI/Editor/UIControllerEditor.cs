using UnityEditor;
using UnityEngine;

namespace ModularWorldSpaceUI.Editor
{
    [CustomEditor(typeof(UIController))]
    public class UIControllerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Separator();
            UIController uiController = (UIController)target;

            uiController.attachmentTypeEnum =
                (UIController.AttachmentTypeEnum)EditorGUILayout.EnumPopup("Attachment Type",
                    uiController.attachmentTypeEnum);
            EditorGUILayout.Space();
            if (uiController.attachmentTypeEnum is UIController.AttachmentTypeEnum.ToPlayer)
            {
                EditorGUILayout.LabelField("Player attachment options", EditorStyles.boldLabel);
                EditorGUILayout.Separator();
                uiController.attachmentPointEnum =
                    (UIController.AttachmentPointEnum)EditorGUILayout.EnumPopup("Attachment Point",
                        uiController.attachmentPointEnum);
                uiController.attachmentOffset =
                    EditorGUILayout.Vector3Field("Attachment Offset", uiController.attachmentOffset);
                uiController.hasMovementTolerance =
                    EditorGUILayout.Toggle("Has movement tolerance", uiController.hasMovementTolerance);
                if (uiController.hasMovementTolerance)
                {
                    EditorGUILayout.Space();
                    uiController.toleranceCollider = EditorGUILayout.ObjectField("Tolerance Collider",
                        uiController.toleranceCollider, typeof(Collider), true) as Collider;
                }
            }
            else
            {
                EditorGUILayout.LabelField("World spawn options", EditorStyles.boldLabel);
                uiController.spawnPoint =
                    EditorGUILayout.ObjectField("Spawn Point", uiController.spawnPoint, typeof(Transform), true) as
                        Transform;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Rotation options", EditorStyles.boldLabel);
            uiController.spawnRotationTypeEnum =
                (UIController.SpawnRotationTypeEnum)EditorGUILayout.EnumPopup("Spawn rotation",
                    uiController.spawnRotationTypeEnum);
            if (uiController.spawnRotationTypeEnum is UIController.SpawnRotationTypeEnum.Absolute)
            {
                EditorGUILayout.Space();
                uiController.spawnRotation = EditorGUILayout.Vector3Field("Spawn rotation", uiController.spawnRotation);
            }

            EditorGUILayout.Space();
            uiController.facePlayer = EditorGUILayout.Toggle("Face player", uiController.facePlayer);
            uiController.rotationOffset = EditorGUILayout.Vector3Field("Rotation Offset", uiController.rotationOffset);
            EditorGUILayout.Space();

            uiController.uiElementReferenceOrPrefab = EditorGUILayout.ObjectField("Ui Element",
                uiController.uiElementReferenceOrPrefab, typeof(GameObject), true) as GameObject;

            if (uiController.uiElementReferenceOrPrefab)
            {
                if (PrefabUtility.IsPartOfPrefabAsset(uiController.uiElementReferenceOrPrefab))
                {
                    uiController.uiObjectPrefab = uiController.uiElementReferenceOrPrefab;
                }
                else
                {
                    uiController.referenceToUIObject = uiController.uiElementReferenceOrPrefab;
                }
            }

            uiController.deactivateUIOnStartup =
                EditorGUILayout.Toggle("Deactivate UI on Startup",uiController.deactivateUIOnStartup);

            uiController.performInteractorSelection = EditorGUILayout.Toggle("Perform Interactor Selection",uiController.performInteractorSelection);
            
            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}