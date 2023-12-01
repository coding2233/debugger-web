//
// Created by wanderer on 2023/11/22.
//

#ifndef RUNTIMEDEBUGGER_APP_SETTINGS_H
#define RUNTIMEDEBUGGER_APP_SETTINGS_H
#include <vector>
#include <string>
#include <functional>
#include <iostream>
#include <exception>
#include <fstream>
#include <map>
#include <filesystem>
namespace stdfs = std::filesystem;
#include "nlohmann/json.hpp"
using json = nlohmann::json;

#include "imgui.h"

#ifdef __EMSCRIPTEN__
    #include <emscripten.h>
    // 将虚拟文件系统与持久化存储同步
    void syncFileSystem() {
        EM_ASM(
            FS.syncfs(false, function(err) {
                if (err) {
                    console.error('Failed to sync filesystem', err);
                } else {
                    console.log('Filesystem synced');
                }
            });
        );
    }
#endif

class AppSettings
{
public:
    AppSettings()
    {}
    ~AppSettings()
    {}

public:
    float FontSize;
    std::map<std::string,float> float_data_;
    std::map<std::string,int> int_data_;
    std::map<std::string,bool> bool_data_;
    std::map<std::string,std::string> string_data_;
  
    NLOHMANN_DEFINE_TYPE_INTRUSIVE(AppSettings,float_data_,int_data_,bool_data_,string_data_);
};

const char* app_settings_json_path_ = "/config/app_setting.json";
AppSettings app_settings_;

void LoadAppSettings()
{
    if(stdfs::exists(app_settings_json_path_))
    {
        std::ifstream f(app_settings_json_path_);
        json data = json::parse(f);
        app_settings_ = data;
    }
}

void SaveAppSettings()
{
    if(stdfs::exists("/config"))
    {
        std::ofstream o(app_settings_json_path_);
        json data = app_settings_;
        o << std::setw(4) << data << std::endl;
    }

    #ifdef __EMSCRIPTEN__
    syncFileSystem();
    #endif
}

void SetFloatConfig(std::string key,float value)
{
    auto iter = app_settings_.float_data_.find(key);
    if(iter ==  app_settings_.float_data_.end())
    {
        app_settings_.float_data_.insert({key,value});
    }
    else
    {
        app_settings_.float_data_[key] = value;
    }
}

float GetFloatConfig(std::string key,float default_value)
{
    auto iter = app_settings_.float_data_.find(key);
    if(iter ==  app_settings_.float_data_.end())
    {
        return default_value;
    }
    else
    {
        return iter->second;
    }
}

#endif //RUNTIMEDEBUGGER_APP_VERSION_H
