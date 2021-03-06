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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeAbility.MonitorAndCommand.Client;
using CodeAbility.MonitorAndCommand.Environment;
using CodeAbility.MonitorAndCommand.Models;

namespace CodeAbility.MonitorAndCommand.RemoteConsole
{
    public class PibrellaRemote
    {
        const int HEARTBEAT_PERIOD_IN_MILLESECONDS = 10000; 

        public static void Start(string ipAddress, int portNumber)
        {
            MessageClient messageClient = new MessageClient(Devices.WINDOWS_PHONE, HEARTBEAT_PERIOD_IN_MILLESECONDS);

            messageClient.DataReceived += client_DataReceived;

            Console.WriteLine("Remote console");
            Console.WriteLine("Hit a key to start client, hit [0,3] to send Pibrella commands, hit ESC to exit.");
            Console.ReadKey();

            messageClient.Start(ipAddress, portNumber);

            Console.WriteLine("Running.");

            messageClient.SubscribeToData(Devices.RASPBERRY_PI_B, Pibrella.OBJECT_GREEN_LED, Pibrella.DATA_LED_STATUS);
            messageClient.SubscribeToData(Devices.RASPBERRY_PI_B, Pibrella.OBJECT_YELLOW_LED, Pibrella.DATA_LED_STATUS);
            messageClient.SubscribeToData(Devices.RASPBERRY_PI_B, Pibrella.OBJECT_RED_LED, Pibrella.DATA_LED_STATUS);
            messageClient.SubscribeToData(Devices.RASPBERRY_PI_B, Pibrella.OBJECT_BUTTON, Pibrella.DATA_BUTTON_STATUS);

            messageClient.PublishCommand(Devices.RASPBERRY_PI_B, Pibrella.OBJECT_GREEN_LED, Pibrella.COMMAND_TOGGLE_LED);
            messageClient.PublishCommand(Devices.RASPBERRY_PI_B, Pibrella.OBJECT_YELLOW_LED, Pibrella.COMMAND_TOGGLE_LED);
            messageClient.PublishCommand(Devices.RASPBERRY_PI_B, Pibrella.OBJECT_RED_LED, Pibrella.COMMAND_TOGGLE_LED);
            messageClient.PublishCommand(Devices.RASPBERRY_PI_B, Pibrella.OBJECT_BUTTON, Pibrella.COMMAND_BUTTON_PRESSED);

            bool running = true;
            while (running)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey();

                if (keyInfo.KeyChar.Equals('0'))
                {
                    messageClient.SendCommand(Devices.RASPBERRY_PI_B, Pibrella.OBJECT_BUTTON, Pibrella.COMMAND_BUTTON_PRESSED, String.Empty);
                }
                if (keyInfo.KeyChar.Equals('1'))
                {
                    messageClient.SendCommand(Devices.RASPBERRY_PI_B, Pibrella.OBJECT_GREEN_LED, Pibrella.COMMAND_TOGGLE_LED, String.Empty);
                }
                else if (keyInfo.KeyChar.Equals('2'))
                {
                    messageClient.SendCommand(Devices.RASPBERRY_PI_B, Pibrella.OBJECT_YELLOW_LED, Pibrella.COMMAND_TOGGLE_LED, String.Empty);
                }
                else if (keyInfo.KeyChar.Equals('3'))
                {
                    messageClient.SendCommand(Devices.RASPBERRY_PI_B, Pibrella.OBJECT_RED_LED, Pibrella.COMMAND_TOGGLE_LED, String.Empty);
                }
                else if (keyInfo.Key == ConsoleKey.Escape)
                {
                    running = false;
                    break;
                }
            }

            Console.WriteLine("Stopped.");

            messageClient.Stop();
        }

        static void client_DataReceived(object sender, MessageEventArgs e)
        {
            Console.WriteLine(e);
        }       
    }
}
