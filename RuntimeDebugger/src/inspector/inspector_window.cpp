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
    auto req_cmd = json_rsp["Cmd"].template get<ReqInspectorCmd>();
    if (req_cmd==Req_Cmd_FindGameObjects)
    {
        if (json_rsp["FindNodes"].empty())
        {
            return;
        }
        else
        {
//            auto rsp = json_rsp.template get<RspInspector>();
            auto find_nodes = json_rsp["FindNodes"].template get<std::vector<HierarchyNode>>();
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
    }
    else if (req_cmd==Req_Cmd_FindComponent)
    {
        if (json_rsp["Components"].empty())
        {
            return;
        }
        else
        {
            auto find_components = json_rsp["Components"].template get<std::vector<CompoentInspector>>();
            int components_size = find_components.size();
            //todo value to void*
        }
    }
}

void InspectorWindow::OnShow(bool show)
{
    if (show)
    {
        ReqInspector req;
        req.Cmd = Req_Cmd_FindGameObjects;
        req.InstanceID = 0;
        json json_req = req;
        std::string message = json_req.dump();
        printf("req %s\n",message.c_str());
        Send(message);
    }
}


void InspectorWindow::OnDraw()
{
    //bool connected = CheckConnect();
    bool connected = true;
    float log_window_height = ImGui::GetWindowWidth()*0.35f;
    ImGui::BeginChild("Inspector_Child_Hierarchy",ImVec2(log_window_height,0));
    for (int i = 0;i<hierarchy_root_nodes_.size();i++)
    {
        DrawInspectorNode(hierarchy_root_nodes_[i]);
    }
    ImGui::EndChild();
    ImGui::SameLine();
    ImGui::BeginChild("Inspector_Child_Component");
    ImGui::EndChild();
}


void InspectorWindow::DrawInspectorNode(const HierarchyNode* hierarchy_node)
{
    if (!hierarchy_node)
    {
        return;
    }
    bool selected = hierarchy_node == hierarchy_node_selected_;
    ImGuiTreeNodeFlags node_flags = ImGuiTreeNodeFlags_OpenOnArrow | ImGuiTreeNodeFlags_OpenOnDoubleClick | ImGuiTreeNodeFlags_SpanFullWidth;
    bool hasChild = hierarchy_node->ChildrenNodes.size() > 0;
    if(!hasChild)
    {
        node_flags = node_flags | ImGuiTreeNodeFlags_Leaf;
    }
    if (selected)
    {
        node_flags = node_flags | ImGuiTreeNodeFlags_Selected;
    }
    bool tree_open = ImGui::TreeNodeEx(hierarchy_node->Name.c_str(),node_flags);
//    if (tree_open !=  hierarchy_node->TreeNodeOpened)
    if (!hasChild || !((HierarchyNode*)hierarchy_node)->CheckTreeNodeOpened(tree_open))
    {
        if (ImGui::IsItemClicked())
        {
            if (!selected)
            {
                hierarchy_node_selected_ = hierarchy_node;

                ReqInspector req;
                req.Cmd = Req_Cmd_FindComponent;
                req.InstanceID = hierarchy_node->InstanceID;
                json json_req = req;
                std::string message = json_req.dump();
                printf("req %s\n", message.c_str());
                Send(message);
            }
        }
    }
    if(tree_open)
    {
        if(hasChild)
        {
            for (int i= 0;i<hierarchy_node->ChildrenNodes.size();i++)
            {
                auto node = hierarchy_node->ChildrenNodes[i];
                DrawInspectorNode(node);
            }
        }
        ImGui::TreePop();
//        ImGui::Spacing();
    }

}