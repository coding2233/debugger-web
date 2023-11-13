//
// Created by EDY on 2023/9/9.
//
#include <map>
#include <exception>

#include "app.h"

// #include "runtime_debugger_log.pb-c.h"
// #include "app_websocket.h"
#include "easywsclient.hpp"
using easywsclient::WebSocket;

#include "nlohmann/json.hpp"
using json = nlohmann::json;



//std::unique_ptr<WebSocket> ws;
//std::string json_information_;

App::App():ImplApp("",1280,800,0)
{
    server_url_ = "ws://127.0.0.1:2233";
    windows_.insert({"/",new InformationWindow()});
//    map_information_.insert("/",new InformationWindow());
//
//    std::string url=server_url_;
//    url.append("/");
//    map_information_.find("/")->second->Connect(url);
//    json ex1 = json::parse(R"(
//    {
//        "pi": 3.141,
//        "happy": true
//    }
//    )");

//    std::cout << ex1.dump() << std::endl;

    //std::unique_ptr<WebSocket> ws(WebSocket::from_url("ws://127.0.0.1:12233"));
//      ws = std::unique_ptr<WebSocket>(WebSocket::from_url("ws://127.0.0.1:2233"));
//    if(ws == NULL)
//    {
//        printf("connect server error.\n");
//    }
    // app_web_socket_ = new AppWebsocket();
    // app_web_socket_->Connect("ws://127.0.0.1:12233");
    // static AppWebsocket app_websocket_;
    // ws = app_websocket_.Connect("ws://127.0.0.1:12233");
    // ws->send("goodbye");
    // ws->send("hello");
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
            if (ImGui::BeginMenu("Options"))
            {
                ImGui::MenuItem("Fullscreen");
                ImGui::MenuItem("Padding");
                ImGui::Separator();

                if (ImGui::MenuItem("Flag: NoSplit", "", (dockspace_flags & ImGuiDockNodeFlags_NoSplit) != 0)) { dockspace_flags ^= ImGuiDockNodeFlags_NoSplit; }
                if (ImGui::MenuItem("Flag: NoResize", "", (dockspace_flags & ImGuiDockNodeFlags_NoResize) != 0)) { dockspace_flags ^= ImGuiDockNodeFlags_NoResize; }
                if (ImGui::MenuItem("Flag: NoDockingInCentralNode", "", (dockspace_flags & ImGuiDockNodeFlags_NoDockingInCentralNode) != 0)) { dockspace_flags ^= ImGuiDockNodeFlags_NoDockingInCentralNode; }
                if (ImGui::MenuItem("Flag: AutoHideTabBar", "", (dockspace_flags & ImGuiDockNodeFlags_AutoHideTabBar) != 0)) { dockspace_flags ^= ImGuiDockNodeFlags_AutoHideTabBar; }
                ImGui::Separator();

                ImGui::EndMenu();
            }

            ImGui::EndMenuBar();

            if (ImGui::Button("Connect"))
            {

            }

//            static bool sss= true;
//            if(ImGui::Begin("aaaa",&sss))
//            {
//                if(json_information_.size() > 0)
//                {
//                    ImGui::Text(json_information_.c_str());
//                }
//                if(ImGui::Button("close"))
//                {
//                    // if(ws)
//                    // {
//                    //     ws->send("close");
//                    // }
//                }
//
//                if(ImGui::Button("test message"))
//                {
//
//                }
//
//
//            }
//            ImGui::End();
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
            ImGui::InputText("server url:",(char*)server_url_.c_str(),1024);
            if (ImGui::Button("Connect"))
            {
                ConnectToServer();
                ImGui::CloseCurrentPopup();
            }
            ImGui::EndPopup();
        }
    }

    // char buffer[1024];
    // size_t recv_size = 0;
    // int ss = app_web_socket_->Recv(&buffer,1024,&recv_size);
//     if(ws && ws->getReadyState() != WebSocket::CLOSED)
//     {
//         WebSocket::pointer wsp = &*ws;
//         ws->poll();
//         ws->dispatch([wsp](const std::string & message) {
//             json infomation = json::parse(message);
//             printf(">>> %s\n", message.c_str());
//             json_information_ = message;
//             // if (message == "world") { wsp->close();
//         });
//     }
//
//    ImplApp::OnImGuiDraw();

}

void App::ConnectToServer()
{
    for (auto iter = windows_.begin(); iter != windows_.end(); iter++)
    {
        bool connected = iter->second->CheckConnect();
        if(!connected)
        {
            std::string url;
            url.append(server_url_);
            url.append(iter->first);
            iter->second->Connect(url);
        }
    }
}


