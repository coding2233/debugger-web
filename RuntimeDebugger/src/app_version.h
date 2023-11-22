//
// Created by wanderer on 2023/11/22.
//

#ifndef RUNTIMEDEBUGGER_APP_VERSION_H
#define RUNTIMEDEBUGGER_APP_VERSION_H
#include <vector>
#include <string>
#include <functional>
#include <iostream>
#include <exception>

#include "nlohmann/json.hpp"
using json = nlohmann::json;

#include "imgui.h"

class AppVersion
{
public:
    AppVersion()
    {}
    ~AppVersion()
    {}

public:
    int Major;
    int Minor;
    int Patch;

    std::string Version;
    void BuildVersion()
    {
        Version.append(std::to_string(Major));
        Version.append(".");
        Version.append(std::to_string(Minor));
        Version.append(".");
        Version.append(std::to_string(Patch));
    }
    NLOHMANN_DEFINE_TYPE_INTRUSIVE(AppVersion,Major,Minor,Patch);
};

#endif //RUNTIMEDEBUGGER_APP_VERSION_H
