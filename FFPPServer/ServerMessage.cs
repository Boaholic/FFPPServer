﻿using System;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Json;

namespace FFPPServer
{
    [DataContract(Name = "ServerMessage", Namespace = "ServerMessage")]
    public class ServerMessage : IExtensibleDataObject
    {
        //https://www.codeproject.com/Articles/140911/log-net-Tutorial
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(
                typeof(ServerMessage)
            );
        public enum messageType
        {
            JOIN,
            ACK,
            HB,
            CHAT
        }
        [DataMember(Name = "thisMessageType")]
        public messageType thisMessageType;
        [DataMember(Name = "messageBody")]
        public String messageBody;

        public ServerMessage(messageType inputMsgType, String inputMessageBody)
        {
            thisMessageType = inputMsgType;
            messageBody = inputMessageBody;
            log.Info("Input Message: " + inputMessageBody);
        }

        private ExtensionDataObject messageDataValue;
        public ExtensionDataObject ExtensionData
        {
            get
            {
                return messageDataValue;
            }
            set
            {
                messageDataValue = value;
            }
        }
    }
}
