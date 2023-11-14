//
// Created by wanderer on 2023/11/14.
//

#include "inspector_window.h"


InspectorWindow::InspectorWindow()
{}

InspectorWindow::~InspectorWindow()
{}

void InspectorWindow::OnMessage(const std::string &message)
{
    json json_rsp = json::parse(message);
    auto rsp = json_rsp.template get<RspInspector>();
    auto find_nodes = rsp.FindNodes;
    for (int i = 0; i < find_nodes.size(); ++i)
    {
        auto find_node = find_nodes[i];
        if (map_hierarchy_nodes_.find(find_node.InstanceID)==map_hierarchy_nodes_.end())
        {
            map_hierarchy_nodes_.insert({find_node.InstanceID,find_node});
        }

        if (find_node.ParentInstanceID == 0)
        {
            bool has_node = false;
            for (int i=0;i< hierarchy_root_nodes_.size();i++)
            {
                if (hierarchy_root_nodes_[i].InstanceID == find_node.InstanceID)
                {
                    has_node = true;
                    break;
                }
            }
            if (!has_node)
            {
                hierarchy_root_nodes_.push_back(find_node);
            }
        }
        else
        {
            auto find_parent_iter = map_hierarchy_nodes_.find(find_node.ParentInstanceID);
            if (find_parent_iter!=map_hierarchy_nodes_.end())
            {
                find_parent_iter->second.AddChild(find_node);
            }
        }

    }

}


bool InspectorWindow::OnDraw()
{
    bool connected = CheckConnect();
    ImGui::Begin("Inspector",&show_);
//    if (message_test_.size()>0)
//    {
//        for (int i= 0;i<message_test_.size();i++)
//        {
//            ImGui::Text(message_test_[i].c_str());
//        }
//    }
    ImGui::End();
    return connected;
}