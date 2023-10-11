//
// Created by EDY on 2023/8/16.
//

#include <iostream>

#include "git2.h"

#define STB_IMAGE_IMPLEMENTATION
//#include "imgui_impl_sdl_opengl3.h"
#include "imgui_impl_sdl2_opengl3_app.h"
#include "app.h"

//ImGuiContext* OnImGuiInit()
//{
//    return CreateImGuiContext();
//}
//
//void OnImGuiDraw()
//{
//    ShowImGuiDemoWindow();
//}

static void check_error(int error_code, const char *action)
{
    const git_error *error = git_error_last();
    if (!error_code)
        return;

    printf("Error %d %s - %s\n", error_code, action,
           (error && error->message) ? error->message : "???");

    exit(1);
}

int main(int argc,char* args[])
{
//    std::cout<< "Hello test main"<<std::endl;
//    static bool main_exit = true;
//
//    // CreateSdlWindow("test windiw",0,0,)
//    auto sdl_window= CreateSdlWindow("test window",1280,800,0);
//    CreateRender(sdl_window,OnImGuiInit,OnImGuiDraw,nullptr,&main_exit);
    std::cout<< "Hello world"<<std::endl;

    git_libgit2_init();
    git_repository *repo_;
    int error = git_repository_open(&repo_, "C:/Users/EDY/Desktop/subconverter-action/.git");
    check_error(error, "opening repository");

    App app;
    app.Run();

    return 0;
}
