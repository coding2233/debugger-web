//
// Created by wanderer on 2023/10/29.
//

#include "app_websocket.h"

#ifdef __EMSCRIPTEN__

EM_BOOL AppWebsocket::onopen(int eventType, const EmscriptenWebSocketOpenEvent* websocketEvent, void* userData) {
    puts("onopen");
    websocket_event_ = websocketEvent;

   /* EMSCRIPTEN_RESULT result;
    result = emscripten_websocket_send_utf8_text(websocketEvent->socket, "hello world");
    if (result) {
        printf("Failed to emscripten_websocket_send_utf8_text(): %d\n", result);
    }*/
    return EM_TRUE;
}
EM_BOOL AppWebsocket::onerror(int eventType, const EmscriptenWebSocketErrorEvent* websocketEvent, void* userData) {
    puts("onerror");

    return EM_TRUE;
}
EM_BOOL AppWebsocket::onclose(int eventType, const EmscriptenWebSocketCloseEvent* websocketEvent, void* userData) {
    puts("onclose");

    return EM_TRUE;
}
EM_BOOL AppWebsocket::onmessage(int eventType, const EmscriptenWebSocketMessageEvent* websocketEvent, void* userData) {
    puts("onmessage");
    data_queue_.push(websocketEvent->data);
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


AppWebsocket::AppWebsocket()
{
#ifdef __EMSCRIPTEN__
    websocket_event_ = nullptr;
#else
    websocket_curl_ = nullptr;
#endif
}


AppWebsocket::~AppWebsocket()
{

}

int AppWebsocket::Connect(const char* websocket_address)
{
#ifdef __EMSCRIPTEN__
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
#else
    websocket_curl_ = curl_easy_init();
    //m_mediaURL填ws接口地址如 ws://xxxx.xxxx.net:8080/BlService
    curl_easy_setopt(websocket_curl_, CURLOPT_URL, websocket_address);

    // Set other options as needed (e.g. TLS/HTTPS options)
    curl_easy_setopt(websocket_curl_, CURLOPT_TIMEOUT_MS, 3000);

    // Perform a regular transfer with CONNECT_ONLY set to enable WebSocket mode
    //注意要使用websocket模式时这里要填2
    curl_easy_setopt(websocket_curl_, CURLOPT_CONNECT_ONLY, (long)2);
    CURLcode code = (CURLcode)-1;
    code = curl_easy_perform(websocket_curl_);
    if (code == CURLE_OK) {
        return 0;
    }
    else
    {
        curl_easy_cleanup(websocket_curl_);
        websocket_curl_ = NULL;
        return -1;
    }
#endif
    return 0;
}


void AppWebsocket::Close()
{
#ifdef __EMSCRIPTEN__
    if (websocket_event_)
    {
        EMSCRIPTEN_RESULT result;
        result = emscripten_websocket_close(websocket_event_->socket, 1000, "no reason");
        if (result) {
            printf("Failed to emscripten_websocket_close(): %d\n", result);
        }
    }
#else
    if (websocket_curl_)
    {
        curl_easy_cleanup(websocket_curl_);
    }
#endif
}


void AppWebsocket::Send(const void* data, size_t size)
{
#ifdef __EMSCRIPTEN__
    if (websocket_event_)
    {
        EMSCRIPTEN_RESULT result;
        result = emscripten_websocket_send_binary(websocket_event_->socket, data, size);
    }
#else
    if (websocket_curl_)
    {
        size_t sended = 0;
        CURLcode  res = curl_ws_send(websocket_curl_, data, size, &sended, 0, CURLWS_BINARY);
    }
#endif
}

int AppWebsocket::Recv(void* data,size_t size, size_t* recv_size)
{
#ifdef __EMSCRIPTEN__
    if (data_queue_.size() > 0)
    {
        data = data_queue_.pop();
        return 0;
    }
#else
    if (websocket_curl_)
    {
        size_t rlen;
        struct curl_ws_frame* meta;
        CURLcode result = curl_ws_recv(websocket_curl_, data, size, recv_size, &meta);
        if (result == CURLE_OK)
        {
            return 0;
        }
    }
#endif
    return -1;
}