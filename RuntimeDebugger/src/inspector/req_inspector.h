//
// Created by wanderer on 2023/11/14.
//

#ifndef RUNTIMEDEBUGGER_REQ_INSPECTOR_H
#define RUNTIMEDEBUGGER_REQ_INSPECTOR_H

#include <string>
#include <vector>

#include "nlohmann/json.hpp"
using json = nlohmann::json;

#include "imgui.h"
#include "hierarchy_node.h"


enum ReqInspectorCmd
{
    FindGameObjects,
    FindComponent,
    Edit,
};

class ReqInspector
{
public:
    ReqInspectorCmd Cmd;
    int InstanceID;
    int ComponentInstanceID;
    NLOHMANN_DEFINE_TYPE_INTRUSIVE(ReqInspector,Cmd,InstanceID,ComponentInstanceID);
};

class RspInspector
{
public:
    ReqInspectorCmd Cmd;
    std::vector<HierarchyNode> FindNodes;
//    int ComponentInstanceID;
    NLOHMANN_DEFINE_TYPE_INTRUSIVE(RspInspector,Cmd,FindNodes);
};



#endif //RUNTIMEDEBUGGER_REQ_INSPECTOR_H
