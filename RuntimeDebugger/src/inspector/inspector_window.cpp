//
// Created by wanderer on 2023/11/14.
//

#include "inspector_window.h"

InspectorWindow::InspectorWindow()
{}

InspectorWindow::~InspectorWindow()
{}

void InspectorWindow::OnMessage(const std::string &message)
{}


bool InspectorWindow::OnDraw()
{
    bool connected = CheckConnect();

    return connected;
}