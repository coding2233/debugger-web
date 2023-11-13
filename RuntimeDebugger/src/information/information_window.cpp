//
// Created by EDY on 2023/11/13.
//

#include "information_window.h"

InformationWindow::InformationWindow()
{}

InformationWindow::~InformationWindow()
{}

void InformationWindow::OnMessage(const std::string &message)
{
        json infor = json::parse(message);
        auto si = infor["System Information"];
        information_ = infor;
        //information_ = infor.get<std::map<std::string,std::string>>();
}

bool InformationWindow::OnDraw()
{
    bool connected = CheckConnect();
    if(connected)
    {

    }

    if (information_.size() > 0)
    {
        ImGui::Begin("Information",&show_);
        for (auto iter = information_.begin();iter!=information_.end();iter++)
        {
            ImGui::Text(iter->first.c_str());
            ImGui::Separator();
//            ImGui::Text(iter->second.c_str());
//            ImGui::Separator();
            ImGui::Separator();
            ImGui::Separator();
        }
        ImGui::End();
    }

    return connected;
}