//
// Created by EDY on 2023/9/9.
//

#include "app.h"

// #include "runtime_debugger_log.pb-c.h"
#include "app_websocket.h"


static std::unique_ptr<WebSocket> ws;

App::App():ImplApp("",1280,800,0)
{
    // app_web_socket_ = new AppWebsocket();
    // app_web_socket_->Connect("ws://127.0.0.1:12233");
    static AppWebsocket app_websocket_;
    ws = app_websocket_.Connect("ws://127.0.0.1:12233");
    ws->send("goodbye");
    ws->send("hello");
}

App::~App()
{
    // app_web_socket_->Close();
    // delete app_web_socket_;
}


void App::OnImGuiDraw()
{
    // char buffer[1024];
    // size_t recv_size = 0;
    // int ss = app_web_socket_->Recv(&buffer,1024,&recv_size);
    if(ws && ws->getReadyState() != WebSocket::CLOSED)
    {
        WebSocket::pointer wsp = &*ws; 
        ws->poll();
        ws->dispatch([wsp](const std::string & message) {
            printf(">>> %s\n", message.c_str());
            // if (message == "world") { wsp->close();
        });
    }

    ImplApp::OnImGuiDraw();
     static bool sss= true;
     if(ImGui::Begin("aaaa",&sss))
     {
         if(ImGui::Button("close"))
         {
            if(ws)
            {
                ws->send("close");
            }
         }

        if(ImGui::Button("test message"))
        {
            // RuntimeDebugger__LogNode log_node = RUNTIME_DEBUGGER__LOG_NODE__INIT;
            // log_node.log_message = "test log_node.log_message";
            // log_node.log_stack_track = "test log_node.log_stack_track";
            // log_node.log_frame_count = 10000;
            // log_node.log_time = "999999";
            // size_t len = runtime_debugger__log_node__get_packed_size(&log_node);

            // std::vector<uint8_t> message(1024*10);
            // size_t size = runtime_debugger__log_node__pack(&log_node,message.data());
            // message.resize(size);
            // size_t len = runtime_debugger__log_node__get_packed_size(&log_node);
            // const std::vector<uint8_t> &send_message = message;
            if(ws)
            {
                // ws->sendBinary(message);
            }
        }

         ImGui::End();
     }

}


