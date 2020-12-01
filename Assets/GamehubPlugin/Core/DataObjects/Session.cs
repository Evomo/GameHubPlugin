using System;
using System.Collections.Generic;
using System.Linq;
using MotionAI.Core.Models.Generated;
using MotionAI.Core.POCO;
using UnityEngine;

namespace GamehubPlugin.Core {
    [Serializable]
    public class Session {
        public DateTime StartTime;
        public RecordType recordType;
        public int coinsCollected;
        public float score;
        public long timestamp;
        public double duration;
        public Record[] movements, elmos;
        public int gameId;
        private double timeOnPause;

        private DateTime pauseStart; 
        private Dictionary<MovementEnum, Record> _mvDict;
        private Dictionary<ElmoEnum, Record> _elmoDict;

        public Session(GameHubGame game) {
            _mvDict = new Dictionary<MovementEnum, Record>();
            _elmoDict = new Dictionary<ElmoEnum, Record>();
            this.recordType = game.recordType;
            StartTime = DateTime.Now;
            timestamp = ((DateTimeOffset) StartTime).ToUnixTimeSeconds();
            this.gameId = game.gameId;
        }

        public override string ToString() {
            return JsonUtility.ToJson(this);
        }

        public Session EndSession() {
            duration = (DateTime.Now - StartTime).TotalSeconds - timeOnPause;
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
            bool overrideFlag = recordType == 0;
            if (recordType.HasFlag(RecordType.ElementalMove) || overrideFlag ) {
                foreach (ElementalMovement elmo in m.elmos) {
                    if (!elmo.rejected) {
                        UpdateRecord(elmo.typeID, ref _elmoDict);
                    }
                }
            }

            if (recordType.HasFlag(RecordType.Movements) || overrideFlag) {
                if(m.typeID != 0 )UpdateRecord(m.typeID, ref _mvDict);
            }
        }

        public void TogglePause(bool shouldPause) {
            if (shouldPause) {
                pauseStart = DateTime.Now;
            }
            else {
                timeOnPause += (DateTime.Now - pauseStart).TotalSeconds;
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