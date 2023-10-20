//
// Created by EDY on 2023/8/29.
//

#ifndef IMGUI_IMPL_SDL2_OPENGL3_APP_H
#define IMGUI_IMPL_SDL2_OPENGL3_APP_H


#include <iostream>
#ifdef __EMSCRIPTEN__
#include <SDL_opengles2.h>
#include <emscripten.h>
#include <GLES3/gl3.h>
#else
#include <GL/glew.h>
#endif
#include <SDL.h>
#include <stdio.h>

#include "imgui.h"
#include "imgui_impl_sdl2.h"
#include "imgui_impl_opengl3.h"

class ImplApp
{

public:
    ImplApp(const char* title, int window_width, int window_height, Uint32 window_flags);
    ~ImplApp();

private:
    int drag_menu_height_;
    bool window_move_dragging_= false;
    bool  window_resize_dragging_= false;
    SDL_Cursor *resize_cursor_;
    const int resize_range_ = 8;
    bool  show_resize_cursor_ = false;
    bool mouse_pressing_ = false;

    SDL_Window* sdl_window_;
    bool is_done_;
    long frame_;
    ImVec4 clear_color_;
    clock_t clock_start_;
    long frame_rate_time_;

    SDL_Window* CreateWindow(const char* title, int window_width, int window_height, Uint32 window_flags);
    int CreateRender(SDL_Window* window);
    void PollEvent(SDL_Event event);
    void ImGuiAppMenuDraw();
public:
    virtual void RunBefore();
    virtual void RunAfter();

    void RunLoop();
    void Run();

    virtual ImGuiContext* OnImGuiContextCreate();
    virtual void OnImGuiAppMenuDraw();
    virtual void OnImGuiDraw();


};


#endif
