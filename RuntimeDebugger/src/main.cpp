//
// Created by EDY on 2023/8/16.
//

#include <iostream>
#include <stdio.h>


#ifdef __EMSCRIPTEN__
#include <emscripten.h>
#include <emscripten/websocket.h>
#endif

#define STB_IMAGE_IMPLEMENTATION

#include "imgui_impl_sdl2_opengl3_app.h"
#include "app.h"


EM_BOOL onopen(int eventType, const EmscriptenWebSocketOpenEvent *websocketEvent, void *userData) {
    puts("onopen");

    EMSCRIPTEN_RESULT result;
    result = emscripten_websocket_send_utf8_text(websocketEvent->socket, "hello world");
    if (result) {
        printf("Failed to emscripten_websocket_send_utf8_text(): %d\n", result);
    }
    return EM_TRUE;
}
EM_BOOL onerror(int eventType, const EmscriptenWebSocketErrorEvent *websocketEvent, void *userData) {
    puts("onerror");

    return EM_TRUE;
}
EM_BOOL onclose(int eventType, const EmscriptenWebSocketCloseEvent *websocketEvent, void *userData) {
    puts("onclose");

    return EM_TRUE;
}
EM_BOOL onmessage(int eventType, const EmscriptenWebSocketMessageEvent *websocketEvent, void *userData) {
    puts("onmessage");
    if (websocketEvent->isText) {
        // For only ascii chars.
        printf("message: %s\n", websocketEvent->data);
    }

    EMSCRIPTEN_RESULT result;
    result = emscripten_websocket_close(websocketEvent->socket, 1000, "no reason");
    if (result) {
        printf("Failed to emscripten_websocket_close(): %d\n", result);
    }
    return EM_TRUE;
}


App app;

void MainLoop()
{
    app.Run();
}

int main(int argc,char* args[])
{
    std::cout<< "Hello world"<<std::endl;

    if (!emscripten_websocket_is_supported()) {
        return 0;
    }
    EmscriptenWebSocketCreateAttributes ws_attrs = {
        "ws://124.222.224.186:8800",
        NULL,
        EM_TRUE
    };

    EMSCRIPTEN_WEBSOCKET_T ws = emscripten_websocket_new(&ws_attrs);
    emscripten_websocket_set_onopen_callback(ws, NULL, onopen);
    emscripten_websocket_set_onerror_callback(ws, NULL, onerror);
    emscripten_websocket_set_onclose_callback(ws, NULL, onclose);
    emscripten_websocket_set_onmessage_callback(ws, NULL, onmessage);


    app.RunBefore();
#ifdef __EMSCRIPTEN__
    emscripten_set_main_loop(MainLoop, 0, 1);
#else
    app.RunLoop();
#endif
    app.RunAfter();
    return 0;
}
