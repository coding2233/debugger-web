//
// Created by EDY on 2023/11/30.
//

#ifndef RUNTIMEDEBUGGER_REQ_PROFILER_H
#define RUNTIMEDEBUGGER_REQ_PROFILER_H

#include <string>
#include <vector>

#include "nlohmann/json.hpp"
using json = nlohmann::json;

#include "imgui.h"

enum ReqProfilerCmd
{
    ReqProfilerCmd_GetMemorySampleTypes,
    ReqProfilerCmd_TaskMemorySample,
};

class ReqProfilerMessage
{
public:
    ReqProfilerMessage()
    {}
    ~ReqProfilerMessage()
    {}
private:

public:
    ReqProfilerCmd CmdId;
    std::string SmapleType;

    NLOHMANN_DEFINE_TYPE_INTRUSIVE(ReqProfilerMessage,CmdId,SmapleType);
};

//class RspProfilerMessage
//{
//public:
//    RspProfilerMessage()
//    {}
//    ~RspProfilerMessage()
//    {}
//private:
//
//public:
//    ReqProfilerCmd CmdId;
//    std::string SmapleType;
//    std::vector<std::string> SmapleTypes;
//
//    NLOHMANN_DEFINE_TYPE_INTRUSIVE(RspProfilerMessage,CmdId,SmapleType);
//};


#endif //RUNTIMEDEBUGGER_REQ_PROFILER_H
