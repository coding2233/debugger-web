//
// Created by EDY on 2023/9/9.
//

#ifndef RZJH_LAUNCH_APP_H
#define RZJH_LAUNCH_APP_H

#include <thread>
#include <stdio.h>
#include <stdlib.h>
#include <vector>

#include "imgui_impl_sdl2_opengl3_app.h"
#include "TextEditor.h"

class App :public ImplApp
{
public:
    App();
    //App(int width,int height,int flags):ImplApp(width,height,flags);
    ~App();
private:

public:
    void OnImGuiDraw() override;
};


#endif //RZJH_LAUNCH_APP_H
