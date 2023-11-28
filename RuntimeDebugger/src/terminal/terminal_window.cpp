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
{
    json json_message = json::parse(message);
    TerminalMessage terminal_msg = json_message;
    termianl_messages_.push_back(terminal_msg);
}


void TerminalWindow::OnDraw()
{
    ImGui::Text("Type help/? to query the available commands");
    if(termianl_messages_.size() >0 )
    {
        for (int i = 0; i < termianl_messages_.size(); i++)
        {
            TerminalMessage &terminal_msg = termianl_messages_[i];
            //0.6431
            ImGui::TextColored(ImVec4(0, 0.7490, 0, 1), terminal_msg.Command.c_str());
            if (terminal_msg.ResultCode == 0)
            {
                ImGui::TextWrapped(terminal_msg.Result.c_str());
            } else
            {
                ImGui::TextColored(ImVec4(0.7490, 0, 0, 1), terminal_msg.Result.c_str());
            }
        }
    }

    ImGui::PushStyleColor(ImGuiCol_FrameBg, 0);
    ImGui::PushStyleColor(ImGuiCol_FrameBgActive, 0);
    ImGui::PushStyleColor(ImGuiCol_FrameBgHovered, 0);

    ImVec2 keySize = ImGui::CalcTextSize("$$");
    if (ImGui::IsWindowFocused() && !ImGui::IsAnyItemActive() && !ImGui::IsMouseClicked(0))
    {
        ImGui::SetKeyboardFocusHere(0);
    }
    ImGui::SetCursorPosX(keySize.x);
    ImGui::SetNextItemWidth(ImGui::GetContentRegionAvail().x - keySize.x);

    if (ImGui::InputText("##command_terminal", input_text_.data(), 256, ImGuiInputTextFlags_NoHorizontalScroll | ImGuiInputTextFlags_EnterReturnsTrue))
    {
        int str_size = strlen(input_text_.c_str());
        if (str_size > 0)
        {
            Send(input_text_.c_str());
            input_text_.resize(256);
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

