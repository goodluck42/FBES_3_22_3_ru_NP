using UPD_Server;

var manualResetEvent = new ManualResetEvent(false);
var server = new UdpServer("10.0.0.198", 13375);

server.Listen();

manualResetEvent.WaitOne();