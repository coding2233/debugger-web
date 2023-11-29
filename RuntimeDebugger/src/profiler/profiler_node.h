//
// Created by EDY on 2023/11/29.
//

#ifndef RUNTIMEDEBUGGER_PROFILER_NODE_H
#define RUNTIMEDEBUGGER_PROFILER_NODE_H

#include <string>

#include "nlohmann/json.hpp"
using json = nlohmann::json;

#include "fps_node.h"

class ProfilerNode
{
public:
    ProfilerNode()
    {}
    ~ProfilerNode()
    {}

public:
    FPSNode FPS;
    NLOHMANN_DEFINE_TYPE_INTRUSIVE(ProfilerNode,FPS);
};

#endif //RUNTIMEDEBUGGER_PROFILER_NODE_H
