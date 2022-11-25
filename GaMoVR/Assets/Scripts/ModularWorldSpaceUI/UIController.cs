using UnityEngine;

namespace ModularWorldSpaceUI
{
    public class UIController : MonoBehaviour
    {
        #region Enums

        public enum AttachmentTypeEnum
        {
            ToPlayer,
            ToWorld
        }

        public enum AttachmentPointEnum
        {
            Camera,
            Body
        }

        public enum SpawnRotationTypeEnum
        {
            LookAtPlayer,
            Absolute
        }

        #endregion

        #region Fields

        //only used during runtime, but the editor loses the reference, so it is here;
        public GameObject uiElementReferenceOrPrefab;

        //- attachment Type
        public AttachmentTypeEnum attachmentTypeEnum;

        //- - attachmentType: Player
        public AttachmentPointEnum attachmentPointEnum;
        public Vector3 attachmentOffset;

        public bool hasMovementTolerance;

        //- -   - has movement tolerance: true
        public Collider toleranceCollider;

        //- - attachmentType: World
        public Transform spawnPoint; //uses local position if empty

        //- Spawn Rotation
        public SpawnRotationTypeEnum spawnRotationTypeEnum;

        //- - Spawn Rotation: Absolute:
        public Vector3 spawnRotation;

        //- Face Player
        public bool facePlayer;

        public GameObject referenceToUIObject;

        public GameObject uiObjectPrefab;

        public NonXRPlayerComponents player;

        public Transform attachmentPoint;

        private bool _isActive;

        public bool deactivateUIOnStartup;

        public Vector3 rotationOffset;

        public bool performInteractorSelection;


        private InteractorSelectionTrigger _interactorSelectionTrigger;

        #endregion

        public void ActivateUI(NonXRPlayerComponents player)
        {
            this.player = player;
            //instantiate UI if it is referenced as a prefab
            referenceToUIObject ??= Instantiate(uiObjectPrefab, transform);
            if (attachmentTypeEnum is AttachmentTypeEnum.ToWorld)
            {
                //no spawnpoint was assigned, spawn at the parents position
                if (spawnPoint is null)
                    referenceToUIObject.transform.position = transform.position;
                else
                {
                    referenceToUIObject.transform.position = spawnPoint.transform.position;
                }

                //use hip position as default when facing player for world position
                attachmentPoint = player.HipPosition;
            }
            else if (attachmentTypeEnum is AttachmentTypeEnum.ToPlayer)
            {
                if (attachmentPointEnum is AttachmentPointEnum.Camera)
                {
                    attachmentPoint = player.CameraAttachmentPosition;
                }
                else if (attachmentPointEnum is AttachmentPointEnum.Body)
                {
                    attachmentPoint = player.HipPosition;
                }

                Vector3 offsetWithCorrectRotation = Vector3.zero;
                offsetWithCorrectRotation += attachmentPoint.forward * attachmentOffset.z;
                offsetWithCorrectRotation += attachmentPoint.right * attachmentOffset.x;
                offsetWithCorrectRotation += attachmentPoint.up * attachmentOffset.y;


                if (hasMovementTolerance)
                {
                    Vector3 colliderPivotCenterOffset =
                        toleranceCollider.transform.position - toleranceCollider.bounds.center;

                    offsetWithCorrectRotation -= colliderPivotCenterOffset;


                    toleranceCollider.transform.position = attachmentPoint.position + offsetWithCorrectRotation;
                    toleranceCollider.transform.LookAt(attachmentPoint);
                }


                referenceToUIObject.transform.position = attachmentPoint.position + offsetWithCorrectRotation;
            }

            if (spawnRotationTypeEnum is SpawnRotationTypeEnum.Absolute)
            {
                referenceToUIObject.transform.rotation = Quaternion.Euler(spawnRotation);
            }
            else if (spawnRotationTypeEnum is SpawnRotationTypeEnum.LookAtPlayer)
            {
                FacePlayer();
            }

            if (performInteractorSelection)
            {
                if (_interactorSelectionTrigger is null)
                {
                    _interactorSelectionTrigger = GetComponent<InteractorSelectionTrigger>();
                }
                _interactorSelectionTrigger.SimulateTriggerEnter(LocalPlayerReference.Instance.LocalPlayer.GetComponentInChildren<XRPlayerComponents>());
            }

            referenceToUIObject.SetActive(true);
            _isActive = true;
        }

        public void DeactivateUI()
        {
            if (performInteractorSelection)
            {
                if (_interactorSelectionTrigger is null)
                {
                    _interactorSelectionTrigger = GetComponent<InteractorSelectionTrigger>();
                }
                _interactorSelectionTrigger.SimulateTriggerExit(LocalPlayerReference.Instance.LocalPlayer.GetComponentInChildren<XRPlayerComponents>());
            }
            referenceToUIObject.SetActive(false);
            _isActive = false;
        }

        public void Update()
        {
            if (_isActive)
            {
                // Position
                if (attachmentTypeEnum is AttachmentTypeEnum.ToWorld)
                {
                    // Right now, there is nothing to do here
                }
                else if (attachmentTypeEnum is AttachmentTypeEnum.ToPlayer)
                {
                    //calculate the given offset relative to the player's current position and rotation
                    Vector3 offsetWithCorrectRotation = Vector3.zero;
                    offsetWithCorrectRotation += attachmentPoint.forward * attachmentOffset.z;
                    offsetWithCorrectRotation += attachmentPoint.right * attachmentOffset.x;
                    offsetWithCorrectRotation += attachmentPoint.up * attachmentOffset.y;

                    if (hasMovementTolerance)
                    {
                        // Update Collider position

                        // Offset of collider's center and position of the object itself
                        Vector3 colliderPivotCenterOffset =
                            toleranceCollider.transform.position - toleranceCollider.bounds.center;

                        offsetWithCorrectRotation -= colliderPivotCenterOffset;

                        toleranceCollider.transform.position = attachmentPoint.position + offsetWithCorrectRotation;
                        toleranceCollider.transform.LookAt(attachmentPoint);

                        // Check UI position for updates
                        if (!toleranceCollider.bounds.Contains(referenceToUIObject.transform.position))
                        {
                            referenceToUIObject.transform.position =
                                toleranceCollider.bounds.ClosestPoint(referenceToUIObject.transform.position);
                        }
                    }
                    else
                    {
                        referenceToUIObject.transform.position = attachmentPoint.position + offsetWithCorrectRotation;
                    }
                }

                // Rotation
                if (facePlayer)
                {
                    FacePlayer();
                }
            }
        }

        public void Start()
        {
            if (referenceToUIObject is null && uiObjectPrefab is null)
            {
                Debug.LogError("Need a reference to ui or ui prefab");
            }

            if (deactivateUIOnStartup)
            {
                if (referenceToUIObject != null) referenceToUIObject.SetActive(false);
            }
        }

        private void FacePlayer()
        {
            referenceToUIObject.transform.LookAt(attachmentPoint);
            referenceToUIObject.transform.Rotate(rotationOffset);
            var rotation = referenceToUIObject.transform.rotation;
            rotation = Quaternion.Euler(new Vector3(0, rotation.eulerAngles.y, rotation.eulerAngles.z));
            referenceToUIObject.transform.rotation = rotation;
        }
    }
}