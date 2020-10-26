using UnityEngine;

namespace GamehubPlugin.Samples.GameLaunchDemo {
	public class RotateCube : MonoBehaviour {
		private Vector3 rotationAxis;

		void Start() {
			rotationAxis = Random.insideUnitSphere;
		}

		void Update() {
	
			transform.Rotate(rotationAxis, 20 * Time.deltaTime );
		}
	}
}