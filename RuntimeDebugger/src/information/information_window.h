//
// Created by EDY on 2023/11/13.
//

#ifndef RUNTIMEDEBUGGER_INFORMATIONWINDOW_H
#define RUNTIMEDEBUGGER_INFORMATIONWINDOW_H

#include <map>
#include <string>

#include "app_window.h"

class InformationWindow:public AppWindow
{
public:
    InformationWindow();
    ~InformationWindow();

private:
    std::map<std::string,std::map<std::string,std::string>> information_;
    void OnDraw() override;
    void OnMessage(const std::string &message) override;
};


#endif //RUNTIMEDEBUGGER_INFORMATIONWINDOW_H
