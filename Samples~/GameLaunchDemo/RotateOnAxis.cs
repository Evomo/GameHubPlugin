using UnityEngine;

namespace GamehubPlugin.Samples.GameLaunchDemo {
    public class RotateOnAxis : MonoBehaviour {
        void Update() {
            
            transform.Rotate(Vector3.up, 50 * Time.deltaTime);
        }
    }
}