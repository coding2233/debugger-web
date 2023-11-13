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
//    json infor = json::parse(message);
    logs_.push_back(message);
}

bool LogWindow::OnDraw()
{
    bool connected = CheckConnect();
    if(connected)
    {

    }


    if (logs_.size() > 0)
    {
        ImGui::Begin("Log",&show_);
        for (int i = 0; i < logs_.size(); ++i)
        {
            ImGui::Text(logs_[i].c_str());
        }
        ImGui::End();
    }

    return connected;
}