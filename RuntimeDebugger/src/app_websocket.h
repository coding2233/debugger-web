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
#include <vector>

#ifdef __EMSCRIPTEN__
#include <emscripten.h>
#include <emscripten/websocket.h>
#else
#ifdef _WIN32
#pragma comment( lib, "ws2_32" )
#include <WinSock2.h>
#endif
#endif
#include "easywsclient.hpp"
using easywsclient::WebSocket;

class AppWebsocket 
{
public:
	AppWebsocket();
	~AppWebsocket();

	std::unique_ptr<WebSocket> Connect(const char* websocket_address);

// #ifdef __EMSCRIPTEN__
// 	void AddData(void* data);
// 	void SetWebSocketEvent(const EmscriptenWebSocketOpenEvent* websocket_event);
// #endif

// private:
// #ifdef __EMSCRIPTEN__
// 	std::queue<void*> data_queue_;
// 	const EmscriptenWebSocketOpenEvent* websocket_event_;
// #else
// 	//CURL* websocket_curl_;
// #endif
// 	MessageCallback *message_callback_;
};


#endif //RUNTIMEDEBUGGER_APP_WEBSOCKET_H
