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
  
    NLOHMANN_DEFINE_TYPE_INTRUSIVE(AppSettings,FontSize);
};

AppSettings app_settings_;

AppSettings& LoadAppSettings()
{
    FILE *fptr = fopen("/config/app_setting.json","r");
    // app_settings_ = 
    fclose(fptr);
    return app_settings_;
}

#endif //RUNTIMEDEBUGGER_APP_VERSION_H
