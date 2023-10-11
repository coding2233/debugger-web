//#if _WIN32
//#pragma comment(linker, "/subsystem:\"windows\" /entry:\"mainCRTStartup\"")
//#endif


#include <Windows.h>
#include <iostream>
#include <sstream>
#include <string.h>
#include <list>
#include <filesystem>
#include <fstream>
#include <map>
#include <thread>

#include <stdlib.h>
#include <curl/curl.h>

#define STB_IMAGE_IMPLEMENTATION
#include "imgui_impl_sdl_opengl3.h"

//#pragma comment(linker, "/entry:\"WinMainCRTStartup\"")
//#pragma comment(linker, "/subsystem:\"console\" /entry:\"WinMainCRTStartup\"")

//下载根链接
#define DWONLOAD_URL "https://gunfire-res.oss-cn-chengdu.aliyuncs.com/renzaijianghu/dev/standalonewindows64/"
#define EXE_PATH "rzjh/"
#define EXE_ASSET_PATH "assets.txt"
#define EXE_NAME "rzjh\\Strategy2D.exe"

std::list<std::string> s_showLogs;
std::map<std::string,float> s_need_download_assets;
std::string s_remote_assets_text;

ImGuiContext* OnImGuiInit()
{
   return CreateImGuiContext();
}

void SaveAsset()
{
    std::string asset_path;
    asset_path.append(EXE_PATH);
    asset_path.append(EXE_ASSET_PATH);
    if (std::filesystem::exists(asset_path))
    {
        std::filesystem::remove(asset_path);
    }

    FILE* fp = fopen(asset_path.c_str(), "wb");
    const char* remote_text_c_str = s_remote_assets_text.c_str();
    size_t return_size = fwrite(remote_text_c_str, s_remote_assets_text.size(), 1, fp);
    fclose(fp);
}

void OpenProgram()
{
    if (std::filesystem::exists(EXE_NAME))
    {
        ShellExecute(NULL, "open", EXE_NAME, NULL, NULL, SW_SHOW);
        exit(0);
    }

}

void OnImGuiDraw()
{
    ShowImGuiDemoWindow();

    if (ImGui::Begin("rzjh logs"))
    {
        for (auto iter = s_showLogs.begin();iter != s_showLogs.end();iter++)
        {
            ImGui::Text((*iter).c_str());
        }
    }
    ImGui::End();

    float download_complete_count = 0;
    float download_count = 0;

    if (!s_showLogs.empty() && s_need_download_assets.size()>0)
    {
        if (ImGui::Begin("rzjh download"))
        {
            download_count = (float)s_need_download_assets.size();

            for (auto iter = s_need_download_assets.begin();iter != s_need_download_assets.end();iter++)
            {
                ImGui::Text((*iter).first.c_str());
                float process = (*iter).second;
                ImGui::ProgressBar(process);

                if (process >= 1.0f)
                {
                    download_complete_count++;
                }
            }

         
        }
        ImGui::End();
    }

    ImGui::SetNextWindowSize(ImVec2(300,180), ImGuiCond_FirstUseEver);
    if (ImGui::Begin("rzjh menu"))
    {
        if (download_count > 0)
        {
            ImGui::Text("Download progress");
            ImGui::SameLine();
            float progress = download_complete_count / download_count;
            ImGui::ProgressBar(progress);

            if (progress >= 1.0f)
            {
                if (ImGui::Button("Save asset & open program"))
                {
                    SaveAsset();
                    OpenProgram();
                }
            }
        }

        if (ImGui::Button("Exit current program"))
        {
            exit(0);
        }
    }
    ImGui::End();
}

 //判断文件是否存在
size_t write_data(char* ptr, size_t size, size_t nmemb, void* userdata) {
    std::ostringstream* stream = (std::ostringstream*)userdata;
    size_t count = size * nmemb;
    stream->write(ptr, count);
    return count;
}

int checkRemoteRuleFileExist(const char* URL, std::string &content)
 {
     std::ostringstream str_stream;
     CURL * curl_fd = curl_easy_init();
     CURLcode code = (CURLcode) - 1;
     CURLINFO response_code = (CURLINFO)0;
     //curl_easy_setopt(curl_fd, CURLOPT_NOBODY, 1);
     curl_easy_setopt(curl_fd, CURLOPT_TIMEOUT, 3);
     curl_easy_setopt(curl_fd, CURLOPT_URL, URL);
     curl_easy_setopt(curl_fd, CURLOPT_WRITEFUNCTION, write_data);
     curl_easy_setopt(curl_fd, CURLOPT_WRITEDATA, &str_stream);
     code = curl_easy_perform(curl_fd);

     if (code == CURLE_OK) 
     {
         curl_easy_getinfo(curl_fd, CURLINFO_RESPONSE_CODE, &response_code);
         content = str_stream.str();
     }

     curl_easy_cleanup(curl_fd);

     return response_code;
 }

std::map<std::string, std::string> GetAssetMD5Map(std::string assets)
{
    std::map<std::string, std::string> asset_map;

    if (!assets.empty())
    {
        std::string key;
        std::string value;
        int read_index = 0;

        std::istringstream input;
        input.str(assets);
        for (std::string line; std::getline(input, line); )
        {
            if (line.empty())
            {
                break;
            }

            if (read_index % 2 == 0)
            {
                key = line;
            }
            else
            {
                value = line;
                asset_map.insert({ key, value });
            }

            read_index++;

        }
    }
    return asset_map;
}

//判断文件是否存在
size_t write_file_data(char* ptr, size_t size, size_t nmemb, void* userdata) {
    FILE* fp = (FILE*)userdata;
    size_t return_size = fwrite(ptr, size, nmemb, fp);
    return return_size;
}

int download_progress_callback(void* clientp, double dltotal, double dlnow, double ultotal, double ulnow)
{
    const char* file_name = (const char*)clientp;
    if (dltotal > 0)
    {
        float nPersent = dlnow / dltotal;
        std::string key = file_name;
        if (s_need_download_assets.find(key) != s_need_download_assets.end())
        {
            s_need_download_assets[key] = nPersent;
        }
    }

    return 0;
}


int download_asset_file(const char* file_name)
{
    std::string download_url;
    download_url.append(DWONLOAD_URL);
    download_url.append(file_name);

    std::string download_path;
    download_path.append(EXE_PATH);
    download_path.append(file_name);

    s_showLogs.push_back("downloading:");
    s_showLogs.push_back(download_url);

    s_showLogs.push_back("write to:");
    s_showLogs.push_back(download_path);

    //这里要创建文件夹
    std::filesystem::path download_dir_path = std::filesystem::path{ download_path }.parent_path();
    if (!std::filesystem::exists(download_dir_path))
    {
        std::filesystem::create_directories(download_dir_path);
    }

    //删除文件
    if (std::filesystem::exists(download_path))
    {
        std::filesystem::remove(download_path);
    }

    FILE* fp = fopen(download_path.c_str(), "wb");

    CURL* curl_fd = curl_easy_init();
    CURLcode code = (CURLcode)-1;
    CURLINFO response_code = (CURLINFO)0;
    //curl_easy_setopt(curl_fd, CURLOPT_NOBODY, 1);
    curl_easy_setopt(curl_fd, CURLOPT_TIMEOUT, 300);
    //char* curl_download_url = curl_escape(download_url.c_str(), download_url.size());
    for (int i = 0; i < download_url.size(); i++) {
        if (download_url[i] == ' ') {
            download_url.replace(i, 1, "%20");
        }
    }
    curl_easy_setopt(curl_fd, CURLOPT_URL, download_url.c_str());
    curl_easy_setopt(curl_fd, CURLOPT_WRITEFUNCTION, write_file_data);
    curl_easy_setopt(curl_fd, CURLOPT_WRITEDATA, fp);

    curl_easy_setopt(curl_fd, CURLOPT_PROGRESSFUNCTION, download_progress_callback);
    curl_easy_setopt(curl_fd, CURLOPT_PROGRESSDATA, file_name);
    curl_easy_setopt(curl_fd, CURLOPT_NOPROGRESS, 0L);
    
    code = curl_easy_perform(curl_fd);

    if (code == CURLE_OK)
    {
        curl_easy_getinfo(curl_fd, CURLINFO_RESPONSE_CODE, &response_code);
    }
    else
    {
        s_showLogs.push_back("download fail.");
    }

    curl_easy_cleanup(curl_fd);
    fclose(fp);

    return response_code;
}

void download_thread_task()
{
    for (auto iter = s_need_download_assets.begin();iter != s_need_download_assets.end();iter++)
    {
        const char* file_name = (*iter).first.c_str();
        download_asset_file(file_name);
    }

    s_showLogs.push_back("all file download compolete.");
}

//int WINAPI WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, PSTR lpCmdLine, int nCmdShow) 
//{
//    static bool main_exit = true;
//    //curl_global_init(CURL_GLOBAL_ALL);
//
//    // std::string assets_url;
//    // assets_url.append(DWONLOAD_URL);
//    // assets_url.append("assets.txt");
//
//    // s_showLogs.push_back(assets_url);
//
//    // std::string &content = std::string();
//    // if(checkRemoteRuleFileExist(assets_url.c_str(), content) != 200)
//    // {
//    //     s_showLogs.push_back("assets.txt not exist!");
//    //     if (!content.empty())
//    //     {
//    //         s_showLogs.push_back(content);
//    //         content = "";
//    //     }
//    // }
//    // else
//    // {
//    //     s_showLogs.push_back("find assets.txt!");
//    //     s_showLogs.push_back(content);
//    // }
//
//    // if (!std::filesystem::exists(EXE_PATH))
//    // {
//    //     std::filesystem::create_directory(EXE_PATH);
//    // }
//
//    // std::string localAssets;
//    // std::string localAssetsPath;
//    // localAssetsPath.append(EXE_PATH);
//    // localAssetsPath.append("assets.txt");
//
//    // if (std::filesystem::exists(localAssetsPath))
//    // {
//    //     std::ifstream assets_stream;
//    //     assets_stream.open(localAssetsPath);
//    //     std::string localAssetLine;
//    //     
//    //     while (std::getline(assets_stream, localAssetLine))
//    //     {
//    //         localAssets.append(localAssetLine);
//    //         localAssets.append("\n");
//    //     }
//    //     assets_stream.close();
//    // }
//
//    // std::map<std::string, std::string> localAssetsMap = GetAssetMD5Map(localAssets);
//    // std::map<std::string, std::string> remoteAssetsMap = GetAssetMD5Map(content);
//
//    // //远程访问失败，
//    // if (remoteAssetsMap.empty())
//    // {
//    //     if (localAssetsMap.empty())
//    //     {
//    //         s_showLogs.push_back("get remote asset fail. get local asset fail.");
//    //     }
//    //     //但是本地有程序,直接运行
//    //     else
//    //     {
//    //        //  main_exit = true;
//    //        OpenProgram();
//    //     }
//    // }
//    // else
//    // {
//    //     s_remote_assets_text = content.c_str();
//    //     //全部下载
//    //     if (localAssetsMap.empty())
//    //     {
//    //         for (const auto& [key01, value01] : remoteAssetsMap)
//    //         {
//    //             s_need_download_assets.insert({ key01 ,0 });
//    //         }
//    //     }
//    //     //下载更新的内容
//    //     else
//    //     {
//    //         for (const auto& [key, value] : remoteAssetsMap)
//    //         {
//    //             auto search = localAssetsMap.find(key);
//    //             if (search != localAssetsMap.end())
//    //             {
//    //                 if (search->second != value)
//    //                 {
//    //                     s_need_download_assets.insert({ key ,0 });
//    //                 }
//    //             }
//    //             else
//    //             {
//    //                 s_need_download_assets.insert({ key ,0 });
//    //             }
//    //         }
//    //     }
//
//    //     //开始下载文件
//    //     if (s_need_download_assets.empty())
//    //     {
//    //         OpenProgram();
//    //     }
//    //     else
//    //     {
//    //         std::thread t(download_thread_task);
//    //         //t.join();
//    //         t.detach();
//    //     }
//    // }
//std::cout << "hello world" << std::endl;
//     //SDL_WINDOW_BORDERLESS
//    auto sdl_window= CreateSdlWindow("rzjh launch window",1280,800,0);
//    CreateRender(sdl_window,OnImGuiInit,OnImGuiDraw,nullptr,&main_exit);
//    return 0;
//}

 int main(int argc,char *args[])
 {
     std::cout<< "Hello test main"<<std::endl;   
     static bool main_exit = true;

     // CreateSdlWindow("test windiw",0,0,)
     auto sdl_window= CreateSdlWindow("test window",1280,800,0);
     CreateRender(sdl_window,OnImGuiInit,OnImGuiDraw,nullptr,&main_exit);

     return 0;
 }
