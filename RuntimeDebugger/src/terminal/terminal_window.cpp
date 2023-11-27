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

//    Vector2 keySize = ImGui.CalcTextSize("$$");
//    if (!ImGui.IsAnyItemActive() && !ImGui.IsMouseClicked(0))
//    {
//        ImGui.SetKeyboardFocusHere(0);
//    }
//    ImGui.SetCursorPosX(keySize.X);
//    ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - keySize.X);
//    if (ImGui.InputText("", ref m_inputCommand, 200, ImGuiInputTextFlags.NoHorizontalScroll | ImGuiInputTextFlags.EnterReturnsTrue))
//    {
//        if (!string.IsNullOrEmpty(m_inputCommand))
//        {
//            RunCommand(m_inputCommand, m_gitRepo != null ? m_gitRepo.RootPath : "");
//            m_inputCommand = "";
//        }
//    }
//
//    ImGui.GetWindowDrawList().AddText(ImGui.GetItemRectMin()-new Vector2(keySize.X*0.5f,-3f), ImGui.ColorConvertFloat4ToU32(Vector4.One), "$");

    ImGui::PopStyleColor();
    ImGui::PopStyleColor();
    ImGui::PopStyleColor();
}

