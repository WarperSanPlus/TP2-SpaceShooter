using UnityEngine;
using UtilityScripts;

namespace Movements
{
    [RequireComponent(typeof(MoveByVector))]
    public class ZigZagMovement : MonoBehaviour
    {
        [SerializeField]
        private float multiplier;

        private MoveByVector moveByVector;

        private void Start() => this.moveByVector = this.gameObject.GetComponent<MoveByVector>();

        // Update is called once per frame
        private void FixedUpdate()
        {
            Vector3 dir = this.moveByVector.direction;

            dir.x = Mathf.Atan(Mathf.Cos(Time.time));

            this.moveByVector.speed = this.multiplier * Mathf.Abs(dir.x);
            this.moveByVector.direction = dir;
        }
    }
}