//
// Created by wanderer on 2023/11/29.
//
#include <iostream>
#include <exception>
#include "profiler_window.h"


ProfilerWindow::ProfilerWindow()
{
    name_ = "Profiler";

    axis_time_limit_ = 15;
    data_node_max_ = 900;
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
                if(fps_nodes_.size() > data_node_max_)
                {
                    fps_nodes_.erase(fps_nodes_.begin());
                }
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
        std::vector<float> real_time_data;
        std::vector<float> fps_data;
        std::vector<float> frame_time_data;
        std::vector<float> fps_avg_data;
        std::vector<float> fps_avg_min_data;
        std::vector<float> fps_avg_max_data;
        float max_time ,min_time;
        float max_value;
        std::string fps_overview;
        for (int i=0;i<fps_nodes_.size();i++)
        {
            FPSNode &fps_node = fps_nodes_[i];
            real_time_data.push_back(fps_node.Realtime);
            fps_data.push_back(fps_node.FPS);
            frame_time_data.push_back(fps_node.FrameTime);
            fps_avg_data.push_back(fps_node.AvgFPS);
            fps_avg_min_data.push_back(fps_node.AvgFPSMin);
            fps_avg_max_data.push_back(fps_node.AvgFPSMax);
            if (i == fps_nodes_.size()-1)
            {
                max_time = fps_node.Realtime;
                min_time = max_time - axis_time_limit_;

                fps_overview="fps:";
                fps_overview.append(std::to_string((int)fps_node.FPS));
                fps_overview.append(" time:");
                fps_overview.append(std::to_string(fps_node.FrameTime));
                fps_overview.append(" avg:");
                fps_overview.append(std::to_string((int)fps_node.AvgFPS));
                fps_overview.append(" max:");
                fps_overview.append(std::to_string((int)fps_node.AvgFPSMax));
                fps_overview.append(" min:");
                fps_overview.append(std::to_string((int)fps_node.AvgFPSMin));
            }
            if(fps_node.AvgFPSMax > max_value)
            {
                max_value = fps_node.AvgFPSMax;
            }
        }

        if(max_value<30)
        {
            max_value = 30;
        }

        bool show_fills = true;
        bool show_lines = true;

        if (ImPlot::BeginPlot("FPS")) {
            ImPlot::SetupAxes("time","fps");
            ImPlot::SetupAxisLimits(ImAxis_X1,min_time, max_time, ImGuiCond_Always);
            ImPlot::SetupAxisLimits(ImAxis_Y1,0, max_value, ImGuiCond_Always);
            if (show_fills) {
                ImPlot::PushStyleVar(ImPlotStyleVar_FillAlpha, 0.25f);
                ImPlot::PlotShaded("fps", real_time_data.data(), fps_data.data(), data_size);
                ImPlot::PlotShaded("time(ms)", real_time_data.data(), frame_time_data.data(), data_size);
                ImPlot::PlotShaded("avg", real_time_data.data(), fps_avg_data.data(), data_size);
                //ImPlot::PlotShaded("min", real_time_data.data(), fps_avg_min_data.data(), data_size);
               // ImPlot::PlotShaded("max", real_time_data.data(), fps_avg_max_data.data(), data_size);
                ImPlot::PopStyleVar();
            }
            if (show_lines) {
                ImPlot::PlotLine("fps", real_time_data.data(), fps_data.data(), data_size);
                ImPlot::PlotLine("time(ms)", real_time_data.data(), frame_time_data.data(), data_size);
                ImPlot::PlotLine("avg", real_time_data.data(), fps_avg_data.data(), data_size);
                ImPlot::PlotLine("min", real_time_data.data(), fps_avg_min_data.data(), data_size);
                ImPlot::PlotLine("max", real_time_data.data(), fps_avg_max_data.data(), data_size);
            }
            ImPlot::EndPlot();
        }

        ImVec2 text_pos = ImGui::GetItemRectMin();
        ImGui::GetWindowDrawList()->AddText(text_pos,ImGui::ColorConvertFloat4ToU32(ImVec4(1,1,1,1)), fps_overview.c_str());
    }
}

void ProfilerWindow::OnDrawExtend()
{
}

void ProfilerWindow::OnShow(bool show)
{
    if (show)
    {
        Reset();
        Send("show");
    }
    else
    {
        Reset();
        Send("hide");
    }
}


void ProfilerWindow::Reset()
{
    fps_nodes_.clear();
}