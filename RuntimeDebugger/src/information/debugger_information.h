//
// Created by wanderer on 2023/11/13.
//

#ifndef RUNTIMEDEBUGGER_DEBUGGER_INFORMATION_H
#define RUNTIMEDEBUGGER_DEBUGGER_INFORMATION_H

#include <map>
#include <string>

#include "nlohmann/json.hpp"
using json = nlohmann::json;

class DebuggerInformation
{
public:
    DebuggerInformation()
    {}
    ~DebuggerInformation()
    {}

    std::map<std::string,std::string> information;

    NLOHMANN_DEFINE_TYPE_NON_INTRUSIVE(information);
};

#endif //RUNTIMEDEBUGGER_DEBUGGER_INFORMATION_H
