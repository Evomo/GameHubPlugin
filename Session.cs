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
		public List<RecordedMovement> movements;


		private Dictionary<MovementEnum, RecordedMovement> _mvDict;

		public Session(string gameName) {
			_mvDict = new Dictionary<MovementEnum, RecordedMovement>();
			movements = new List<RecordedMovement>();
			StartTime = DateTime.Now;
			name = gameName;
		}

		public override string ToString() {
			return JsonUtility.ToJson(this);
		}

		public void EndSession(float sessionScore) {
			duration = (DateTime.Now - StartTime).TotalSeconds;
			score = sessionScore;
			movements = _mvDict.Values.ToList();
		}

		public void RecordMovement(MovementDto m) {
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