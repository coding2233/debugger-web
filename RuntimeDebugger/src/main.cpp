//
// Created by EDY on 2023/8/16.
//

#include <iostream>

#ifdef __EMSCRIPTEN__
#include <emscripten.h>
#endif

#define STB_IMAGE_IMPLEMENTATION

#include "imgui_impl_sdl2_opengl3_app.h"
#include "app.h"

App app;

void MainLoop()
{
    std::cout<< "emscripten_set_main_loop"<<std::endl;
    app.Run();
}

int main(int argc,char* args[])
{
    std::cout<< "Hello world"<<std::endl;
    app.RunBefore();
#ifdef __EMSCRIPTEN__
    emscripten_set_main_loop(MainLoop, 0, 1);
#else
    app.RunLoop();
#endif
    app.RunAfter();
    return 0;
}
