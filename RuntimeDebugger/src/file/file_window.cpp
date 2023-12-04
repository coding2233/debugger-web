//
// Created by wanderer on 2023/11/14.
//

#include <iostream>
#include <exception>
#include <filesystem>
namespace stdfs = std::filesystem;
#include "file_window.h"

FileWindow::FileWindow()
{
    name_ = "File";
}

FileWindow::~FileWindow()
{}


void FileWindow::OnMessage(const std::string & message)
{
    try
    {
        json json_message = json::parse(message);
        if (file_nodes_.size() == 0)
        {
            file_nodes_ = json_message;
        }
        else
        {
            select_file_ = json_message;
        }
    }
    catch (std::exception &e)
    {
        std::cout << "Standard exception: " << e.what() << std::endl;
    }
}

void FileWindow::OnShow(bool  show)
{
    if (show)
    {
        file_nodes_.clear();
        Send("show");
    }
    else
    {
        file_nodes_.clear();
        Send("hide");
    }
}

void FileWindow::OnDraw()
{
    if (file_nodes_.size()>0)
    {
        bool draw_file = select_file_.size() > 0;
        float child_window_height = draw_file ? ImGui::GetWindowWidth() * 0.35f : 0;
        if (ImGui::BeginChild("File_Window_Child_File_Node", ImVec2(child_window_height, 0), true))
        {
            for (auto iter = file_nodes_.begin(); iter != file_nodes_.end(); iter++) {
                const FileNode *file_node = &(iter->second);
                std::string file_node_root = iter->first;
                file_node_root.append(" ");
                file_node_root.append(file_node->FullName);
                if (ImGui::TreeNode(file_node_root.c_str())) {
                    DrawFileNode(file_node);
                    ImGui::TreePop();
                }
            }
        }
        ImGui::EndChild();

        if (draw_file)
        {
            ImGui::SameLine();
            if(ImGui::BeginChild("File_Window_Child_File_Content"))
            {
                for (auto iter = select_file_.begin(); iter!= select_file_.end();iter++)
                {
                    if(ImGui::Button("Save"))
                    {
                        stdfs::path filepath(iter->first.c_str());
                        std::string filename = filepath.filename().string();
                        std::string& read_data = iter->second;
                        AppSettings::SaveFile(read_data.c_str(),read_data.size(),filename.c_str());
                    }
                    ImGui::SameLine();
                    ImGui::Text(iter->first.c_str());
                    
                    ImGui::Separator();
                    ImGui::TextWrapped(iter->second.c_str());
                    break;
                }
            }
            ImGui::EndChild();
        }
    }
}

void FileWindow::DrawFileNode(const FileNode * file_node)
{
    ImGuiTreeNodeFlags node_flags = ImGuiTreeNodeFlags_OpenOnArrow | ImGuiTreeNodeFlags_OpenOnDoubleClick | ImGuiTreeNodeFlags_SpanFullWidth;
    bool hasChild = file_node->Children.size() > 0;
    if(!hasChild )
    {
        node_flags = node_flags | ImGuiTreeNodeFlags_Leaf;
    }
    bool selected = file_node->FullName == select_file_full_path_;
    if (selected)
    {
        node_flags = node_flags | ImGuiTreeNodeFlags_Selected;
    }
    bool tree_open = ImGui::TreeNodeEx(file_node->Name.c_str(),node_flags);
    if (ImGui::IsItemClicked())
    {
        if (selected)
        {
            select_file_.clear();
            select_file_full_path_.clear();
        }
        else
        {
            if (file_node->IsFile)
            {
                select_file_.clear();
                select_file_full_path_ = file_node->FullName;
                Send(select_file_full_path_);
            }
        }
    }
    if (tree_open)
    {
        if(hasChild)
        {
            for (int i= 0;i<file_node->Children.size();i++)
            {
                auto file_node_kid = &(file_node->Children[i]);
                DrawFileNode(file_node_kid);
            }
        }
        ImGui::TreePop();
    }
}