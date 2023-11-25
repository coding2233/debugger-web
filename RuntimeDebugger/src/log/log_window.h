//
// Created by wanderer on 2023/11/13.
//

#ifndef RUNTIMEDEBUGGER_LOG_WINDOW_H
#define RUNTIMEDEBUGGER_LOG_WINDOW_H

#include <map>
#include <string>
#include <vector>

#include "app_window.h"
#include "log_node.h"

#define MAX_RUNTIME_DEBUGGER_LOG_NUMBER 10240

class LogWindow : public AppWindow
{
public:
    LogWindow();
    ~LogWindow();

private:
    std::vector<LogNode> log_nodes_;
    int log_node_index_;
    int log_node_selected_index_;
public:
    void OnMessage(const std::string &message) override;
    void OnDraw() override;
};


#endif //RUNTIMEDEBUGGER_LOG_WINDOW_H
