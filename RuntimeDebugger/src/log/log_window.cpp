//
// Created by wanderer on 2023/11/13.
//

#include "log_window.h"

LogWindow::LogWindow()
{}

LogWindow::~LogWindow()
{}

void LogWindow::OnMessage(const std::string &message)
{
    json json_log = json::parse(message);
    auto log_node = json_log.template get<LogNode>();
    log_nodes_.push_back(log_node);
    logs_.push_back(message);
}

bool LogWindow::OnDraw()
{
    bool connected = CheckConnect();
    if(connected)
    {

    }

    ImGui::Begin("Log",&show_);
//    if (logs_.size() > 0)
//    {
//        for (int i = 0; i < logs_.size(); ++i)
//        {
//            ImGui::Text(logs_[i].c_str());
//        }
//    }
    if (log_nodes_.size() > 0)
    {
        for (int i = 0; i < log_nodes_.size(); ++i)
        {
            ImGui::Text(log_nodes_[i].LogTime.c_str());
            ImGui::SameLine();
            ImGui::Text(log_nodes_[i].LogMessage.c_str());
        }
    }
    ImGui::End();

    return connected;
}