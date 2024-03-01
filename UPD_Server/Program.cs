using UPD_Server;

var manualResetEvent = new ManualResetEvent(false);
var server = new UdpServer("10.0.0.101", 13374);

server.Listen();

manualResetEvent.WaitOne();