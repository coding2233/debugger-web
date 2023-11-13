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
            ImGuiTabBarFlags tab_bar_flags = ImGuiTabBarFlags_None;
            if (ImGui::BeginTabBar("InformationBar", tab_bar_flags))
            {
                if (ImGui::BeginTabItem(iter->first.c_str()))
                {
                    ImGuiTableFlags table_flags = ImGuiTableFlags_RowBg | ImGuiTableFlags_Borders;
                    ImGui::BeginTable("Information_Table",2,table_flags);
                    const std::map<std::string,std::string> infos = iter->second;
                    for (auto iterInfo = infos.begin(); iterInfo!=infos.end();iterInfo++)
                    {
                        ImGui::TableNextRow();
                        ImGui::TableSetColumnIndex(0);
                        ImGui::Text(iterInfo->first.c_str());
                        ImGui::TableSetColumnIndex(1);
                        ImGui::Text(iterInfo->second.c_str());
                    }

                    ImGui::EndTable();
                    ImGui::EndTabItem();
                }
                ImGui::EndTabBar();
            }

//            ImGui::Text(iter->first.c_str());
//            ImGui::Separator();
////            ImGui::Text(iter->second.c_str());
////            ImGui::Separator();
//            ImGui::Separator();
//            ImGui::Separator();
        }
        ImGui::End();
    }

    return connected;
}