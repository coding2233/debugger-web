//
// Created by wanderer on 2023/11/14.
//

#include "inspector_window.h"

std::vector<std::string> message_test_;

InspectorWindow::InspectorWindow()
{}

InspectorWindow::~InspectorWindow()
{}

void InspectorWindow::OnMessage(const std::string &message)
{
    message_test_.push_back(message);
}


bool InspectorWindow::OnDraw()
{
    bool connected = CheckConnect();
    ImGui::Begin("Inspector",&show_);
    if (message_test_.size()>0)
    {
        for (int i= 0;i<message_test_.size();i++)
        {
            ImGui::Text(message_test_[i].c_str());
        }
    }
    ImGui::End();
    return connected;
}