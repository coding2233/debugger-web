//
// Created by wanderer on 2023/11/29.
//

#ifndef RUNTIMEDEBUGGER_PROFILER_WINDOW_H
#define RUNTIMEDEBUGGER_PROFILER_WINDOW_H

#include <vector>
#include <string>

#include "imgui.h"
#include "app_window.h"
#include "implot.h"
#include "implot_internal.h"

class ProfilerWindow : public AppWindow
{
public:
    ProfilerWindow();
    ~ProfilerWindow();


public:
    void OnMessage(const std::string &message) override;
    void OnDraw() override;
    void OnDrawExtend() override;
};



#endif //RUNTIMEDEBUGGER_PROFILER_WINDOW_H
