//
// Created by EDY on 2023/11/29.
//

#ifndef RUNTIMEDEBUGGER_FPS_NODE_H
#define RUNTIMEDEBUGGER_FPS_NODE_H

#include <string>

#include "nlohmann/json.hpp"
using json = nlohmann::json;

class FPSNode
{
public:
    FPSNode()
    {}
    ~FPSNode()
    {}
public:
    int FrameCount;
    double Realtime;
    double FPS;
    int FrameTime;
    double AvgFPS;
    double AvgFPSMin;
    double AvgFPSMax;

    NLOHMANN_DEFINE_TYPE_INTRUSIVE(FPSNode,FrameCount,Realtime,FPS,FrameTime,AvgFPS,AvgFPSMin,AvgFPSMax);
};

#endif //RUNTIMEDEBUGGER_FPS_NODE_H
