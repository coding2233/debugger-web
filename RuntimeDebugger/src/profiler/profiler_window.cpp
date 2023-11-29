//
// Created by wanderer on 2023/11/29.
//
#include <iostream>
#include <exception>
#include "profiler_window.h"


ProfilerWindow::ProfilerWindow()
{
    name_ = "Profiler";
}

ProfilerWindow::~ProfilerWindow()
{}

void ProfilerWindow::OnMessage(const std::string &message)
{
    json json_message = json::parse(message);
    try
    {
        //实时刷新的数据
        if (json_message["CmdId"].empty())
        {
            auto json_fps = json_message["FPS"];
            if (!json_fps.empty())
            {
                FPSNode fps_node = json_fps;
                fps_nodes_.push_back(fps_node);
            }
        }
            //请求回包
        else
        {}
    }
    catch (std::exception &e)
    {
        std::cout << "Standard exception: " << e.what() << "  json: "
                  << json_message.dump() << std::endl;
    }
}

void ProfilerWindow::OnDraw()
{
    if (fps_nodes_.size()>0)
    {
        int data_size = fps_nodes_.size();
        std::vector<double> real_time_data;
        std::vector<double> fps_data;
        std::vector<double> frame_time_data;
        std::vector<double> fps_avg_data;
        for (int i=0;i<fps_nodes_.size();i++)
        {
            FPSNode &fps_node = fps_nodes_[i];
            real_time_data.push_back(fps_node.Realtime);
            fps_data.push_back(fps_node.FPS);
            frame_time_data.push_back(fps_node.FrameTime);
            fps_avg_data.push_back(fps_node.AvgFPS);
           // ImGui::Text("fps:%f time:%d avg:%f",fps_node.FPS,fps_node.FrameTime,fps_node.AvgFPS);
        }

        bool show_fills = true;
        bool show_lines = true;

        if (ImPlot::BeginPlot("FPS")) {
            ImPlot::SetupAxes("time","fps");
            ImPlot::SetupAxesLimits(0,60,0,200);
//            if (show_fills) {
//                ImPlot::PushStyleVar(ImPlotStyleVar_FillAlpha, 0.25f);
//                ImPlot::PlotShaded("Stock 1", xs1, ys1, 101, shade_mode == 0 ? -INFINITY : shade_mode == 1 ? INFINITY : fill_ref, flags);
//                ImPlot::PlotShaded("Stock 2", xs1, ys2, 101, shade_mode == 0 ? -INFINITY : shade_mode == 1 ? INFINITY : fill_ref, flags);
//                ImPlot::PlotShaded("Stock 3", xs1, ys3, 101, shade_mode == 0 ? -INFINITY : shade_mode == 1 ? INFINITY : fill_ref, flags);
//                ImPlot::PopStyleVar();
//            }
            if (show_lines) {
                ImPlot::PlotLine("fps", real_time_data.data(), fps_data.data(), data_size);
                ImPlot::PlotLine("time(ms)", real_time_data.data(), frame_time_data.data(), data_size);
                ImPlot::PlotLine("avg", real_time_data.data(), fps_avg_data.data(), data_size);
            }
            ImPlot::EndPlot();
        }
    }
}

void ProfilerWindow::OnDrawExtend()
{
}

void ProfilerWindow::OnShow(bool show)
{
    if (show)
    {
        Send("show");
    }
    else
    {
        Send("hide");
    }
}