//
// Created by EDY on 2023/11/28.
//

#ifndef RUNTIMEDEBUGGER_TERMINAL_MESSAGE_H
#define RUNTIMEDEBUGGER_TERMINAL_MESSAGE_H

#include <string>

#include "nlohmann/json.hpp"
using json = nlohmann::json;

#include "imgui.h"

class TerminalMessage
{
public:
    TerminalMessage()
    {}
    ~TerminalMessage()
    {}
public:
    std::string Command;
    std::string Result;
    int ResultCode;
    NLOHMANN_DEFINE_TYPE_INTRUSIVE(TerminalMessage,Command,Result,ResultCode);
};

#endif //RUNTIMEDEBUGGER_TERMINAL_MESSAGE_H
