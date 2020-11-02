using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GamehubPlugin.Core {
    [Serializable]
    public class CommunicationMessages {
        public List<Session> sessions;
        public CommunicationMessageType messageType;

        public override string ToString() {
            if (messageType == CommunicationMessageType.CURRENTSESSION) {
                return JsonUtility.ToJson(sessions.Last());
            }

            return JsonUtility.ToJson(sessions);
        }
    }

    public enum CommunicationMessageType {
        ENDGAME,
        CURRENTSESSION
    }
}