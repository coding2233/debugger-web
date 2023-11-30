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
            auto json_memory = json_message["Memory"];
            if (!json_memory.empty())
            {
                MemoryNode memory_node = json_memory;
                if(memory_nodes_.size() > data_node_max_)
                {
                    memory_nodes_.erase(memory_nodes_.begin());
                }
                memory_nodes_.push_back(memory_node);
            }
        }
            //请求回包
        else
        {
            ReqProfilerCmd cmd = json_message["CmdId"];
            if (cmd == ReqProfilerCmd_GetMemorySampleTypes)
            {
                memory_sample_types_ = json_message["SmapleTypes"];
            }
            else if (cmd == ReqProfilerCmd_TaskMemorySample)
            {
                std::string sample_type = json_message["SmapleType"];
                MemorySampleProfiler sample =  json_message["Sample"];
                auto sample_iter = memory_sample_nodes_.find(sample_type);
                if (sample_iter==memory_sample_nodes_.end())
                {
                    std::vector<MemorySampleProfiler> sample_nodes;
                    sample_nodes.push_back(sample);
                    memory_sample_nodes_.insert({sample_type,sample_nodes});
                }
                else
                {
                    std::vector<MemorySampleProfiler> &sample_nodes = sample_iter->second;
                    sample_nodes.push_back(sample);
                }
            }
        }
    }
    catch (std::exception &e)
    {
        std::cout << "Standard exception: " << e.what() << "  json: "
                  << json_message.dump() << std::endl;
    }
}

void ProfilerWindow::OnDraw()
{
    if (ImGui::BeginTabBar("ProfilerTabBar", ImGuiTabBarFlags_None))
    {
        if (ImGui::BeginTabItem("Summary"))
        {
            DrawSummary();
            ImGui::EndTabItem();
        }
        if (ImGui::BeginTabItem("Memory"))
        {
            DrawMemorySample();
            ImGui::EndTabItem();
        }
        ImGui::EndTabBar();
    }
}

void ProfilerWindow::DrawSummary()
{
    DrawFPS();
    DrawMemory();
}

void ProfilerWindow::DrawMemorySample()
{
    if (memory_sample_types_.size()>0)
    {
        if (ImGui::BeginTabBar("ProfilerMemorySampleBar", ImGuiTabBarFlags_None))
        {
            for (int i =0 ;i<memory_sample_types_.size();i++)
            {
                std::string sample_type = memory_sample_types_[i];
                if (ImGui::BeginTabItem(sample_type.c_str()))
                {
                    if (ImGui::Button("Take Sample"))
                    {
                        //获取内存的sample
                        ReqProfilerMessage req;
                        req.CmdId = ReqProfilerCmd_TaskMemorySample;
                        req.SmapleType = sample_type;
                        json json_req = req;
                        Send(json_req.dump());
                    }

                    //draw memory_sample_nodes_
                    auto sample_iter = memory_sample_nodes_.find(sample_type);
                    if (sample_iter != memory_sample_nodes_.end())
                    {
                        std::vector<MemorySampleProfiler> &sample_nodes = sample_iter->second;
                        //清理数据
                        ImGui::SameLine();
                        if (ImGui::Button("Clear"))
                        {
                            sample_nodes.clear();
                        }

                        if (sample_nodes.size()>0)
                        {
                            for (int i=0;i<sample_nodes.size();i++)
                            {
                                MemorySampleProfiler &sample_profile = sample_nodes[i];
                                DrawMemorySampleProfiler(sample_profile);
                            }
                        }
                    }

                    ImGui::EndTabItem();
                }
            }
            ImGui::EndTabBar();
        }
    }
}

void ProfilerWindow::DrawMemorySampleProfiler(MemorySampleProfiler &sample_profile)
{
    int node_size = sample_profile.Nodes.size();
    std::string title_name = sample_profile.DateTime;
    title_name.append(" [");
    title_name.append(std::to_string(node_size));
    title_name.append("] size:");
    title_name.append(std::to_string(sample_profile.Size));
    title_name.append("MB frameCount:");
    title_name.append(std::to_string(sample_profile.FrameCount));
    title_name.append(" realtime:");
    title_name.append(std::to_string(sample_profile.Realtime));

    if(ImGui::TreeNode(title_name.c_str()))
    {
        if (node_size > 0)
        {
            ImGuiTableFlags table_flags =
                    ImGuiTableFlags_RowBg | ImGuiTableFlags_Borders | ImGuiTableFlags_Resizable;
            if(ImGui::BeginTable("Profiler_Memory_Sample_Table", 3, table_flags))
            {
                ImGui::TableSetupColumn("Name", ImGuiTableColumnFlags_WidthFixed);
                ImGui::TableSetupColumn("Type", ImGuiTableColumnFlags_WidthStretch);
                ImGui::TableSetupColumn("Size", ImGuiTableColumnFlags_WidthStretch);
//                    ImGui::TableHeadersRow();
                for (int i = 0; i < node_size; ++i)
                {
                    MemorySampleNode &node = sample_profile.Nodes[i];
                    ImGui::TableNextRow();
                    ImGui::TableSetColumnIndex(0);
                    ImGui::Text(node.Name.c_str());
                    ImGui::TableSetColumnIndex(1);
                    ImGui::Text(node.Type.c_str());
                    ImGui::TableSetColumnIndex(2);
                    ImGui::Text("%f",node.Size);
                }

            }
            ImGui::EndTable();
        }
        ImGui::TreePop();
    }
}

void ProfilerWindow::DrawFPS()
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
        float max_value = 30;
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

                fps_overview="FPS:";
                fps_overview.append(std::to_string((int)fps_node.FPS));
                fps_overview.append(" Time:");
                fps_overview.append(std::to_string(fps_node.FrameTime));
                fps_overview.append(" Avg:");
                fps_overview.append(std::to_string((int)fps_node.AvgFPS));
                fps_overview.append(" Max:");
                fps_overview.append(std::to_string((int)fps_node.AvgFPSMax));
                fps_overview.append(" Min:");
                fps_overview.append(std::to_string((int)fps_node.AvgFPSMin));
            }

            if(fps_node.AvgFPSMax > max_value)
            {
                max_value = fps_node.AvgFPSMax;
            }
        }

        max_value+=50;

        bool show_fills = true;
        bool show_lines = true;

        if (ImPlot::BeginPlot("FPS")) {
            ImPlot::SetupAxes("time","fps");
            ImPlot::SetupAxisLimits(ImAxis_X1,min_time, max_time, ImGuiCond_Always);
            ImPlot::SetupAxisLimits(ImAxis_Y1,0, max_value, ImGuiCond_Always);
            if (show_fills) {
                ImPlot::PushStyleVar(ImPlotStyleVar_FillAlpha, 0.25f);
                ImPlot::PlotShaded("FPS", real_time_data.data(), fps_data.data(), data_size);
                ImPlot::PlotShaded("Time(ms)", real_time_data.data(), frame_time_data.data(), data_size);
                ImPlot::PlotShaded("Avg", real_time_data.data(), fps_avg_data.data(), data_size);
                //ImPlot::PlotShaded("min", real_time_data.data(), fps_avg_min_data.data(), data_size);
                // ImPlot::PlotShaded("max", real_time_data.data(), fps_avg_max_data.data(), data_size);
                ImPlot::PopStyleVar();
            }
            if (show_lines) {
                ImPlot::PlotLine("FPS", real_time_data.data(), fps_data.data(), data_size);
                ImPlot::PlotLine("Time(ms)", real_time_data.data(), frame_time_data.data(), data_size);
                ImPlot::PlotLine("Avg", real_time_data.data(), fps_avg_data.data(), data_size);
                ImPlot::PlotLine("Min", real_time_data.data(), fps_avg_min_data.data(), data_size);
                ImPlot::PlotLine("Max", real_time_data.data(), fps_avg_max_data.data(), data_size);
            }
            ImPlot::EndPlot();
        }

        ImVec2 text_pos = ImGui::GetItemRectMin();
        ImGui::GetWindowDrawList()->AddText(text_pos,ImGui::ColorConvertFloat4ToU32(ImGui::GetStyleColorVec4(ImGuiCol_Text)), fps_overview.c_str());
    }
}

void ProfilerWindow::DrawMemory()
{
    if (memory_nodes_.size() > 0)
    {
        int data_size = memory_nodes_.size();
        std::vector<float> real_time_data;
        std::vector<float> total_reserved_data;
        std::vector<float> total_allocated_data;
        std::vector<float> mono_used_size_data;
        std::vector<float> max_used_data;
        std::vector<float> mono_heap_data;
        std::vector<float> used_heap_data;
        std::vector<float> total_unused_reserved_data;
        std::vector<float> allocated_memory_for_graphics_driver_data;
        std::vector<float> temp_allocator_size_data;
        float max_time,min_time;
        float max_value = 100;
        std::string memory_overview;
        for (int i=0;i<memory_nodes_.size();i++)
        {
            MemoryNode &memory_node = memory_nodes_[i];
            real_time_data.push_back(memory_node.Realtime);
            total_reserved_data.push_back(memory_node.TotalReservedMemory);
            total_allocated_data.push_back(memory_node.TotalAllocatedMemory);
            mono_used_size_data.push_back(memory_node.MonoUsedSize);
            max_used_data.push_back(memory_node.MaxUsedMemory);
            mono_heap_data.push_back(memory_node.MonoHeapSize);
            used_heap_data.push_back(memory_node.UsedHeapSize);
            total_unused_reserved_data.push_back(memory_node.TotalUnusedReserved);
            allocated_memory_for_graphics_driver_data.push_back(memory_node.AllocatedMemoryForGraphicsDriver);
            temp_allocator_size_data.push_back(memory_node.TempAllocatorSize);
            if(memory_node.TotalReservedMemory > max_value)
            {
                max_value = memory_node.TotalReservedMemory;
            }

            if (i == memory_nodes_.size()-1)
            {
                max_time = memory_node.Realtime;
                min_time = memory_node.Realtime - axis_time_limit_;

                memory_overview="TotalReservedMemory:";
                memory_overview.append(std::to_string((int)(memory_node.TotalReservedMemory)));
                memory_overview.append("MB MaxUsedMemory:");
                memory_overview.append(std::to_string((int)(memory_node.MaxUsedMemory)));
                memory_overview.append("MB TotalAllocatedMemory:");
                memory_overview.append(std::to_string((int)(memory_node.TotalAllocatedMemory)));
                memory_overview.append("MB\n MonoUsedSize:");
                memory_overview.append(std::to_string((int)(memory_node.MonoUsedSize)));
                memory_overview.append("MB TotalUnusedReserved:");
                memory_overview.append(std::to_string((int)(memory_node.TotalUnusedReserved)));
                memory_overview.append("MB");
            }
        }

        max_value+=50;

        bool show_fills = true;
        bool show_lines = true;

        if (ImPlot::BeginPlot("Memory")) {
            ImPlot::SetupAxes("time","memory");
            ImPlot::SetupAxisLimits(ImAxis_X1,min_time, max_time, ImGuiCond_Always);
            ImPlot::SetupAxisLimits(ImAxis_Y1,0, max_value, ImGuiCond_Always);

            if (show_lines) {
                ImPlot::PlotLine("TotalReservedMemory", real_time_data.data(), total_reserved_data.data(), data_size);
                ImPlot::PlotLine("TotalAllocatedMemory", real_time_data.data(), total_allocated_data.data(), data_size);
                ImPlot::PlotLine("MonoUsedSize", real_time_data.data(), mono_used_size_data.data(), data_size);
                ImPlot::PlotLine("MaxUsedMemory", real_time_data.data(), max_used_data.data(), data_size);
                ImPlot::PlotLine("MonoHeapSize", real_time_data.data(), mono_heap_data.data(), data_size);
                ImPlot::PlotLine("UsedHeapSize", real_time_data.data(), used_heap_data.data(), data_size);
                ImPlot::PlotLine("TotalUnusedReserved", real_time_data.data(), total_unused_reserved_data.data(), data_size);
                ImPlot::PlotLine("AllocatedMemoryForGraphicsDriver", real_time_data.data(), allocated_memory_for_graphics_driver_data.data(), data_size);
                ImPlot::PlotLine("TempAllocatorSize", real_time_data.data(), temp_allocator_size_data.data(), data_size);
            }
            ImPlot::EndPlot();
        }

        ImVec2 text_pos = ImGui::GetItemRectMin();
        ImGui::GetWindowDrawList()->AddText(text_pos,ImGui::ColorConvertFloat4ToU32(ImGui::GetStyleColorVec4(ImGuiCol_Text)), memory_overview.c_str());
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
        //通知界面开启
        Send("show");
        //查询类型
        ReqProfilerMessage req;
        req.CmdId = ReqProfilerCmd_GetMemorySampleTypes;
        json json_req = req;
        Send(json_req.dump());
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
    memory_nodes_.clear();
    memory_sample_types_.clear();
//    memory_sample_nodes_.clear();
}