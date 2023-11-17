//
// Created by wanderer on 2023/11/14.
//

#include <iostream>
#include <exception>
#include "inspector_window.h"


InspectorWindow::InspectorWindow()
{
    name_ = "Inspector";
    hierarchy_node_selected_ = nullptr;
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
        map_components_.clear();
        auto json_rsp_components = json_rsp["Components"];
        if (json_rsp_components.empty())
        {
            return;
        }
        else
        {
            int instanceID = json_rsp["InstanceID"].template get<int>();
            if (!hierarchy_node_selected_ || instanceID!=hierarchy_node_selected_->InstanceID)
            {
                return;
            }
            auto find_components = json_rsp_components.template get<std::vector<CompoentInspector>>();
            int components_size = find_components.size();
            //todo value to void*
            if (components_size>0)
            {
                for (int i=0;i<components_size;i++)
                {
                    int component_id = find_components[i].InstanceID;
                    if (map_components_.find(component_id) != map_components_.end())
                    {
                        continue;
                    }
                    map_components_.insert({component_id, find_components[i]});
                    const CompoentInspector* compoent_inspector = &(map_components_.find(component_id)->second);
                    auto json_rsp_reflection = json_rsp_components[i]["ReflectionValues"];
                    for (int j = 0; j < compoent_inspector->ReflectionValues.size(); ++j)
                    {
                        auto json_rsp_reflection_value = json_rsp_reflection[j];
                        auto value_json = json_rsp_reflection_value["Value"];

                        ReflectionInspector* reflection_value = (ReflectionInspector*)&(compoent_inspector->ReflectionValues[j]);
                        try
                        {
                            reflection_value->Value.ToData(reflection_value->ValueType, value_json);
                        }
                        catch(std::exception& e)
                        {
                            std::cout << "Standard exception: " << e.what() << "  json: "<< json_rsp_reflection_value.dump() << std::endl;
                        }
                        //printf("%s %s\n",compoent_inspector.ReflectionValues[j].ValueType.c_str(),value_json.dump());
                    }
                }
            }
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

    bool node_selected = hierarchy_node_selected_;

    float log_window_height = node_selected? ImGui::GetWindowWidth()*0.35f:0;
    if(ImGui::BeginChild("Inspector_Child_Hierarchy",ImVec2(log_window_height,0),true))
    {
        for (int i = 0; i < hierarchy_root_nodes_.size(); i++)
        {
            DrawInspectorNode(hierarchy_root_nodes_[i]);
        }
    }
    ImGui::EndChild();
    if (node_selected)
    {
        ImGui::SameLine();
        if(ImGui::BeginChild("Inspector_Child_Component"))
        {
            float  one_third_width = ImGui::GetWindowWidth() * 0.3f;
            //gameObject
            ImGui::Checkbox(hierarchy_node_selected_->Name.c_str(),&(hierarchy_node_selected_->Active));
            ImGui::SetNextItemWidth(one_third_width);
            ImGui::InputText("Tag", hierarchy_node_selected_->Tag.data(),128);
            ImGui::SameLine();
            ImGui::SetNextItemWidth(one_third_width);
            ImGui::InputText("Layer", hierarchy_node_selected_->Layer.data(),128);

            if (map_components_.size() > 0)
            {
                for (auto iter = map_components_.begin(); iter != map_components_.end(); iter++)
                {
                    ImGui::Separator();
                    if (ImGui::TreeNode(iter->second.Name.c_str()))
                    {
                        auto ref_values = iter->second.ReflectionValues;
                        if(ref_values.size()>0)
                        {
                            for (int i = 0; i < ref_values.size(); i++)
                            {
                                DrawReflectionInspector(&ref_values[i],iter->second.InstanceID);
                            }
                        }
                        ImGui::TreePop();
                    }
                }
            }
        }
        ImGui::EndChild();
    }
}

void InspectorWindow::DrawReflectionInspector(ReflectionInspector *reflection_node,int component_id)
{
    if (reflection_node)
    {
        if(reflection_node->DrawReflectionValue())
        {
            if (hierarchy_node_selected_)
            {
                ReqInspector req;
                req.Cmd = Req_Cmd_EditReflectionValue;
                req.InstanceID = hierarchy_node_selected_->InstanceID;
                req.ComponentInstanceID = component_id;
                json json_req = req;
                json_req["ReflectionValue"] = *reflection_node;
                json_req["ReflectionValue"]["Value"] = reflection_node->ToJson();
                std::string message = json_req.dump();
                printf("req %s\n", message.c_str());
                Send(message);
            }
        }
    }

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
                hierarchy_node_selected_ = (HierarchyNode*)hierarchy_node;

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