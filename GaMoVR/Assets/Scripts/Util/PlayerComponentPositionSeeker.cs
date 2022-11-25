using UnityEngine;

namespace ProceduralNetworkPlayer.Scripts
{
    public class PlayerComponentPositionSeeker : MonoBehaviour
    {
        public GameObject leftHandTargetObject;
        public GameObject rightHandTargetObject;
        public GameObject headTargetObject;

        private GameObject leftHandSourceObject;
        private GameObject rightHandSourceObject;
        private GameObject headSourceObject;

        private void Start()
        {
            foreach (var position in FindObjectsOfType<PlayerComponentPositionFinder>())
            {
                if (position.positionType is PlayerComponentPositionFinder.PositionType.LeftHand)
                {
                    leftHandSourceObject = position.gameObject;
                }
                else if (position.positionType is PlayerComponentPositionFinder.PositionType.RightHand)
                {
                    rightHandSourceObject = position.gameObject;
                }
                else if (position.positionType is PlayerComponentPositionFinder.PositionType.Head)
                {
                    headSourceObject = position.gameObject;
                }
            }

            if (leftHandSourceObject is null)
            {
                Debug.LogError("Could not find left Hand Position");
            }

            if (rightHandSourceObject is null)
            {
                Debug.LogError("Could not find right Hand Position");
            }

            if (headSourceObject is null)
            {
                Debug.LogError("Could not find head Position");
            }
        }

        private void Update()
        {
            if (!(leftHandSourceObject is null))
            {
                leftHandTargetObject.transform.position = leftHandSourceObject.transform.position;
                leftHandTargetObject.transform.rotation = leftHandSourceObject.transform.rotation;
            }

            if (!(rightHandSourceObject is null))
            {
                rightHandTargetObject.transform.position = rightHandSourceObject.transform.position;
                rightHandTargetObject.transform.rotation = rightHandSourceObject.transform.rotation;
            }

            if (!(headSourceObject is null))
            {
                headTargetObject.transform.position = headSourceObject.transform.position;
                headTargetObject.transform.rotation = headSourceObject.transform.rotation;
            }
        }
    }
}