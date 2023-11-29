//
// Created by EDY on 2023/11/29.
//

#ifndef RUNTIMEDEBUGGER_MEMORY_NODE_H
#define RUNTIMEDEBUGGER_MEMORY_NODE_H


#include <string>

#include "nlohmann/json.hpp"
using json = nlohmann::json;

class MemoryNode
{
public:
    MemoryNode()
    {}
    ~MemoryNode()
    {}
public:
    int FrameCount;
    float Realtime;
    float MaxUsedMemory;
    float MonoUsedSize;
    float MonoHeapSize ;
    float UsedHeapSize;
    float TotalAllocatedMemory;
    float TotalReservedMemory;
    float TotalUnusedReserved;
    float AllocatedMemoryForGraphicsDriver;
    float TempAllocatorSize ;

    NLOHMANN_DEFINE_TYPE_INTRUSIVE(MemoryNode,FrameCount,Realtime,MaxUsedMemory,MonoUsedSize,MonoHeapSize,UsedHeapSize,TotalAllocatedMemory,TotalReservedMemory,TotalUnusedReserved,AllocatedMemoryForGraphicsDriver,TempAllocatorSize);
};

#endif //RUNTIMEDEBUGGER_MEMORY_NODE_H
