//
// Created by wanderer on 2023/11/14.
//

#include "inspector_window.h"


InspectorWindow::InspectorWindow()
{
    name_ = "Inspector";
}

InspectorWindow::~InspectorWindow()
{}

void InspectorWindow::OnMessage(const std::string &message)
{
    json json_rsp = json::parse(message);
    if (json_rsp["Cmd"].template get<ReqInspectorCmd>()==Req_Cmd_FindGameObjects)
    {
        if (json_rsp["FindNodes"].empty())
        {
            return;
        }
    }
    auto rsp = json_rsp.template get<RspInspector>();
    auto find_nodes = rsp.FindNodes;
    for (int i = 0; i < find_nodes.size(); ++i)
    {
            const HierarchyNode find_node = find_nodes[i];
            if (map_hierarchy_nodes_.find(find_node.InstanceID) == map_hierarchy_nodes_.end())
            {
                map_hierarchy_nodes_.insert({find_node.InstanceID, find_node});
                printf("map_hierarchy_nodes_ size: %d\n",(int)map_hierarchy_nodes_.size());
            }

            const HierarchyNode& map_node_iter = map_hierarchy_nodes_.find(find_node.InstanceID)->second;

            if (map_node_iter.ParentInstanceID == 0)
            {
                bool has_node = false;
                for (int i = 0; i < hierarchy_root_nodes_.size(); i++) {
                    if (hierarchy_root_nodes_[i]->InstanceID == map_node_iter.InstanceID) {
                        has_node = true;
                        break;
                    }
                }
                if (!has_node) {
                    hierarchy_root_nodes_.push_back(&map_node_iter);
                    printf("hierarchy_root_nodes_ size: %d\n",(int)hierarchy_root_nodes_.size());
                }
            }
            else
            {
                auto find_parent_iter = map_hierarchy_nodes_.find(map_node_iter.ParentInstanceID);
                if (find_parent_iter != map_hierarchy_nodes_.end())
                {
                    find_parent_iter->second.AddChild(&map_node_iter);
                    printf("find_parent_iter %d name:%s find_id: %d size: %d\n",find_parent_iter->first,find_parent_iter->second.Name.c_str(), find_node.InstanceID,(int)find_parent_iter->second.ChildrenNodes.size());
                }
            }

        }
}



bool InspectorWindow::OnDraw()
{
    //bool connected = CheckConnect();
    bool connected = true;
    ImGui::Begin("Inspector",&show_);
    for (int i = 0;i<hierarchy_root_nodes_.size();i++)
    {
        DrawInspectorNode(hierarchy_root_nodes_[i]);
    }

    ImGui::End();
    return connected;
}


void InspectorWindow::DrawInspectorNode(const HierarchyNode* hierarchy_node)
{
    if (!hierarchy_node)
    {
        return;
    }

    bool tree_open = ImGui::TreeNode(hierarchy_node->Name.c_str());
    if (ImGui::IsItemClicked())
    {
        ReqInspector req;
        req.Cmd = Req_Cmd_FindGameObjects;
        req.InstanceID = hierarchy_node->InstanceID;
        json json_req = req;
        std::string message = json_req.dump();
        printf("req %s\n",message.c_str());
        Send(message);
    }
    if(tree_open)
    {
        if(hierarchy_node->ChildrenNodes.size()>0)
        {
            for (int i= 0;i<hierarchy_node->ChildrenNodes.size();i++)
            {
                auto node = hierarchy_node->ChildrenNodes[i];
                DrawInspectorNode(node);
            }
        }
        ImGui::TreePop();
        ImGui::Spacing();
    }

}