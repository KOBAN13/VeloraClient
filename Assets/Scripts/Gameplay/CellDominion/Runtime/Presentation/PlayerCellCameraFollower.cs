using UnityEngine;

namespace Gameplay.CellDominion.Runtime.Presentation
{
    public sealed class PlayerCellCameraFollower : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float followSpeed = 8f;
        [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            Vector3 targetPosition = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }

        public void SetTarget(Transform value)
        {
            target = value;
        }
    }
}
