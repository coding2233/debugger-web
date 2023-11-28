//
// Created by EDY on 2023/11/27.
//

#include "terminal_window.h"


TerminalWindow::TerminalWindow()
{
    name_ = "Terminal";
}

TerminalWindow::~TerminalWindow()
{}

void TerminalWindow::OnMessage(const std::string &message)
{}


void TerminalWindow::OnDraw()
{


    for (int i= 0;i<log_nodes_.size();i++)
    {
        ImGui::Text(log_nodes_[i].c_str());
    }

    ImGui::PushStyleColor(ImGuiCol_FrameBg, 0);
    ImGui::PushStyleColor(ImGuiCol_FrameBgActive, 0);
    ImGui::PushStyleColor(ImGuiCol_FrameBgHovered, 0);

    ImVec2 keySize = ImGui::CalcTextSize("$$");
    if (!ImGui::IsAnyItemActive() && !ImGui::IsMouseClicked(0))
    {
        ImGui::SetKeyboardFocusHere(0);
    }
    ImGui::SetCursorPosX(keySize.x);
    ImGui::SetNextItemWidth(ImGui::GetContentRegionAvail().x - keySize.x);

    if (ImGui::InputText("##command", input_text_.data(), 512, ImGuiInputTextFlags_NoHorizontalScroll | ImGuiInputTextFlags_EnterReturnsTrue))
    {
        int str_size = strlen(input_text_.c_str());
        if (str_size > 0)
        {
            log_nodes_.push_back(input_text_.c_str());
            printf("%s \n",input_text_.c_str());
            //RunCommand(m_inputCommand, m_gitRepo != null ? m_gitRepo.RootPath : "");
            input_text_.clear();
        }
    }
//
    ImVec2 text_pos = ImVec2(ImGui::GetItemRectMin().x - keySize.x*0.5,ImGui::GetItemRectMin().y+2);
    ImGui::GetWindowDrawList()->AddText(text_pos,ImGui::ColorConvertFloat4ToU32(ImVec4(1,1,1,1)), "$");

    ImGui::PopStyleColor();
    ImGui::PopStyleColor();
    ImGui::PopStyleColor();
}

