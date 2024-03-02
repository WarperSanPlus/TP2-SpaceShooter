using UnityEngine;

namespace UtilityScripts
{
    public class MoveByVector : MonoBehaviour
    {
        [Tooltip("Direction of the movement")]
        public Vector3 direction;

        [SerializeField, Tooltip("By how much the movement is sped up")]
        public float speed;

        // Update is called once per frame
        private void FixedUpdate() 
            => this.transform.Translate(this.speed * Time.fixedDeltaTime * this.direction.normalized, Space.World);
    }
}
