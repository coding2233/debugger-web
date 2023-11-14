//
// Created by EDY on 2023/11/14.
//

#ifndef RUNTIMEDEBUGGER_LOG_NODE_H
#define RUNTIMEDEBUGGER_LOG_NODE_H
#include <string>

#include "nlohmann/json.hpp"
using json = nlohmann::json;

class LogNode
{
public:
    std::string LogTime;
    int LogType;
    long LogFrameCount;
    std::string LogMessage;
    std::string StackTrack;

    NLOHMANN_DEFINE_TYPE_INTRUSIVE(LogNode,LogTime,LogType,LogFrameCount,LogMessage,StackTrack);
};

#endif //RUNTIMEDEBUGGER_LOG_NODE_H
