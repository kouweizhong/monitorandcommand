﻿/*
 * Copyright (c) 2015, Paul Gaunard (www.codeability.net)
 * All rights reserved.

 * Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
 * - Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
 * - Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the 
 *  documentation and/or other materials provided with the distribution.

 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED 
 * TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR 
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, 
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF 
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS 
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Text;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets; 

using Newtonsoft.Json;

using CodeAbility.MonitorAndCommand.Models;


namespace CodeAbility.MonitorAndCommand.W8Client
{
    internal class SocketClient
    {
        public delegate void MessageStringReceivedEventHandler(object sender, MessageStringEventArgs e);
        public event MessageStringReceivedEventHandler MessageStringReceived;
        protected void OnMessageStringReceived(MessageStringEventArgs e)
        {
            if (MessageStringReceived != null)
                MessageStringReceived(this, e);
        }

        StreamSocket socket = null;

        DataWriter writer = null;
        DataReader reader = null;

        byte[] payload = new Byte[Message.BUFFER_SIZE];

        static ManualResetEvent clientDone = new ManualResetEvent(false);

        // Define a timeout in milliseconds for each asynchronous call. If a response is not received within this 
        // timeout period, the call is aborted.
        const int TIMEOUT_MILLISECONDS = 5000;

        public bool IsConnected { get; set; }

        private Queue<Message> messagesToSend = new Queue<Message>();

        public SocketClient()
        {

        }

        public void Cancel()
        {
            socket.Dispose();
            socket = null;
        }

        public async Task Connect(string hostName, int portNumber)
        {
            socket = new StreamSocket();

            clientDone.Reset();

            await socket.ConnectAsync(new HostName(hostName), portNumber.ToString());

            writer = new DataWriter(socket.OutputStream);
            reader = new DataReader(socket.InputStream);

            IsConnected = true;

            Receive();

            clientDone.WaitOne(TIMEOUT_MILLISECONDS);
        }

        public async Task Send(string data)
        {
            string response = "Operation Timeout";

            if (socket != null)
            {
                string paddedData = CodeAbility.MonitorAndCommand.Helpers.JsonHelpers.PadSerializedMessage(data, Message.BUFFER_SIZE);
                byte[] payload = Encoding.UTF8.GetBytes(paddedData);

                clientDone.Reset();

                writer.WriteBytes(payload);
                await writer.StoreAsync();

                clientDone.Set();

                clientDone.WaitOne(TIMEOUT_MILLISECONDS);
            }
            else
            {
                response = "Socket is not initialized";
            }

            //return response;
        }

        public async void Receive()
        {
            while (IsConnected)
            {
                if (socket != null)
                {
                    clientDone.Reset();

                    try
                    { 
                        await reader.LoadAsync(Message.BUFFER_SIZE);
                        reader.ReadBytes(payload);
                    }
                    catch(Exception exception)
                    {
                        //TODO : implement log
                    }

                    clientDone.Set();

                    string receivedMessage = Encoding.UTF8.GetString(payload, 0, Message.BUFFER_SIZE);
                    OnMessageStringReceived(new MessageStringEventArgs(receivedMessage));

                    clientDone.WaitOne();
                }
            }
        }

        public void Close()
        {
            if (socket != null)
            {
                socket.InputStream.Dispose();
                socket.OutputStream.Dispose();
            }
        }

    }
}
