#include <future>
#include <WinSock2.h>
#include <WS2tcpip.h>
#include <iostream>
#include <thread>

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

    SOCKET serverSocket = socket(hints.ai_family, hints.ai_socktype, hints.ai_protocol);

    if (serverSocket == INVALID_SOCKET)
    {
        std::cout << "Failed to create socket" << '\n';

        WSACleanup();

        return -1;
    }

    addrinfo* socketAddressInfo;

    if (int result = getaddrinfo("127.0.0.1", "13374", &hints, &socketAddressInfo); result != 0)
    {
        std::cout << "Failed to get address info" << '\n';

        WSACleanup();
        freeaddrinfo(socketAddressInfo);

        return result;
    }

    if (int result = bind(serverSocket, socketAddressInfo->ai_addr, static_cast<int>(socketAddressInfo->ai_addrlen));
        result != 0)
    {
        std::cout << "Failed to bind socket" << '\n';

        WSACleanup();
        freeaddrinfo(socketAddressInfo);
        return result;
    }

    if (int result = listen(serverSocket, SOMAXCONN); result != 0)
    {
        std::cout << "Failed to listen socket" << '\n';

        WSACleanup();
        freeaddrinfo(socketAddressInfo);

        return result;
    }

    constexpr size_t maxClientCount = 3;
    size_t currentClientCount = 0;


    std::cout << "Server started!" << '\n';

    std::vector<std::thread> clients;
    
    while (true)
    {
        // sockaddr addr;
        // int size;

        SOCKET clientSocket = accept(serverSocket, NULL, NULL);

        if (clientSocket == INVALID_SOCKET)
        {
            continue;
        }
        
        std::cout << "Client accepted!" << '\n';
        
        ++currentClientCount;

        std::thread thread{
            [clientSocket]()
            {
                constexpr size_t len = 2048;
                auto buffer = new char[len]{};

                while (recv(clientSocket, buffer, len, NULL) > 0)
                {
                    std::cout << "Message: " << buffer << '\n';

                    std::memset(buffer, 0, len);
                }

                closesocket(clientSocket);
            
                delete[] buffer;

                std::cout << "Client disconnected!" << '\n';
            }
        };

        clients.push_back(std::move(thread));
    }

    WSACleanup();

    return 0;
}
