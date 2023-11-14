//
// Created by EDY on 2023/11/14.
//

#ifndef RUNTIMEDEBUGGER_HIERARCHY_NODE_H
#define RUNTIMEDEBUGGER_HIERARCHY_NODE_H

#include <string>
#include <vector>

#include "nlohmann/json.hpp"
using json = nlohmann::json;

#include "imgui.h"

class HierarchyNode
{
public:
    HierarchyNode()
    {}
    ~HierarchyNode()
    {}

public:
    int InstanceID;
    int ParentInstanceID;
    std::string Name;
    ImVec3 LocalPosition;
    ImVec3 LocalAngle;
    ImVec4 LocalQuaternion;
    ImVec3 LocalScale;
    std::string Tag;
    std::string Layer;
    bool Active;
    std::vector<HierarchyNode> ChildrenNodes;
    //    NLOHMANN_DEFINE_TYPE_INTRUSIVE(LogNode,LogTime,LogType,LogFrameCount,LogMessage,StackTrack);
};

#endif //RUNTIMEDEBUGGER_HIERARCHY_NODE_H
