#include <winsock2.h>
#include <ws2tcpip.h>
#include <iostream>
#include <vector>
#include <map>
#include <fstream>
#include <ctime>
#include <string>
#include <thread>
#include <mutex>
#include <windows.h>

#pragma comment(lib, "ws2_32.lib")
#pragma warning(disable:4996)

#define DEFAULT_BUFLEN 4096
#define MAX_CLIENTS 10

using namespace std;

SOCKET server_socket;
map<SOCKET, string> clients_nick;
vector<string> history;
mutex mtx;

string timestamp() {
    time_t now = time(0);
    tm* ltm = localtime(&now);
    char buf[20];
    sprintf(buf, "%02d:%02d:%02d", ltm->tm_hour, ltm->tm_min, ltm->tm_sec);
    return string(buf);
}

void save_message_to_file(const string& msg) {
    ofstream file("history.txt", ios::app);
    if (file.is_open()) {
        file << msg << "\n";
    }
}

void broadcast(const string& message, SOCKET sender, bool include_sender = true) {
    lock_guard<mutex> lock(mtx);
    for (map<SOCKET, string>::iterator it = clients_nick.begin(); it != clients_nick.end(); ++it) {
        SOCKET sock = it->first;
        if (sock != sender || include_sender) {
            send(sock, message.c_str(), (int)message.length(), 0);
        }
    }
}

void client_handler(SOCKET client_socket) {
    char buffer[DEFAULT_BUFLEN];
    int len;

    len = recv(client_socket, buffer, DEFAULT_BUFLEN - 1, 0);
    if (len <= 0) return;
    buffer[len] = '\0';
    string nick(buffer);

    {
        lock_guard<mutex> lock(mtx);
        clients_nick[client_socket] = nick;
    }

    cout << u8"Користувач \"" << nick << "\" підключився\n";

    ifstream file("history.txt");
    string line;
    while (getline(file, line)) {
        send(client_socket, line.c_str(), (int)line.length(), 0);
        send(client_socket, "\n", 1, 0);
    }

    string join_msg = nick + u8" приєднався до чату.";
    broadcast(join_msg, client_socket);
    cout << join_msg << "\n";

    while (true) {
        len = recv(client_socket, buffer, DEFAULT_BUFLEN - 1, 0);
        if (len <= 0) break;

        buffer[len] = '\0';
        string input(buffer);

        if (input == "/exit") break;

        string full_msg = nick + " [" + timestamp() + "]: " + input;

        {
            lock_guard<mutex> lock(mtx);
            history.push_back(full_msg);
            save_message_to_file(full_msg);
        }

        if (input.find("/msg ") == 0) {
            size_t space1 = input.find(' ', 5);
            if (space1 != string::npos) {
                string target_nick = input.substr(5, space1 - 5);
                string msg = input.substr(space1 + 1);
                string formatted = nick + " -> " + target_nick + ": " + msg;

                for (map<SOCKET, string>::iterator it = clients_nick.begin(); it != clients_nick.end(); ++it) {
                    SOCKET sock = it->first;
                    string n = it->second;
                    if (n == target_nick || sock == client_socket) {
                        send(sock, formatted.c_str(), (int)formatted.length(), 0);
                    }
                }

                cout << nick << " -> " << target_nick << ": " << msg << "\n";
                continue;
            }
        }

        broadcast(full_msg, client_socket);
        cout << full_msg << "\n";
    }

    string leave_msg = nick + u8" вийшов з чату.";
    {
        lock_guard<mutex> lock(mtx);
        clients_nick.erase(client_socket);
    }
    broadcast(leave_msg, client_socket);
    cout << u8"Користувач \"" << nick << "\" відключився\n";

    closesocket(client_socket);
}

int main() {
    SetConsoleOutputCP(CP_UTF8);
    SetConsoleCP(CP_UTF8);

    WSADATA wsa;
    WSAStartup(MAKEWORD(2, 2), &wsa);

    server_socket = socket(AF_INET, SOCK_STREAM, 0);
    sockaddr_in server;
    server.sin_family = AF_INET;
    server.sin_addr.s_addr = INADDR_ANY;
    server.sin_port = htons(8888);

    bind(server_socket, (sockaddr*)&server, sizeof(server));
    listen(server_socket, MAX_CLIENTS);

    cout << u8"Сервер запущено. Очікування підключень...\n";

    while (true) {
        SOCKET client_socket = accept(server_socket, NULL, NULL);
        thread(client_handler, client_socket).detach();
    }

    WSACleanup();
    return 0;
}
