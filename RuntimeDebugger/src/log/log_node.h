//
// Created by EDY on 2023/11/14.
//

#ifndef RUNTIMEDEBUGGER_LOG_NODE_H
#define RUNTIMEDEBUGGER_LOG_NODE_H
#include <string>

#include "nlohmann/json.hpp"
using json = nlohmann::json;

#include "imgui.h"

static std::string log_type_str_[] = {"Error","Assert","Warning","Info","Exception"};
static ImVec4 log_type_color_[] = {ImVec4(1,0,0,1),ImVec4(1,0,0,1),ImVec4(1,1,0,1),ImVec4(1,1,1,1),ImVec4(1,0,0,1)};

class LogNode
{
private:

public:

    std::string LogTime;
    int LogType;
    long LogFrameCount;
    std::string LogMessage;
    std::string StackTrack;
    std::string SortLog;

    NLOHMANN_DEFINE_TYPE_INTRUSIVE(LogNode,LogTime,LogType,LogFrameCount,LogMessage,StackTrack);

    void BuildSortLog()
    {
        int timeStartIndex = LogTime.find("T")+1;
        int timeEndIndex = LogTime.find(".");
        int timeLen = timeEndIndex - timeStartIndex;
        std::string time = LogTime.substr(timeStartIndex,timeLen);
        SortLog.append("[");
        SortLog.append(time);
        SortLog.append("]");
        SortLog.append(" ");
        SortLog.append("[");
        SortLog.append(std::to_string(LogFrameCount));
        SortLog.append("]");
        SortLog.append(" ");
        SortLog.append("[");
        SortLog.append(log_type_str_[LogType]);
        SortLog.append("]");
        SortLog.append(" ");
        SortLog.append(LogMessage);
    }

    ImVec4 GetColor() const
    {
        auto color = log_type_color_[LogType];
        return color;
    }
};

#endif //RUNTIMEDEBUGGER_LOG_NODE_H
