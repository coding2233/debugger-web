//
// Created by wanderer on 2023/11/13.
//

#ifndef RUNTIMEDEBUGGER_LOG_WINDOW_H
#define RUNTIMEDEBUGGER_LOG_WINDOW_H

#include <map>
#include <string>
#include <vector>

#include "app_window.h"
// #include "log_node.h"
#include "debugger_protocol_log.pb.h"

#define MAX_RUNTIME_DEBUGGER_LOG_NUMBER 10240
static std::string log_type_str_[] = {"Error","Assert","Warning","Info","Exception"};
static ImVec4 log_type_color_[] = {ImVec4(1,0,0,1),ImVec4(1,0,0,1),ImVec4(1,1,0,1),ImVec4(1,1,1,1),ImVec4(1,0,0,1)};

class LogWindow : public AppWindow
{
public:
    LogWindow();
    ~LogWindow();

private:
    // std::vector<LogNode> log_nodes_;
    int log_node_index_;
    int log_node_selected_index_;
    
    std::vector<DebuggerProtocol::LogNode> log_nodes_;

public:
    void OnMessage(const std::string &message) override;
    void OnDraw() override;
};


#endif //RUNTIMEDEBUGGER_LOG_WINDOW_H
