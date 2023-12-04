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
    json json_log = json::parse(message);
    auto log_node = json_log.template get<LogNode>();
    log_node.BuildSortLog();
    if (log_node_index_ == MAX_RUNTIME_DEBUGGER_LOG_NUMBER)
    {
        log_nodes_.erase(log_nodes_.begin());
        if (log_node_selected_index_>=0)
        {
            log_node_selected_index_--;
        }
    }
    else
    {
        log_node_index_++;
    }
    log_nodes_.insert(log_nodes_.begin()+log_node_index_-1,log_node);
}

void LogWindow::OnDraw()
{
    if (log_node_index_ > 0)
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
                    save_data.append(log_nodes_[i].SortLog);
                    save_data.append("\n");
                    save_data.append(log_nodes_[i].LogTime);
                    save_data.append("\n");
                    save_data.append(log_nodes_[i].StackTrack);
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
            for (int i = 0; i < log_node_index_; ++i)
            {
                ImGui::PushStyleColor(ImGuiCol_Text, log_nodes_[i].GetColor());
                if (ImGui::RadioButton(log_nodes_[i].SortLog.c_str(), log_node_selected_index_ == i))
                {
                    if (log_node_selected_index_ == i)
                    {
                        log_node_selected_index_ = -1;
                    } else
                    {
                        log_node_selected_index_ = i;
                    }
                }
                ImGui::PopStyleColor();
            }
        }
        ImGui::EndChild();
    }

    if (log_node_selected_index_ >= 0 && log_node_selected_index_ < MAX_RUNTIME_DEBUGGER_LOG_NUMBER)
    {
        const LogNode &select_log_node = log_nodes_[log_node_selected_index_];
        if(ImGui::BeginChild("LogWindow_StackTrack",ImVec2(0, 0),true))
        {
            ImGui::Spacing();
            ImGui::TextWrapped(select_log_node.LogTime.c_str());
            ImGui::Separator();
            ImGui::TextWrapped(select_log_node.LogMessage.c_str());
            ImGui::Separator();
            ImGui::TextWrapped(select_log_node.StackTrack.c_str());
        }
        ImGui::EndChild();
    }


}