//
// Created by EDY on 2023/9/26.
//

#ifndef RZJH_LAUNCH_COMMAND_VIEW_H
#define RZJH_LAUNCH_COMMAND_VIEW_H

#ifdef _WIN32
#include <windows.h>
#define  popen _popen
#define  pclose _pclose
#endif
#include <thread>
#include <stdio.h>
#include <stdlib.h>
#include <vector>
#include <iostream>

#include "TextEditor.h"


class CommandView
{
public:
    CommandView();
    ~CommandView();

private:
    TextEditor* text_editor_;
    char buf[1024];

public:
    void Run(const char* command);
    void Render();
};


#endif //RZJH_LAUNCH_COMMAND_VIEW_H
