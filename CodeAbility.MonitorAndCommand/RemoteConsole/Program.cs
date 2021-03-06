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
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAbility.MonitorAndCommand.RemoteConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string ipAddress = ConfigurationManager.AppSettings["IpAddress"];
            int portNumber = Int32.Parse(ConfigurationManager.AppSettings["PortNumber"]);

            Console.WriteLine("Remote console.");
            Console.WriteLine("Hit [1] to start a Data Generator receptor.");
            Console.WriteLine("Hit [2] to start a Pibrella remote.");
            Console.WriteLine("Hit [3] to start a LEDs remote.");
            Console.WriteLine("Hit [4] to start a MCP4921 remote.");
            Console.WriteLine("Hit [5] to start a Photon remote.");

            ConsoleKeyInfo keyInfo;

            do
            {
                keyInfo = Console.ReadKey();
            }
            while (!(keyInfo.KeyChar.Equals('1') || 
                     keyInfo.KeyChar.Equals('2') || 
                     keyInfo.KeyChar.Equals('3') || 
                     keyInfo.KeyChar.Equals('4') || 
                     keyInfo.KeyChar.Equals('5')));

            if (keyInfo.KeyChar.Equals('1'))
                DataGeneratorRemote.Start(ipAddress, portNumber);
            else if (keyInfo.KeyChar.Equals('2'))
                PibrellaRemote.Start(ipAddress, portNumber);
            else if (keyInfo.KeyChar.Equals('3'))
                NetduinoRemote.Start(ipAddress, portNumber);
            else if (keyInfo.KeyChar.Equals('4'))
                MCP4921Remote.Start(ipAddress, portNumber);
            else if (keyInfo.KeyChar.Equals('5'))
                PhotonRemote.Start(ipAddress, portNumber);
        }


    }
}
