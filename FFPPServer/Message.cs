﻿using System;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Net;
namespace FFPPServer
{
    [DataContract(Name = "serverMessage", Namespace = "serverMessage")]
    public class Message : IExtensibleDataObject
    {
        //https://www.codeproject.com/Articles/140911/log-net-Tutorial
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(
                typeof(Message)

            );
        //Conversation and Message ID or GWID and sequence number
        [DataMember(Name = "thisMessageType")]
        public MessageType thisMessageType;
        [DataMember(Name = "messageBody")]
        public String messageBody;
        [DataMember(Name = "fromAddress")]
        public IPEndPoint fromAddress;
        public Message(MessageType inputMsgType, String inputMessageBody)
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
