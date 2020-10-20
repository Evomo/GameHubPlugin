using System;
using System.Collections.Generic;

namespace GamehubPlugin.Core {
	[Serializable]
	public class CommunicationMessages {
		public List<Session> sessions;
		public CommunicationMessageType messageType;
	}

	public enum CommunicationMessageType {
		ENDGAME,
		CURRENTSESSION
	}
}