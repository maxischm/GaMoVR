using UnityEngine;

namespace ProceduralNetworkPlayer.Scripts
{
    public class PlayerComponentPositionFinder : MonoBehaviour
    {
        public enum PositionType
        {
            LeftHand,
            RightHand,
            Head
        }

        public PositionType positionType;
    }
}