//
// Created by EDY on 2023/11/27.
//

#ifndef RUNTIMEDEBUGGER_TERMINAL_WINDOW_H
#define RUNTIMEDEBUGGER_TERMINAL_WINDOW_H

#include <vector>
#include <string>

#include "imgui.h"
#include "app_window.h"

class TerminalWindow : public AppWindow
{
public:
    TerminalWindow();
    ~TerminalWindow();

private:
    std::vector<std::string> log_nodes_;
    std::string input_text_;
public:
    void OnMessage(const std::string &message) override;
    void OnDraw() override;
};


#endif //RUNTIMEDEBUGGER_TERMINAL_WINDOW_H
