using System;
using System.Collections.Generic;
using System.Linq;
using MotionAI.Core.Models.Generated;
using MotionAI.Core.POCO;
using UnityEngine;

namespace GamehubPlugin {
	[Serializable]
	public class Session {
		public DateTime StartTime;
		public string name;
		public float score;
		public double duration;
		public RecordedMovement[] movements;
		public long timestamp;


		private Dictionary<MovementEnum, RecordedMovement> _mvDict;

		public Session(string gameName) {
			_mvDict = new Dictionary<MovementEnum, RecordedMovement>();
			StartTime = DateTime.Now;
			timestamp = ((DateTimeOffset)StartTime).ToUnixTimeSeconds();
			name = gameName;
		}

		public override string ToString() {
			return JsonUtility.ToJson(this);
		}

		public void EndSession(float sessionScore) {
			duration = (DateTime.Now - StartTime).TotalSeconds;
			score = sessionScore;
			Debug.Log(_mvDict.Values.ToArray());
			movements = _mvDict.Values.ToArray();
		}

		public void RecordMovement(MovementDto m) {
			Debug.Log("recording");
			RecordedMovement rm;
			if (!_mvDict.TryGetValue(m.typeID, out rm)) {
				rm = new RecordedMovement(m.typeID);
				_mvDict[m.typeID] = rm;
			}

			rm.amount++;
		}

		[Serializable]
		public class RecordedMovement {
			public string name;
			public int amount;

			public RecordedMovement(MovementEnum e) {
				name = e.ToString();
				amount = 0;
			}
		}
	}
}