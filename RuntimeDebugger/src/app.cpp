//
// Created by EDY on 2023/9/9.
//
#include <map>
#include <exception>

#include "app.h"


#include "information/information_window.h"
#include "log/log_window.h"
#include "file/file_window.h"
#include "inspector/inspector_window.h"


App::App():ImplApp("",1280,800,0)
{
    server_url_ = "ws://127.0.0.1:2233";

    windows_.insert({0,InformationWindow()});
    windows_.insert({1,LogWindow()});
    windows_.insert({2,InspectorWindow()});
    windows_.insert({3,FileWindow()});
}

App::~App()
{
    // app_web_socket_->Close();
    // delete app_web_socket_;
}


void App::OnImGuiDraw()
{
//    if(!CheckConnect())
//    {
//        ImGui::OpenPopup("Connect Modal window");
//        bool connect_modal_window_open = true;
//        ImGui::SetNextWindowSize(ImVec2(400,200),ImGuiCond_FirstUseEver);
//        if (ImGui::BeginPopupModal("Connect Modal window", &connect_modal_window_open))
//        {
//            ImGui::InputText("server url",(char*)server_url_.c_str(),128);
//            if (ImGui::Button("Connect"))
//            {
//                ConnectToServer();
//                ImGui::CloseCurrentPopup();
//            }
//            ImGui::EndPopup();
//        }
//    }

    static bool show_information = true;
    static ImGuiDockNodeFlags dockspace_flags = ImGuiDockNodeFlags_None;
    ImGuiWindowFlags window_flags = ImGuiWindowFlags_MenuBar | ImGuiWindowFlags_NoDocking;
    const ImGuiViewport *viewport = ImGui::GetMainViewport();
    ImGui::SetNextWindowPos(viewport->WorkPos);
    ImGui::SetNextWindowSize(viewport->WorkSize);
    ImGui::SetNextWindowViewport(viewport->ID);
    ImGui::PushStyleVar(ImGuiStyleVar_WindowRounding, 0.0f);
    ImGui::PushStyleVar(ImGuiStyleVar_WindowBorderSize, 0.0f);
    window_flags |= ImGuiWindowFlags_NoTitleBar | ImGuiWindowFlags_NoCollapse | ImGuiWindowFlags_NoResize |
                    ImGuiWindowFlags_NoMove;
    window_flags |= ImGuiWindowFlags_NoBringToFrontOnFocus | ImGuiWindowFlags_NoNavFocus;

    ImGui::PushStyleVar(ImGuiStyleVar_WindowPadding, ImVec2(0.0f, 0.0f));
    if (ImGui::Begin("Information DockSpace", &show_information, window_flags))
    {
        ImGui::PopStyleVar(3);

        ImGuiIO &io = ImGui::GetIO();
        if (io.ConfigFlags & ImGuiConfigFlags_DockingEnable)
        {
            ImGuiID dockspace_id = ImGui::GetID("DebuggerDockSpace");
            ImGui::DockSpace(dockspace_id, ImVec2(0.0f, 0.0f), dockspace_flags);
        }

        if (ImGui::BeginMenuBar())
        {
            if (ImGui::BeginMenu("Window"))
            {
                for (auto iter = windows_.begin(); iter != windows_.end(); iter++)
                {
                    AppWindow & app_window = iter->second;
                    bool show_window = app_window.GetShow();
                    if(ImGui::MenuItem(app_window.GetName().c_str(),NULL,&show_window))
                    {
                        app_window.SetShow(show_window);
                    }
                    ImGui::Separator();
                }

                ImGui::EndMenu();
            }
            ImGui::EndMenuBar();
        }
    }
    ImGui::End();

    //绘制窗口
    for (auto iter = windows_.begin(); iter != windows_.end(); iter++)
    {
        AppWindow & app_window = iter->second;
        app_window.DrawWindow();
    }

}

void App::ConnectToServer()
{
    std::string url;
    int charLen = strlen(server_url_.c_str());
    url.append(server_url_.c_str(),charLen);
    Connect(url);
}

bool App::CheckConnect()
{
    if(ws_ && ws_->getReadyState() != WebSocket::CLOSED)
    {
        ws_->poll();
        WebSocket::pointer wsp = &*ws_;
////            ws_->dispatch([wsp,this](const std::string & message) {
////                this->OnMessage(message);
////            });
        ws_->dispatchBinary([wsp](const std::vector<uint8_t> & message) {
            const int *message_size_ = (const int *)&message[0];
            int message_size = *message_size_;
            int message_sieze_o = message.size();
            uint8_t message_type = message[4];
            std::string json_mssage(message.begin()+5, message.end());
            printf(json_mssage.c_str());
//                this->OnMessage(message);
        });

        return true;
    }
    return false;
}

bool App::Connect(std::string server_url)
{
    if(CheckConnect())
    {
        ws_->close();
    }
    ws_ = std::unique_ptr<WebSocket>(WebSocket::from_url(server_url.c_str()));

    return  ws_ != NULL;
}

