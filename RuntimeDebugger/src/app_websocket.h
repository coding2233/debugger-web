//
// Created by wanderer on 2023/10/29.
//

#ifndef RUNTIMEDEBUGGER_APP_WEBSOCKET_H
#define RUNTIMEDEBUGGER_APP_WEBSOCKET_H

#include <queue>
#include <stdio.h>
#include <string>
#include <assert.h>
#include <memory>

#ifdef __EMSCRIPTEN__
#include <emscripten.h>
#include <emscripten/websocket.h>
#else
#include "easywsclient.hpp"
#ifdef _WIN32
#pragma comment( lib, "ws2_32" )
#include <WinSock2.h>
#endif
using easywsclient::WebSocket;
#endif

class AppWebsocket 
{
public:
	AppWebsocket();
	~AppWebsocket();

	int Connect(const char* websocket_address);
	void Close();
	void Send(const void *data, size_t size);
	int Recv(void* data, size_t size, size_t* recv_size);

#ifdef __EMSCRIPTEN__
	void AddData(void* data);
#endif

private:
#ifdef __EMSCRIPTEN__
	std::queue<void*> data_queue_;
	EmscriptenWebSocketOpenEvent* websocket_event_;
#else
	//CURL* websocket_curl_;
#endif
};


#endif //RUNTIMEDEBUGGER_APP_WEBSOCKET_H
