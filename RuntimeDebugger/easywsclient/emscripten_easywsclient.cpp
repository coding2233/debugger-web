#ifdef __EMSCRIPTEN__

#include <vector>
#include <string>
#include <emscripten.h>
#include <emscripten/websocket.h>


#include "easywsclient.hpp"

using easywsclient::Callback_Imp;
using easywsclient::BytesCallback_Imp;

namespace {


EM_BOOL onopen(int eventType, const EmscriptenWebSocketOpenEvent* websocketEvent, void* userData) {
    puts("onopen");
    _EMScriptenWebSocket* ems_websocket = (_EMScriptenWebSocket*)userData;
    ems_websocket->websocket_event_ =  websocketEvent;
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
        _EMScriptenWebSocket* emsWebsocket = (_EMScriptenWebSocket*)userData;
        emsWebsocket->AddData(websocketEvent->data);
    }
    //if (websocketEvent->isText) {
    //    // For only ascii chars.
    //    printf("message: %s\n", websocketEvent->data);
    //}

    
    return EM_TRUE;
}


class _EMScriptenWebSocket : public easywsclient::WebSocket
{
    public:
    _EMScriptenWebSocket(const std::string& url) 
    {
    if (!emscripten_websocket_is_supported()) 
    {
        return -1;
    }
    EmscriptenWebSocketCreateAttributes ws_attrs = {
        websocket_address,
        NULL,
        EM_TRUE
    };

    EMSCRIPTEN_WEBSOCKET_T ws = emscripten_websocket_new(&ws_attrs);
    emscripten_websocket_set_onopen_callback(ws, this, onopen);
    emscripten_websocket_set_onerror_callback(ws, this, onerror);
    emscripten_websocket_set_onclose_callback(ws, this, onclose);
    emscripten_websocket_set_onmessage_callback(ws, this, onmessage);
    }
protected:
    const EmscriptenWebSocketMessageEvent* websocket_event_;
public:
    void poll(int timeout) { }
    void send(const std::string& message) { }
    void sendBinary(const std::string& message) { }
    void sendBinary(const std::vector<uint8_t>& message) { 
        EMSCRIPTEN_RESULT result;
         result = emscripten_websocket_send_binary(websocket_event_->socket,message, size);
    }
    void sendPing() { }
    void close() { 
        EMSCRIPTEN_RESULT result;
        result = emscripten_websocket_close(websocket_event_->socket, 1000, "no reason");
        if (result) 
        {
            printf("Failed to emscripten_websocket_close(): %d\n", result);
        }
    } 
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

#endif