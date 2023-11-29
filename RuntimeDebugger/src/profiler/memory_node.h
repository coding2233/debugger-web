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
    int MaxUsedMemory;
    long MonoUsedSize;
    long MonoHeapSize ;
    long UsedHeapSize;
    long TotalAllocatedMemory ;
    long TotalReservedMemory;
    long TotalUnusedReserved;
    long AllocatedMemoryForGraphicsDriver ;
    unsigned int TempAllocatorSize ;

    NLOHMANN_DEFINE_TYPE_INTRUSIVE(MemoryNode,FrameCount,Realtime,MaxUsedMemory,MonoUsedSize,MonoHeapSize,UsedHeapSize,TotalAllocatedMemory,TotalReservedMemory,TotalUnusedReserved,AllocatedMemoryForGraphicsDriver,TempAllocatorSize);
};

#endif //RUNTIMEDEBUGGER_MEMORY_NODE_H
