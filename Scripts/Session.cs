using System;
using UnityEngine;

namespace GamehubPlugin {
	[Serializable]
	public class Session {

		public DateTime startTime;
		public string name;
		public float score;
		public double duration;
		public Session(string gameName) {
			startTime = DateTime.Now;
			name = gameName;
		}


		public override string ToString() {
			return JsonUtility.ToJson(this);
		}

		public void EndSession(float sessionScore) {
			duration = (DateTime.Now - startTime).TotalSeconds;
			score = sessionScore;
		}
	}
}
