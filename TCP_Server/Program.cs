﻿using TCP_Server;

var manualResetEvent = new ManualResetEvent(false);
var server = new TcpServer("10.0.0.198", 13375);

server.Listen();

manualResetEvent.WaitOne();