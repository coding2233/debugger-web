//
// Created by wanderer on 2023/11/13.
//

#ifndef RUNTIMEDEBUGGER_LOG_WINDOW_H
#define RUNTIMEDEBUGGER_LOG_WINDOW_H

#include <map>
#include <string>
#include <vector>

#include "app_window.h"

class LogWindow : public AppWindow
{

public:
    LogWindow();
    ~LogWindow();

private:
    std::vector<std::string> logs_;
public:
    void OnMessage(const std::string &message) override;
    bool OnDraw() override;
};


#endif //RUNTIMEDEBUGGER_LOG_WINDOW_H
