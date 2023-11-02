//
// Created by wanderer on 2023/10/29.
//

#include "app_websocket.h"

AppWebsocket::AppWebsocket()
{
#ifdef __EMSCRIPTEN__
    websocket_event_ = nullptr;
#else
#ifdef _WIN32
    INT rc;
    WSADATA wsaData;

    rc = WSAStartup(MAKEWORD(2, 2), &wsaData);
    if (rc) {
        printf("WSAStartup Failed.\n");
    }
#endif
#endif
}


AppWebsocket::~AppWebsocket()
{
#ifdef __EMSCRIPTEN__
#else
#ifdef _WIN32
    WSACleanup();
#endif
  /*  if (websocket_curl_)
    {
        curl_easy_cleanup(websocket_curl_);
    }*/
#endif
}

// int AppWebsocket::Connect(const char* websocket_address)
// {
// #ifdef __EMSCRIPTEN__
//     if (!emscripten_websocket_is_supported()) 
//     {
//         return -1;
//     }
//     EmscriptenWebSocketCreateAttributes ws_attrs = {
//         websocket_address,
//         NULL,
//         EM_TRUE
//     };

//     EMSCRIPTEN_WEBSOCKET_T ws = emscripten_websocket_new(&ws_attrs);
//     emscripten_websocket_set_onopen_callback(ws, this, onopen);
//     emscripten_websocket_set_onerror_callback(ws, this, onerror);
//     emscripten_websocket_set_onclose_callback(ws, this, onclose);
//     emscripten_websocket_set_onmessage_callback(ws, this, onmessage);
// #else

//  std::unique_ptr<WebSocket> ws(WebSocket::from_url(websocket_address));


  
// #endif
//     return 0;
// }


std::unique_ptr<WebSocket> AppWebsocket::Connect(const char* websocket_address)
{
         std::unique_ptr<WebSocket> ws(WebSocket::from_url(websocket_address));
         //    std::unique_ptr<WebSocket> ws(WebSocket::from_url("ws://localhost:8126/foo"));
//     assert(ws);
//     ws->send("goodbye");
//     ws->send("hello");
//     while (ws->getReadyState() != WebSocket::CLOSED) {
//         WebSocket::pointer wsp = &*ws; // <-- because a unique_ptr cannot be copied into a lambda
//         ws->poll();
//         ws->dispatch([wsp](const std::string & message) {
//             printf(">>> %s\n", message.c_str());
//             if (message == "world") { wsp->close(); }
//         });
//     }
return ws;
}


// int AppWebsocket::Connect(const char* websocket_address,MessageCallback &message_callback)
// {
//     message_callback_ = &message_callback;
//     return 0;
// }

// void AppWebsocket::Close()
// {
// #ifdef __EMSCRIPTEN__
//     if (websocket_event_)
//     {
//         EMSCRIPTEN_RESULT result;
//         result = emscripten_websocket_close(websocket_event_->socket, 1000, "no reason");
//         if (result) {
//             printf("Failed to emscripten_websocket_close(): %d\n", result);
//         }
//     }
// #else
// #ifdef _WIN32
//     WSACleanup();
// #endif
//   /*  if (websocket_curl_)
//     {
//         curl_easy_cleanup(websocket_curl_);
//     }*/
// #endif
// }


// void AppWebsocket::Send(const void* data, size_t size)
// {
// #ifdef __EMSCRIPTEN__
//     if (websocket_event_)
//     {
//         EMSCRIPTEN_RESULT result;
//         result = emscripten_websocket_send_binary(websocket_event_->socket, (void*)data, size);
//     }
// #else
//   /*  if (websocket_curl_)
//     {
//         size_t sended = 0;
//         CURLcode  res = curl_ws_send(websocket_curl_, data, size, &sended, 0, CURLWS_BINARY);
//     }*/
// #endif
// }

// int AppWebsocket::Recv(void* data,size_t size, size_t* recv_size)
// {
// #ifdef __EMSCRIPTEN__
//     if (data_queue_.size() > 0)
//     {
//         data = data_queue_.front();
//         data_queue_.pop();
//         return 0;
//     }
// #else
//    /* if (websocket_curl_)
//     {
//         size_t rlen;
//         struct curl_ws_frame* meta;
//         CURLcode result = curl_ws_recv(websocket_curl_, data, size, recv_size, &meta);
//         if (result == CURLE_OK)
//         {
//             return 0;
//         }
//     }*/
// #endif
//     return -1;
// }

// #ifdef __EMSCRIPTEN__
// void AppWebsocket::AddData(void* data)
// {
//     data_queue_.push(data);
// }

// void AppWebsocket::SetWebSocketEvent(const EmscriptenWebSocketOpenEvent* websocket_event)
// {
//     websocket_event_ = websocket_event;
// }
// #endif