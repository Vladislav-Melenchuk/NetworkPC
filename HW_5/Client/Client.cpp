#define WIN32_LEAN_AND_MEAN

#include <ws2tcpip.h>
#include <windows.h>
#include <iostream>
#include <string>

#pragma comment (lib, "Ws2_32.lib")

#define DEFAULT_BUFLEN 4096
#define SERVER_IP "127.0.0.1"
#define DEFAULT_PORT "8888"

using namespace std;

SOCKET client_socket;
string nickname;

DWORD WINAPI Sender(void* param)
{
    while (true) {
        string msg;
        cout << u8"-> " << flush;
        getline(cin, msg);

        if (msg == "/exit") {
            send(client_socket, msg.c_str(), msg.length(), 0);
            closesocket(client_socket);
            WSACleanup();
            exit(0);
        }

        send(client_socket, msg.c_str(), msg.length(), 0);
    }
}

DWORD WINAPI Receiver(void* param)
{
    char buffer[DEFAULT_BUFLEN];
    int result;
    while ((result = recv(client_socket, buffer, DEFAULT_BUFLEN - 1, 0)) > 0) {
        buffer[result] = '\0';
        cout << "\r" << buffer << "\n" << flush;
        cout << u8"-> " << flush;
    }
    return 0;
}

int main()
{
    SetConsoleOutputCP(CP_UTF8);
    SetConsoleCP(CP_UTF8);
    SetConsoleCtrlHandler(NULL, TRUE);
    system("title Клієнт");

    cout << u8"Введіть ваш нік: " << flush;
    getline(cin, nickname);

    WSADATA wsaData;
    int iResult = WSAStartup(MAKEWORD(2, 2), &wsaData);
    if (iResult != 0) {
        cout << u8"Помилка WSAStartup: " << iResult << "\n" << flush;
        return 1;
    }

    addrinfo hints = {};
    hints.ai_family = AF_INET;
    hints.ai_socktype = SOCK_STREAM;
    hints.ai_protocol = IPPROTO_TCP;

    addrinfo* result = nullptr;
    iResult = getaddrinfo(SERVER_IP, DEFAULT_PORT, &hints, &result);
    if (iResult != 0) {
        cout << u8"Помилка getaddrinfo: " << iResult << "\n" << flush;
        WSACleanup();
        return 1;
    }

    client_socket = socket(result->ai_family, result->ai_socktype, result->ai_protocol);
    if (client_socket == INVALID_SOCKET) {
        cout << u8"Не вдалося створити сокет.\n" << flush;
        freeaddrinfo(result);
        WSACleanup();
        return 1;
    }

    iResult = connect(client_socket, result->ai_addr, (int)result->ai_addrlen);
    freeaddrinfo(result);

    if (iResult == SOCKET_ERROR) {
        cout << u8"Не вдалося підключитися до сервера.\n" << flush;
        closesocket(client_socket);
        WSACleanup();
        return 1;
    }

    send(client_socket, nickname.c_str(), nickname.length(), 0);

    CreateThread(NULL, 0, Receiver, NULL, 0, NULL);
    CreateThread(NULL, 0, Sender, NULL, 0, NULL);

    while (true) Sleep(1000);

    closesocket(client_socket);
    WSACleanup();
    return 0;
}
