#ifndef __HTTPD_EXPORT_H__
#define __HTTPD_EXPORT_H__

#if defined _WIN32 || defined __CYGWIN__
#include "windows.h"
#define API __declspec(dllexport)
#elif __GNUC__
#define API  __attribute__((__visibility__("default")))
#else
#define API
#endif

#if defined __cplusplus
#define EXTERN extern "C"
#else
#define EXTERN extern
#endif

#define EXPORT_API EXTERN API

#include "hv/WebSocketServer.h"

typedef void (*OnWebSocketOpen)(const WebSocketChannelPtr& channel, const char* req_path);
typedef void (*OnWebSocketMessage)(const WebSocketChannelPtr& channel, const std::string& msg);
typedef void (*OnWebSocketClose)(const WebSocketChannelPtr& channel);

EXPORT_API int CreateHttpService(int port,const char* config_path);
EXPORT_API void RunHttpService();
EXPORT_API void BindWebSocketService(OnWebSocketOpen on_open,OnWebSocketMessage on_message,OnWebSocketClose on_close);
EXPORT_API void WebSocketSend(const WebSocketChannelPtr& channel,const char* message);


#endif