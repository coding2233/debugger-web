//
// Created by wanderer on 2023/11/13.
//

#include <stdio.h>
#include <time.h>
#include "log_window.h"

LogWindow::LogWindow()
{
    name_ = "Log";
    log_node_index_ = 0;
    log_node_selected_index_ = -1;
    log_nodes_.resize(MAX_RUNTIME_DEBUGGER_LOG_NUMBER);
}

LogWindow::~LogWindow()
{}

void LogWindow::OnMessage(const std::string &message)
{
    DebuggerProtocol::LogList log_list;
    log_list.ParseFromString(message);
    printf("%s \nlog count: %d",message.c_str(),log_list.log_node_list_size());

    if (log_list.log_node_list_size()>1)
    {
        log_node_selected_index_ = -1;
        log_nodes_.clear();
    }

    for(int i= 0;i<log_list.log_node_list_size();i++)
    {
        DebuggerProtocol::LogNode &log_node = log_list.log_node_list(i);

        log_nodes_.push_back(log_node);
        log_node_index_ = log_nodes_.size();
    }
}

void LogWindow::OnDraw()
{
    if (log_nodes_.size() > 0)
    {
        if (ImGui::Button("Clear"))
        {
            log_nodes_.erase(log_nodes_.begin(),log_nodes_.end());
            log_node_index_ = 0;
            log_node_selected_index_ = -1;
            return;
        }
        ImGui::SameLine();
        if (ImGui::Button("Save"))
        {
            if(log_nodes_.size()>0)
            {
                std::string save_data;
                 for(int i=0;i< log_nodes_.size();i++)
                 {
                     save_data.append(log_nodes_[i].log_message());
                     save_data.append("\n");
                     save_data.append(log_nodes_[i].log_time());
                     save_data.append("\n");
                     save_data.append(log_nodes_[i].log_stack_track());
                     save_data.append("\n");
                 }

                // 创建一个time_t类型变量来存储当前时间
                time_t now;
                // 获取当前时间
                time(&now);
                // 创建一个tm结构体来存储转换后的本地时间
                struct tm *local = localtime(&now);
                // 创建一个足够大的字符数组来存储日期和时间字符串
                char time_str[80];
                // 使用strftime函数格式化时间
                strftime(time_str, sizeof(time_str), "%Y_%m_%d_%H_%M_%S", local);
                std::string log_file_name(time_str);
                log_file_name.append(".log");

                AppSettings::SaveFile(save_data.c_str(),save_data.size(),log_file_name.c_str());
            }
        }
        float log_window_height = log_node_selected_index_<0?0: ImGui::GetWindowHeight()*0.55f;
        if(ImGui::BeginChild("LogWindow_log_node",ImVec2(0,log_window_height)))
        {
            for (int i = 0; i < log_nodes_.size(); ++i)
            {
                DebuggerProtocol::LogNode & log_node = log_nodes_[i];
                auto color = log_type_color_[log_node.log_type()];
                ImGui::PushStyleColor(ImGuiCol_Text, color);
                if (ImGui::RadioButton(log_node.log_message().c_str(), log_node_selected_index_ == i))
                {
                    if (log_node_selected_index_ == i)
                    {
                        log_node_selected_index_ = -1;
                    }
                    else
                    {
                        log_node_selected_index_ = i;
                    }
                }
                ImGui::PopStyleColor();
            }
        }
        ImGui::EndChild();
    }

     if (log_node_selected_index_ >= 0 && log_node_selected_index_ < log_nodes_.size())
     {
         DebuggerProtocol::LogNode &select_log_node = log_nodes_[log_node_selected_index_];
         if(ImGui::BeginChild("LogWindow_StackTrack",ImVec2(0, 0),true))
         {
             ImGui::Spacing();
             ImGui::TextWrapped(select_log_node.log_time().c_str());
             ImGui::Separator();
             ImGui::TextWrapped(select_log_node.log_message().c_str());
             ImGui::Separator();
             ImGui::TextWrapped(select_log_node.log_stack_track().c_str());
         }
         ImGui::EndChild();
     }


}