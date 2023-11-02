#include <vector>
#include <string>

#ifdef __EMSCRIPTEN__
#include <emscripten.h>
#include <emscripten/websocket.h>
#endif

#include "easywsclient.hpp"

using easywsclient::Callback_Imp;
using easywsclient::BytesCallback_Imp;

namespace {
#ifdef __EMSCRIPTEN__

EM_BOOL onopen(int eventType, const EmscriptenWebSocketOpenEvent* websocketEvent, void* userData) {
    puts("onopen");
    AppWebsocket* app_websocket = (AppWebsocket*)userData;
    app_websocket->SetWebSocketEvent(websocketEvent);
   /* EMSCRIPTEN_RESULT result;
    result = emscripten_websocket_send_utf8_text(websocketEvent->socket, "hello world");
    if (result) {
        printf("Failed to emscripten_websocket_send_utf8_text(): %d\n", result);
    }*/
    return EM_TRUE;
}
EM_BOOL onerror(int eventType, const EmscriptenWebSocketErrorEvent* websocketEvent, void* userData) {
    puts("onerror");

    return EM_TRUE;
}
EM_BOOL onclose(int eventType, const EmscriptenWebSocketCloseEvent* websocketEvent, void* userData) {
    puts("onclose");

    return EM_TRUE;
}
EM_BOOL onmessage(int eventType, const EmscriptenWebSocketMessageEvent* websocketEvent, void* userData) {
    puts("onmessage");

    if (userData != nullptr)
    {
        AppWebsocket* appWebsocket = (AppWebsocket*)userData;
        appWebsocket->AddData(websocketEvent->data);
    }
    //if (websocketEvent->isText) {
    //    // For only ascii chars.
    //    printf("message: %s\n", websocketEvent->data);
    //}

    //EMSCRIPTEN_RESULT result;
    //result = emscripten_websocket_close(websocketEvent->socket, 1000, "no reason");
    //if (result) {
    //    printf("Failed to emscripten_websocket_close(): %d\n", result);
    //}
    return EM_TRUE;
}
#endif

class _EMScriptenWebSocket : public easywsclient::WebSocket
{
  public:
    _EMScriptenWebSocket(const std::string& url) {}
    void poll(int timeout) { }
    void send(const std::string& message) { }
    void sendBinary(const std::string& message) { }
    void sendBinary(const std::vector<uint8_t>& message) { }
    void sendPing() { }
    void close() { } 
    readyStateValues getReadyState() const { return CLOSED; }
    void _dispatch(Callback_Imp & callable) { }
    void _dispatchBinary(BytesCallback_Imp& callable) { }
};

    easywsclient::WebSocket::pointer from_url(const std::string& url) {
    {
        return easywsclient::WebSocket::pointer(new _EMScriptenWebSocket(url));
    }
}

namespace easywsclient {
    WebSocket::pointer WebSocket::from_url(const std::string& url,const std::string& origin) {
        return ::from_url(url);
    }
}