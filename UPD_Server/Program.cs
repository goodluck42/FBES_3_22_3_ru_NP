using UPD_Server;

var manualResetEvent = new ManualResetEvent(false);


var server = new UdpServer("127.0.0.1", 13374);

server.Listen();

manualResetEvent.WaitOne();