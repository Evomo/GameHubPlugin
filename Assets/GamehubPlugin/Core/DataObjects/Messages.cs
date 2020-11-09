using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GamehubPlugin.Core {
    [Serializable]
    public struct CommunicationMessages {
        public Session session;

        public CommunicationMessageType messageType;

        public override string ToString() {
            return $@"{{
                ""gameSession"" :{session.ToString()},
                ""messageType"": {Enum.GetName(typeof(CommunicationMessageType), messageType)}
        }}";
            return JsonUtility.ToJson(session);
        }
    }

    public enum CommunicationMessageType {
        PAUSE,
        GAMEOVER,
        GAMELOADED
    }
}