//
// Created by wanderer on 2023/10/29.
//

#ifndef RUNTIMEDEBUGGER_APP_WEBSOCKET_H
#define RUNTIMEDEBUGGER_APP_WEBSOCKET_H

#include <queue>

#ifdef __EMSCRIPTEN__
#include <emscripten.h>
#include <emscripten/websocket.h>
#else
#include <curl/curl.h>
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

private:
#ifdef __EMSCRIPTEN__
	std::queue<void*> data_queue_;
	EmscriptenWebSocketOpenEvent* websocket_event_;
	EM_BOOL onopen(int eventType, const EmscriptenWebSocketOpenEvent* websocketEvent, void* userData);
	EM_BOOL onerror(int eventType, const EmscriptenWebSocketErrorEvent* websocketEvent, void* userData);
	EM_BOOL onclose(int eventType, const EmscriptenWebSocketCloseEvent* websocketEvent, void* userData);
	EM_BOOL onmessage(int eventType, const EmscriptenWebSocketMessageEvent* websocketEvent, void* userData);
#else
	CURL* websocket_curl_;
#endif
};


#endif //RUNTIMEDEBUGGER_APP_WEBSOCKET_H
