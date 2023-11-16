//
// Created by EDY on 2023/11/13.
//

#include "information_window.h"

InformationWindow::InformationWindow()
{
    name_ = "Information";
}

InformationWindow::~InformationWindow()
{}

void InformationWindow::OnMessage(const std::string &message)
{
        json infor = json::parse(message);
        information_ = infor;
}

void InformationWindow::OnDraw()
{
    //bool connected = CheckConnect();
    bool connected = true;
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
                    ImGui::BeginChild("Information_Child");
                    ImGuiTableFlags table_flags = ImGuiTableFlags_RowBg | ImGuiTableFlags_Borders | ImGuiTableFlags_Resizable ;
                    ImGui::BeginTable("Information_Table",2,table_flags);
                    ImGui::TableSetupColumn("Key", ImGuiTableColumnFlags_WidthFixed);
                    ImGui::TableSetupColumn("Value", ImGuiTableColumnFlags_WidthStretch);
//                    ImGui::TableHeadersRow();
                    const std::map<std::string,std::string> infos = iter->second;
                    for (auto iterInfo = infos.begin(); iterInfo!=infos.end();iterInfo++)
                    {
                        ImGui::TableNextRow();
                        ImGui::TableSetColumnIndex(0);
                        ImGui::Text(iterInfo->first.c_str());
                        ImGui::TableSetColumnIndex(1);
                        ImGui::TextWrapped(iterInfo->second.c_str());
                    }

                    ImGui::EndTable();
                    ImGui::EndChild();
                    ImGui::EndTabItem();
                }
                ImGui::EndTabBar();
            }
        }
        ImGui::End();
    }

}