using System;
using System.Collections.Generic;
using System.Linq;
using MotionAI.Core.Models.Generated;
using MotionAI.Core.POCO;
using UnityEngine;

namespace GamehubPlugin.Core {
	public class SessionSettings {
		public static SessionSettings CreateInstance(GameHubGame game) {
			SessionSettings sess = new SessionSettings();
			sess.gameId = game.gameId;
			sess.recordElmos = game.recordElmos;
			return sess;
		}

		public int gameId;
		public bool recordElmos;
	}

	[Serializable]
	public class Session {
		public DateTime StartTime;
		public bool recordElmos;
		public int coinsCollected;
		public float score;
		public long timestamp;
		public double duration;
		public Record[] movements, elmos;
		public int gameId;


		private Dictionary<MovementEnum, Record> _mvDict;
		private Dictionary<ElmoEnum, Record> _elmoDict;

		public Session(int gameId, bool recordElmos = false) {
			_mvDict = new Dictionary<MovementEnum, Record>();
			_elmoDict = new Dictionary<ElmoEnum, Record>();
			this.recordElmos = recordElmos;
			StartTime = DateTime.Now;
			timestamp = ((DateTimeOffset) StartTime).ToUnixTimeSeconds();
			this.gameId = gameId;
		}

		public override string ToString() {
			return JsonUtility.ToJson(this);
		}

		public Session EndSession(int sessionScore, int coins) {
			duration = (DateTime.Now - StartTime).TotalSeconds;
			score = sessionScore;
			coinsCollected = coins;
			movements = _mvDict.Values.ToArray();
			elmos = _elmoDict.Values.ToArray();
			return this;
		}

		private void UpdateRecord<T>(T enumVal, ref Dictionary<T, Record> dict) {
			Record rm;
			if (!dict.TryGetValue(enumVal, out rm)) {
				rm = new Record(Enum.GetName(typeof(T), enumVal));
				dict[enumVal] = rm;
			}

			rm.amount++;
		}

		public void RecordMovement(EvoMovement m) {
			UpdateRecord(m.typeID, ref _mvDict);
			if (recordElmos) {
				foreach (ElementalMovement elmo in m.elmos) {
					UpdateRecord(elmo.typeID, ref _elmoDict);
				}
			}
		}

		[Serializable]
		public class Record {
			public string name;
			public int amount;

			public Record(string e) {
				name = e;
				amount = 0;
			}
		}
	}
}