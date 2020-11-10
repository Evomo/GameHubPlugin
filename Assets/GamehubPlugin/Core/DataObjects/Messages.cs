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
            string s = $@"{{
                ""gameSession"" :{session.ToString()},
                ""messageType"": {Enum.GetName(typeof(CommunicationMessageType), messageType)}
        }}";
            return s;
        }
    }

    public enum CommunicationMessageType {
        PAUSE,
        GAMEOVER,
        GAMELOADED
    }
}