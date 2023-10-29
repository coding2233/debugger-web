//
// Created by EDY on 2023/9/9.
//

#include "app.h"

#include "runtime_debugger_log.pb-c.h"
#include "app_websocket.h"


static AppWebsocket* app_web_socket_;

App::App():ImplApp("",1280,800,0)
{
    app_web_socket_ = new AppWebsocket();
    app_web_socket_->Connect("ws://127.0.0.1:12233");
}

App::~App()
{
    app_web_socket_->Close();
    delete app_web_socket_;
}


void App::OnImGuiDraw()
{
    char buffer[1024];
    size_t recv_size = 0;
    int ss = app_web_socket_->Recv(&buffer,1024,&recv_size);

    ImplApp::OnImGuiDraw();
     static bool sss= true;
     if(ImGui::Begin("aaaa",&sss))
     {
         if(ImGui::Button("close"))
         {
         }
         ImGui::End();
     }

}


