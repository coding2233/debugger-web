//
// Created by EDY on 2023/9/9.
//
#include <map>
#include <exception>
#include <filesystem>
namespace stdfs = std::filesystem;

#include "app.h"

#include "information/information_window.h"
#include "log/log_window.h"
#include "file/file_window.h"
#include "inspector/inspector_window.h"
#include "terminal/terminal_window.h"
#include "profiler/profiler_window.h"
#include "app_settings.h"

std::string server_url_;

#ifdef __EMSCRIPTEN__
#include <unistd.h>
    #include <emscripten.h>
    // // 将虚拟文件系统与持久化存储同步
    // void syncFileSystem() {
    //     EM_ASM(
    //         FS.syncfs(false, function(err) {
    //             if (err) {
    //                 console.error('Failed to sync filesystem', err);
    //             } else {
    //                 console.log('Filesystem synced');
    //             }
    //         });
    //     );
    // }

    void resetFileSystem(bool reset) 
    {
        std::string config_ini = "/config/imgui.ini";
        std::string default_ini = "/data/imgui.ini";

        printf("%s exists %d\n","/config",stdfs::exists("/config"));

        std::string load_config = default_ini;     
        if(reset)
        {
            if(stdfs::exists(config_ini.c_str()))
            {
                int status = unlink(config_ini.c_str());
                if (status == 0) {
                    printf("File deleted successfully\n");
                } else {
                    perror("unlink");
                }
            }
            if(stdfs::exists(default_ini.c_str()))
            {
                load_config = default_ini;
            }
        }
        else
        {
            if(stdfs::exists(config_ini.c_str()))
            {
                load_config = config_ini;
            }
        }

        ImGui::LoadIniSettingsFromDisk(load_config.c_str());
        printf("%s exists %d\n%s exists %d\n",config_ini.c_str(),stdfs::exists(config_ini.c_str()),default_ini.c_str(),stdfs::exists(default_ini.c_str()));
    }


    extern "C" 
    { 
        EMSCRIPTEN_KEEPALIVE 
        void set_web_file_system()
        {
            resetFileSystem(false);
            // LoadAppSettings();

            // float size_pixels = GetFloatConfig("FontSize",20);
            // ImGuiIO &io = ImGui::GetIO();
            // io.Fonts->AddFontFromFileTTF("/data/SourceCodePro-Medium.ttf", size_pixels);
        }

        EMSCRIPTEN_KEEPALIVE 
        void set_web_location_host(const char* host)
        {
            server_url_ = "ws://";
            server_url_.append(std::string(host));
            // server_url_ = "ws://100.72.78.107:2233";
            puts(server_url_.c_str());
        }
    }

    
#endif

App::App():ImplApp("Debugger",1280,800,0)
{
    ImPlot::CreateContext();

    server_url_ = "ws://127.0.0.1:2233";

    windows_.insert({1,new InformationWindow()});
    windows_.insert({7,new LogWindow()});
    windows_.insert({3,new InspectorWindow()});
    windows_.insert({4,new FileWindow()});
    windows_.insert({5,new TerminalWindow()});
    windows_.insert({6,new ProfilerWindow()});

    //bind websocket send
    for (auto iter = windows_.begin();iter!= windows_.end();iter++)
    {
        iter->second->BindSend(std::bind(&App::OnWebSocketSend,this,std::placeholders::_1,std::placeholders::_2),iter->first);
    }

    ImGuiIO &io = ImGui::GetIO();
    float size_pixels = 16.0f;

#ifdef __EMSCRIPTEN__
    EM_ASM(
        var url;
        url=window.location.host;
        // alert(url);
        Module.onRuntimeInitialized = function() {
            if (!FS.analyzePath('/config').exists) {
                FS.mkdir('/config');
            }
            else
            {
                console.log("config --xxxxx--")
            }
            FS.mount(IDBFS, {}, '/config');
             // 同步从 IndexedDB 读取到内存
            FS.syncfs(true, function (err) {
                if (err) {
                    console.error('Error loading from IndexedDB', err);
                } else {
                    // 文件系统准备就绪，可以读取文件了
                    Module.ccall("set_web_file_system",null);
                    console.log("set_web_file_system")
                }
            });

            // 现在可以安全地调用导出的函数了
            const url_str = String(url);
            Module.ccall("set_web_location_host",null,['string'],[url_str]);
        };
    );

    //io.IniFilename  = "/data/imgui.ini";
    //printf("/data/imgui.ini exists %d\n",fs::exists("/data/imgui.ini"));
    //io.Fonts->AddFontFromFileTTF("/data/wqy-microhei.ttc", 14.0f,NULL,io.Fonts->GetGlyphRangesChineseSimplifiedCommon());
    io.Fonts->AddFontFromFileTTF("/data/SourceCodePro-Medium.ttf", size_pixels);
#else
    io.Fonts->AddFontFromFileTTF("SourceCodePro-Medium.ttf", size_pixels);
#endif
}

App::~App()
{
    for (auto iter = windows_.begin();iter!= windows_.end();iter++)
    {
        delete iter->second;
    }
}


void App::OnImGuiDraw()
{
    if(!CheckConnect())
    {
        ImGui::OpenPopup("Connect Modal window");
        bool connect_modal_window_open = true;
        ImGui::SetNextWindowSize(ImVec2(400,200),ImGuiCond_FirstUseEver);
        if (ImGui::BeginPopupModal("Connect Modal window", &connect_modal_window_open))
        {
            ImGui::InputText("server url",server_url_.data(),128);
            if (ImGui::Button("Connect"))
            {
                if (ConnectToServer())
                {
                    for (auto iter = windows_.begin();iter!= windows_.end();iter++)
                    {
                        if (iter->second->GetShow())
                        {
                            //重新激活，去拉取数据
                            iter->second->SetShow(true);
                        }
                    }
                }
                ImGui::CloseCurrentPopup();
            }
            ImGui::EndPopup();
        }
    }

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
                    bool show_window = iter->second->GetShow();
                    if(ImGui::MenuItem(iter->second->GetName().c_str(),NULL,&show_window))
                    {
                        iter->second->SetShow(show_window);
                    }
//                    ImGui::Separator();
                }

                ImGui::EndMenu();
            }
#ifndef __EMSCRIPTEN__
            if (ImGui::BeginMenu("Demo"))
            {
               if(ImGui::MenuItem("ImGui",NULL,&show_demo_window_))
               {
               }
                if(ImGui::MenuItem("Implot",NULL,&show_implot_demo_window_))
                {
                }
                ImGui::EndMenu();
            }
#endif


            if (ImGui::BeginMenu("Settings"))
            {
#ifdef __EMSCRIPTEN__
                if(ImGui::MenuItem("Save"))
                {
                    if(stdfs::exists("/config"))
                    {
                        std::string config_ini = "/config/imgui.ini";
                        printf("SaveIniSettingsToDisk: %s \n",config_ini.c_str());
                        ImGui::SaveIniSettingsToDisk(config_ini.c_str());

                        AppSettings::SyncFileSystem();
                    }
                }

                if(ImGui::MenuItem("Reset"))
                {
                    resetFileSystem(true);
                }

#endif
                ImGui::EndMenu();
            }

            if (ImGui::BeginMenu("Version"))
            {
                if (ImGui::BeginMenu("Server"))
                {
                    ImGui::MenuItem(server_version_.Version.c_str());
                    ImGui::EndMenu();
                }
                if (ImGui::BeginMenu("Client"))
                {
                    ImGui::MenuItem(client_version_.Version.c_str());
                    ImGui::EndMenu();
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
        iter->second->DrawWindow();
    }

    if(show_demo_window_)
    {
        ImGui::ShowDemoWindow(&show_demo_window_);
    }
    if(show_implot_demo_window_)
    {
        ImPlot::ShowDemoWindow();
    }
}

bool App::ConnectToServer()
{
    std::string url;
    int charLen = strlen(server_url_.c_str());
    url.append(server_url_.c_str(),charLen);
    return Connect(url);
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
        ws_->dispatchBinary([wsp,this](const std::vector<uint8_t> & message) {
            const int *message_size_ = (const int *)&message[0];
            int message_size = *message_size_;
            int message_sieze_o = message.size();
            uint8_t message_type = message[4];
            std::string json_mssage(message.begin()+5, message.end());
            this->DispatchMessage(message_type,json_mssage);
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


void App::DispatchMessage(uint8_t key,const std::string & message)
{
    auto window_iter = windows_.find(key);
    if (window_iter!=windows_.end())
    {
        window_iter->second->OnMessage(message);
    }
    else
    {
        if (key==0)
        {
            server_version_ = json::parse(message);
            server_version_.BuildVersion();
            AppVersion &client_version = client_version_;
            client_version.Major = 0;
            client_version.Minor = 2;
            client_version.Patch = 2;
            client_version.BuildVersion();
        }
    }
}

void App::OnWebSocketSend(uint8_t key,const std::string & message)
{
    int message_size = message.size();
    if (message_size<=0)
    {
        return;
    }
    int size = message.size()+4+1;
    std::vector<uint8_t> data;
    data.assign(message.begin(),message.end());
    message_size = data.size();
    data.insert(data.begin(),key);
    message_size = data.size();
    uint8_t *size_t = (uint8_t *)&size;
    data.insert(data.begin(),size_t,size_t+4);
    message_size = data.size();

    if(ws_ && ws_->getReadyState() != WebSocket::CLOSED)
    {
        ws_->sendBinary(data);
    }
}

void App::RunAfter()
{
    ImPlot::DestroyContext();
}
