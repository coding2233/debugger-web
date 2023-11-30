//
// Created by wanderer on 2023/11/29.
//

#ifndef RUNTIMEDEBUGGER_PROFILER_WINDOW_H
#define RUNTIMEDEBUGGER_PROFILER_WINDOW_H

#include <map>
#include <vector>
#include <string>

#include "imgui.h"
#include "app_window.h"
#include "implot.h"
#include "implot_internal.h"
#include "fps_node.h"
#include "memory_node.h"
#include "req_profiler.h"
#include "memory_sample_node.h"

class ProfilerWindow : public AppWindow
{
public:
    ProfilerWindow();
    ~ProfilerWindow();

private:
    std::vector<FPSNode> fps_nodes_;
    std::vector<MemoryNode> memory_nodes_;
    std::vector<std::string> memory_sample_types_;
    std::map<std::string,std::vector<MemorySampleProfiler>> memory_sample_nodes_;

    float axis_time_limit_ ;
    int data_node_max_;
    void Reset();

    void DrawSummary();
    void DrawMemorySample();

    void DrawFPS();
    void DrawMemory();

    void DrawMemorySampleProfiler(MemorySampleProfiler &sample_profile);
public:
    void OnMessage(const std::string &message) override;
    void OnDraw() override;
    void OnDrawExtend() override;

    void OnShow(bool show) override;
};



#endif //RUNTIMEDEBUGGER_PROFILER_WINDOW_H
