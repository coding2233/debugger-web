//
// Created by EDY on 2023/11/30.
//

#ifndef RUNTIMEDEBUGGER_MEMORY_SAMPLE_NODE_H
#define RUNTIMEDEBUGGER_MEMORY_SAMPLE_NODE_H

#include <string>

#include "nlohmann/json.hpp"
using json = nlohmann::json;

class MemorySampleNode
{
public:
    MemorySampleNode()
    {}
    ~MemorySampleNode()
    {}
private:
public:
    std::string Name;
    std::string Type;
    float Size;

    NLOHMANN_DEFINE_TYPE_INTRUSIVE(MemorySampleNode,Name,Type,Size);
};

class MemorySampleProfiler
{
public:
    MemorySampleProfiler()
    {}
    ~MemorySampleProfiler()
    {}
private:
public:
    std::string DateTime;
    int FrameCount;
    float Realtime;
    std::vector<MemorySampleNode> Nodes;

    NLOHMANN_DEFINE_TYPE_INTRUSIVE(MemorySampleProfiler,DateTime,FrameCount,Realtime,Nodes);
};

#endif //RUNTIMEDEBUGGER_MEMORY_SAMPLE_NODE_H
