//
// Created by wanderer on 2023/11/14.
//

#ifndef RUNTIMEDEBUGGER_INSPECTOR_WINDOW_H
#define RUNTIMEDEBUGGER_INSPECTOR_WINDOW_H

#include <map>
#include <string>
#include <vector>

#include "app_window.h"

class InspectorWindow :public AppWindow
{
public:
    InspectorWindow();
    ~InspectorWindow();
public:
    void OnMessage(const std::string &message) override;
    bool OnDraw() override;
};


#endif //RUNTIMEDEBUGGER_INSPECTOR_WINDOW_H
