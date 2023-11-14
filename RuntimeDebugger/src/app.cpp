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
    windows_.insert({"/",new InformationWindow()});
    //windows_.insert({"/log",new LogWindow()});

    menu_windows_.insert({"/log",new LogWindow()});
    menu_windows_.insert({"/file",new FileWindow()});
    menu_windows_.insert({"/inspector",new InspectorWindow()});
}

App::~App()
{
    // app_web_socket_->Close();
    // delete app_web_socket_;
}


void App::OnImGuiDraw()
{
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
                for (auto iter = menu_windows_.begin(); iter != menu_windows_.end(); iter++)
                {
                    if(ImGui::MenuItem(iter->first.c_str()))
                    {
                        std::string url;
                        int charLen = strlen(server_url_.c_str());
                        url.append(server_url_.c_str(),charLen);
                        url.append(iter->first);
                        iter->second->Connect(url);

                        windows_.insert(make_pair(iter->first,iter->second));
                        menu_windows_.erase(iter);
                    }
                    ImGui::Separator();
                }

                ImGui::EndMenu();
            }
            ImGui::EndMenuBar();
        }
    }
    ImGui::End();

    bool allConnected = true;
    for (auto iter = windows_.begin(); iter != windows_.end(); iter++)
    {
        bool connected = iter->second->OnDraw();
        allConnected = allConnected && connected;
    }

    if (!allConnected)
    {
        ImGui::OpenPopup("Connect Modal window");

        bool connect_modal_window_open = true;
        ImGui::SetNextWindowSize(ImVec2(400,200),ImGuiCond_FirstUseEver);
        if (ImGui::BeginPopupModal("Connect Modal window", &connect_modal_window_open))
        {
            ImGui::InputText("server url",(char*)server_url_.c_str(),128);
            if (ImGui::Button("Connect"))
            {
                ConnectToServer();
                ImGui::CloseCurrentPopup();
            }
            ImGui::EndPopup();
        }
    }
}

void App::ConnectToServer()
{
    for (auto iter = windows_.begin(); iter != windows_.end(); iter++)
    {
        bool connected = iter->second->CheckConnect();
        if(!connected)
        {
            std::string url;
            int charLen = strlen(server_url_.c_str());
            url.append(server_url_.c_str(),charLen);
            url.append(iter->first);
            iter->second->Connect(url);
        }
    }
}


