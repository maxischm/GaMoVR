using UnityEngine;

namespace ProceduralNetworkPlayer.Scripts
{
    public class MoveThisTransformLikeOther : MonoBehaviour
    {
        [SerializeField] private Transform transformSource;

        public Transform TransformSource
        {
            get => transformSource;
            set => transformSource = value;
        }

        private void Update()
        {
            var transform1 = transform;
            transform1.position = transformSource.position;
            transform1.rotation = transformSource.rotation;
        }
    }
}