//
// Created by wanderer on 2023/11/14.
//

#ifndef RUNTIMEDEBUGGER_INSPECTOR_WINDOW_H
#define RUNTIMEDEBUGGER_INSPECTOR_WINDOW_H

#include <map>
#include <string>
#include <vector>

#include "app_window.h"
#include "hierarchy_node.h"
#include "req_inspector.h"

class InspectorWindow :public AppWindow
{
public:
    InspectorWindow();
    ~InspectorWindow();
private:
    std::vector<const HierarchyNode*> hierarchy_root_nodes_;
    std::map<int,HierarchyNode> map_hierarchy_nodes_;

    void DrawInspectorNode(const HierarchyNode* hierarchy_node);
public:
    void OnMessage(const std::string &message) override;
    bool OnDraw() override;
};


#endif //RUNTIMEDEBUGGER_INSPECTOR_WINDOW_H
