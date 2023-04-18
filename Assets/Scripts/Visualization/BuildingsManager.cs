using UnityEngine;

namespace Visualization
{
    public class BuildingsManager : MonoBehaviour
    {
        public float maxDistance = 10000f;
        public float delay = 10f;

        private void Start()
        {
            Invoke(nameof(AlignObjects), delay);
        }

        private void AlignObjects()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                Ray ray = new(child.position, Vector3.down);

                if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
                {
                    child.position = new Vector3(child.position.x, hit.point.y, child.position.z);
                }
            }
        }
    }
}