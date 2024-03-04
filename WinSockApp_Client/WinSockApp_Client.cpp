#include <WinSock2.h>
#include <WS2tcpip.h>
#include <iostream>
#include <string>

#pragma comment (lib, "Ws2_32.lib")

int main(int argc, char* argv[])
{
    WORD version = MAKEWORD(2, 2); // 2.2
    WSAData wsaData;
    
    if (int result = WSAStartup(version, &wsaData); result != 0)
    {
        std::cout << "Failed to initialize WinsSock" << '\n';

        return result;
    }

    addrinfo hints;

    ZeroMemory(&hints, sizeof(hints));

    hints.ai_family = AF_INET;
    hints.ai_protocol = IPPROTO_TCP;
    hints.ai_socktype = SOCK_STREAM;
    
    SOCKET clientSocket = socket(hints.ai_family, hints.ai_socktype, hints.ai_protocol);
    
    addrinfo* socketAddressInfo;

    if (int result = getaddrinfo("127.0.0.1", "13374", &hints, &socketAddressInfo); result != 0)
    {
        std::cout << "Failed to get address info" << '\n';

        WSACleanup();
        freeaddrinfo(socketAddressInfo);

        return result;
    }
    

    if (int result = connect(clientSocket, socketAddressInfo->ai_addr, static_cast<int>(socketAddressInfo->ai_addrlen)); result != 0)
    {
        std::cout << "Failed to connect to the server" << '\n';
        WSACleanup();
        freeaddrinfo(socketAddressInfo);

        return result;
    }
    
    std::string message;
    
    while(true)
    {
        std::cout << "Enter a message: ";
        std::getline(std::cin, message);

        if (message == "0" || message.length() == 0)
        {
            closesocket(clientSocket);
            break;
        }

        send(clientSocket, message.c_str(), message.length(), NULL);
    }
    
    return 0;
}
