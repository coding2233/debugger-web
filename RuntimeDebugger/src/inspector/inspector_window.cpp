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
            Reset();
//            auto rsp = json_rsp.template get<RspInspector>();
            auto find_nodes = json_rsp["FindNodes"].template get<std::vector<HierarchyNode>>();
            //添加到map
            for (int i = 0; i < find_nodes.size(); ++i)
            {
                const HierarchyNode find_node = find_nodes[i];
                if (map_hierarchy_nodes_.find(find_node.InstanceID) == map_hierarchy_nodes_.end())
                {
                    map_hierarchy_nodes_.insert({find_node.InstanceID, find_node});
                }
            }

            //整理层级
            for (int i = 0; i < find_nodes.size(); ++i)
            {
                const HierarchyNode &find_node = find_nodes[i];
                const HierarchyNode& map_node_iter = map_hierarchy_nodes_.find(find_node.InstanceID)->second;

                if (map_node_iter.ParentInstanceID == 0)
                {

                    auto root_iter = hierarchy_root_nodes_.find(map_node_iter.Scene);
                    if (root_iter == hierarchy_root_nodes_.end())
                    {
                        std::vector<const HierarchyNode*> new_hier_nodes;
                        hierarchy_root_nodes_.insert({map_node_iter.Scene,new_hier_nodes});
                        root_iter = hierarchy_root_nodes_.find(map_node_iter.Scene);
                    }

                    std::vector<const HierarchyNode*> &hierarchy_nodes = root_iter->second;
                    bool has_node = false;
                    for (int i = 0; i < hierarchy_nodes.size(); i++)
                    {
                        if (hierarchy_nodes[i]->InstanceID == map_node_iter.InstanceID)
                        {
                            has_node = true;
                            break;
                        }
                    }
                    if (!has_node)
                    {
                        hierarchy_nodes.push_back(&map_node_iter);
                    }
                }
                else
                {
                    auto find_parent_iter = map_hierarchy_nodes_.find(map_node_iter.ParentInstanceID);
                    if (find_parent_iter != map_hierarchy_nodes_.end())
                    {
                        find_parent_iter->second.AddChild(&map_node_iter);
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

                        ReflectionInspector *reflection_value = (ReflectionInspector *) &(compoent_inspector->ReflectionValues[j]);
                        try
                        {
                            reflection_value->Value.ToData(reflection_value->ValueType, value_json);
                        }
                        catch (std::exception &e)
                        {
                            std::cout << "Standard exception: " << e.what() << "  json: "
                                      << json_rsp_reflection_value.dump() << std::endl;
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
        FindGameObjects();
    }
}

void InspectorWindow::FindGameObjects()
{
    Reset();

    ReqInspector req;
    req.Cmd = Req_Cmd_FindGameObjects;
    req.InstanceID = 0;
    json json_req = req;
    std::string message = json_req.dump();
    printf("req %s\n",message.c_str());
    Send(message);
}

void InspectorWindow::Reset()
{
    search_hierarchy_root_nodes_.clear();
    hierarchy_root_nodes_.clear();
    map_hierarchy_nodes_.clear();
    hierarchy_node_selected_ = nullptr;
}

void InspectorWindow::OnDraw()
{

    bool node_selected = hierarchy_node_selected_;

    float log_window_height = node_selected? ImGui::GetWindowWidth()*0.35f:0;
    if(ImGui::BeginChild("Inspector_Child_Hierarchy",ImVec2(log_window_height,0),true))
    {
        //搜索节点
        if (ImGui::InputText("Search",search_hierarchy_text_.data(),256))
        {
            search_hierarchy_root_nodes_.clear();
            const char* search_text = search_hierarchy_text_.c_str();
            int text_size = strlen(search_text);
            if (text_size > 0)
            {
                for (auto iter = map_hierarchy_nodes_.begin(); iter != map_hierarchy_nodes_.end(); iter++)
                {
                    const HierarchyNode *search_node = &(iter->second);
                    if (search_node->Name.find(search_text)!=std::string::npos)
                    {
                        search_hierarchy_root_nodes_.push_back(search_node);
                    }
                }
            }
        }
        //刷新所有的物体
        ImGui::SameLine();
        if(ImGui::Button("Refresh"))
        {
            FindGameObjects();
        }
        //绘制搜索结果
        if (search_hierarchy_root_nodes_.size()>0)
        {
            ImGui::Separator();
            for (int i = 0; i < search_hierarchy_root_nodes_.size(); i++)
            {
                DrawInspectorNode(search_hierarchy_root_nodes_[i], false);
            }
        }
        ImGui::Separator();
        //绘制节点树
        for(auto hierarchy_root_iter = hierarchy_root_nodes_.begin();hierarchy_root_iter!=hierarchy_root_nodes_.end();hierarchy_root_iter++)
        {
            if(ImGui::TreeNode(hierarchy_root_iter->first.c_str()))
            {
                std::vector<const HierarchyNode*> &hierarchy_root_nodes = hierarchy_root_iter->second;
                for (int i = 0; i < hierarchy_root_nodes.size(); i++)
                {
                    DrawInspectorNode(hierarchy_root_nodes[i], true);
                }

                ImGui::TreePop();
            }
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
            bool edit_gameobject= false;
            if(ImGui::Checkbox("Active",&(hierarchy_node_selected_->Active)))
            {
                edit_gameobject = true;
            }
            ImGui::SameLine();
            if(ImGui::InputText("Name",hierarchy_node_selected_->Name.data(),128,ImGuiInputTextFlags_ReadOnly))
            {
//                int name_end_index = hierarchy_node_selected_->Name.find('\0');
//                if (name_end_index>0)
//                {
//                    hierarchy_node_selected_->Name = hierarchy_node_selected_->Name.substr(0, name_end_index);
//                }
//                edit_gameobject = true;
            }

            if (ImGui::Button("Destory"))
            {
                std::string destory_game_object = "DestroyGameObject ";
                destory_game_object.append(std::to_string(hierarchy_node_selected_->InstanceID));
                SendForward(5,destory_game_object);
            }
            ImGui::SameLine();
            ImGui::SetNextItemWidth(one_third_width);
            if(ImGui::InputText("Tag", hierarchy_node_selected_->Tag.data(),128,ImGuiInputTextFlags_ReadOnly))
            {
                edit_gameobject = true;
            }
            ImGui::SameLine();
            ImGui::SetNextItemWidth(one_third_width);
            if(ImGui::InputText("Layer", hierarchy_node_selected_->Layer.data(),128,ImGuiInputTextFlags_EnterReturnsTrue))
            {
                int end_index = hierarchy_node_selected_->Layer.find('\0');
                if (end_index>0)
                {
                    hierarchy_node_selected_->Layer = hierarchy_node_selected_->Layer.substr(0, end_index);
                }
                edit_gameobject = true;
            }
            if (edit_gameobject)
            {
                ReqInspector req;
                req.Cmd = Req_Cmd_EditGameObject;
                req.InstanceID = hierarchy_node_selected_->InstanceID;
                json json_req = req;
                json_req["HierarchyNode"] = *hierarchy_node_selected_;
                std::string message = json_req.dump();
                printf("req %s\n", message.c_str());
                Send(message);
            }

            if (map_components_.size() > 0)
            {
                for (auto iter = map_components_.begin(); iter != map_components_.end(); iter++)
                {
                    ImGui::Separator();
                    if (ImGui::TreeNode(iter->second.Name.c_str()))
                    {
                        if(ImGui::Button("Destory"))
                        {
                            std::string destory_compoent = "DestroyComponent ";
                            destory_compoent.append(std::to_string(hierarchy_node_selected_->InstanceID));
                            destory_compoent.append(" ");
                            destory_compoent.append(std::to_string(iter->second.InstanceID));

                            SendForward(5,destory_compoent);
                        }

                        auto ref_values = iter->second.ReflectionValues;
                        if(ref_values.size()>0)
                        {
                            std::map<std::string,ReflectionInspector*> material_reflection_nodes;
                            for (int i = 0; i < ref_values.size(); i++)
                            {
                                ReflectionInspector *reflection_node = &ref_values[i];
                                bool normal_reflection_node = reflection_node->Name == reflection_node->FullName;
                                if (normal_reflection_node)
                                {
                                    DrawReflectionInspector(reflection_node, iter->second.InstanceID);
                                }
                                //材质的属性
                                else
                                {
                                    material_reflection_nodes.insert({reflection_node->FullName,reflection_node});
                                }
                            }

                            //materials
                            auto ref_materials_values = iter->second.MapMaterialValues;
                            if (ref_materials_values.size()>0)
                            {
                                for (auto iter_material = ref_materials_values.begin();iter_material != ref_materials_values.end();iter_material++)
                                {
                                    if (ImGui::TreeNode(iter_material->first.c_str()))
                                    {
                                        std::vector<std::vector<std::string>> material_values=iter_material->second;
                                        for (int i=0;i<material_values.size();i++)
                                        {
                                            std::vector<std::string> material_ref_values = material_values[i];

                                            std::string material_child_name = "Inspector_Child_Component_";
                                            material_child_name.append(iter_material->first);
                                            material_child_name.append(std::to_string(i));
                                            if(ImGui::BeginChild(material_child_name.c_str(),ImVec2(0,350),true))
                                            {
                                                if (material_ref_values.size()>0)
                                                {
                                                    ImGui::Text(material_ref_values[0].c_str());
                                                    for (int j = 0;j<material_ref_values.size();j++)
                                                    {
                                                        ReflectionInspector* find_ref_inspctor = material_reflection_nodes.find(material_ref_values[j])->second;
                                                        DrawReflectionInspector(find_ref_inspctor,iter->second.InstanceID);
                                                    }
                                                }
                                            }
                                            ImGui::EndChild();
                                        }
                                        ImGui::TreePop();
                                    }
                                }
                            }
                        }
                        ImGui::TreePop();
                    }
                }
            }

            //Add Component
            if (ImGui::InputText("##Add Component Input",add_component_type_.data(),128, ImGuiInputTextFlags_NoHorizontalScroll | ImGuiInputTextFlags_EnterReturnsTrue))
            {
            }
            ImGui::SameLine();
            if (ImGui::Button("Add Component"))
            {
                std::string add_compoent = "AddComponent ";
                add_compoent.append(std::to_string(hierarchy_node_selected_->InstanceID));
                add_compoent.append(" ");
                add_compoent.append(add_component_type_.c_str());

                SendForward(5,add_compoent);
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

void InspectorWindow::DrawInspectorNode(const HierarchyNode* hierarchy_node,bool draw_child)
{
    if (!hierarchy_node)
    {
        return;
    }
    bool selected = hierarchy_node == hierarchy_node_selected_;
    ImGuiTreeNodeFlags node_flags = ImGuiTreeNodeFlags_OpenOnArrow | ImGuiTreeNodeFlags_OpenOnDoubleClick | ImGuiTreeNodeFlags_SpanFullWidth;
    bool hasChild = hierarchy_node->ChildrenNodes.size() > 0;
    if(!hasChild || !draw_child)
    {
        node_flags = node_flags | ImGuiTreeNodeFlags_Leaf;
    }
    if (selected)
    {
        node_flags = node_flags | ImGuiTreeNodeFlags_Selected;
    }

    if (!hierarchy_node->Active)
    {
        ImGui::PushStyleColor( ImGuiCol_Text, ImGui::GetStyle().Colors[ ImGuiCol_TextDisabled ] );
    }
    bool tree_open = ImGui::TreeNodeEx(hierarchy_node->Name.c_str(),node_flags);
    if (!hierarchy_node->Active)
    {
        ImGui::PopStyleColor();
    }
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
        if(hasChild && draw_child)
        {
            for (int i= 0;i<hierarchy_node->ChildrenNodes.size();i++)
            {
                auto node = hierarchy_node->ChildrenNodes[i];
                DrawInspectorNode(node,draw_child);
            }
        }
        ImGui::TreePop();
//        ImGui::Spacing();
    }


}