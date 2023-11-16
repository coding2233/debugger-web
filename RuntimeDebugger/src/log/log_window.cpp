//
// Created by wanderer on 2023/11/13.
//

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

bool LogWindow::OnDraw()
{
//    bool connected = CheckConnect();
//    if(connected)
//    {
//
//    }

    if(ImGui::Begin("Log",&show_))
    {
        if (log_node_index_ > 0)
        {
            float log_window_height = log_node_selected_index_<0?0: ImGui::GetWindowHeight()*0.55f;
            ImGui::BeginChild("LogWindow_log_node",ImVec2(0,log_window_height));
            for (int i = 0; i < log_node_index_; ++i)
            {
                ImGui::PushStyleColor(ImGuiCol_Text, log_nodes_[i].GetColor());
                if (ImGui::RadioButton(log_nodes_[i].SortLog.c_str(), log_node_selected_index_ == i))
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
            ImGui::EndChild();
        }

        if (log_node_selected_index_ >= 0 && log_node_selected_index_ < MAX_RUNTIME_DEBUGGER_LOG_NUMBER)
        {
            const LogNode &select_log_node = log_nodes_[log_node_selected_index_];
            ImGui::BeginChild("LogWindow_StackTrack",ImVec2(0, 0),true);
            ImGui::Spacing();
            ImGui::Text(select_log_node.LogTime.c_str());
            ImGui::Separator();
            ImGui::Text(select_log_node.LogMessage.c_str());
            ImGui::Separator();
            ImGui::Text(select_log_node.StackTrack.c_str());
            ImGui::EndChild();
        }

    }
    ImGui::End();

    return true;
}