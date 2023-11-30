//
// Created by EDY on 2023/8/29.
//

#include "imgui_impl_sdl2_opengl3_app.h"

ImplApp::ImplApp(const char* title, int window_width, int window_height, Uint32 window_flags)
{
    resize_cursor_ = SDL_CreateSystemCursor(SDL_SYSTEM_CURSOR_SIZENWSE);
    sdl_window_ = CreateWindow(title,window_width,window_height,window_flags);
    int result = CreateRender(sdl_window_);
    std::cout<< "CreateRender:"<<result<<std::endl;
}

ImplApp::~ImplApp()
{
    if (resize_cursor_)
    {
        SDL_FreeCursor(resize_cursor_);
    }
}

SDL_Window* ImplApp::CreateWindow(const char* title, int window_width, int window_height, Uint32 window_flags)
{

    if (SDL_Init(SDL_INIT_VIDEO | SDL_INIT_EVENTS | SDL_INIT_TIMER | SDL_INIT_GAMECONTROLLER) != 0)
    {
        std::cout << "Error initializing sdl: " << SDL_GetError() << std::endl;
        return nullptr;
    }

#if defined(IMGUI_IMPL_OPENGL_ES2)
    // GL ES 2.0 + GLSL 100
    SDL_GL_SetAttribute(SDL_GL_CONTEXT_FLAGS, 0);
    SDL_GL_SetAttribute(SDL_GL_CONTEXT_PROFILE_MASK, SDL_GL_CONTEXT_PROFILE_ES);
    SDL_GL_SetAttribute(SDL_GL_CONTEXT_MAJOR_VERSION, 2);
    SDL_GL_SetAttribute(SDL_GL_CONTEXT_MINOR_VERSION, 0);
#elif defined(__APPLE__)
    // GL 3.2 Core + GLSL 150
    SDL_GL_SetAttribute(SDL_GL_CONTEXT_FLAGS, SDL_GL_CONTEXT_FORWARD_COMPATIBLE_FLAG); // Always required on Mac
    SDL_GL_SetAttribute(SDL_GL_CONTEXT_PROFILE_MASK, SDL_GL_CONTEXT_PROFILE_CORE);
    SDL_GL_SetAttribute(SDL_GL_CONTEXT_MAJOR_VERSION, 3);
    SDL_GL_SetAttribute(SDL_GL_CONTEXT_MINOR_VERSION, 2);
#else
    // GL 3.0 + GLSL 130
    SDL_GL_SetAttribute(SDL_GL_CONTEXT_FLAGS, 0);
    SDL_GL_SetAttribute(SDL_GL_CONTEXT_PROFILE_MASK, SDL_GL_CONTEXT_PROFILE_CORE);
    SDL_GL_SetAttribute(SDL_GL_CONTEXT_MAJOR_VERSION, 3);
    SDL_GL_SetAttribute(SDL_GL_CONTEXT_MINOR_VERSION, 0);
#endif

    SDL_GL_SetAttribute(SDL_GL_DOUBLEBUFFER, 1);
    SDL_GL_SetAttribute(SDL_GL_DEPTH_SIZE, 24);
    SDL_GL_SetAttribute(SDL_GL_STENCIL_SIZE, 8);


    SDL_EnableScreenSaver();
    // SDL_EventState(SDL_DROPFILE, SDL_ENABLE);
    // atexit(SDL_Quit);

#ifdef SDL_HINT_VIDEO_X11_NET_WM_BYPASS_COMPOSITOR /* Available since 2.0.8 */
    SDL_SetHint(SDL_HINT_VIDEO_X11_NET_WM_BYPASS_COMPOSITOR, "0");
#endif
#if SDL_VERSION_ATLEAST(2, 0, 5)
    SDL_SetHint(SDL_HINT_MOUSE_FOCUS_CLICKTHROUGH, "1");
#endif
#if SDL_VERSION_ATLEAST(2, 0, 18)
    SDL_SetHint(SDL_HINT_IME_SHOW_UI, "1");
#endif
#if SDL_VERSION_ATLEAST(2, 0, 22)
    SDL_SetHint(SDL_HINT_IME_SUPPORT_EXTENDED_TEXT, "1");
#endif

#if SDL_VERSION_ATLEAST(2, 0, 8)
    /* This hint tells SDL to respect borderless window as a normal window.
    ** For example, the window will sit right on top of the taskbar instead
    ** of obscuring it. */
    SDL_SetHint("SDL_BORDERLESS_WINDOWED_STYLE", "1");
#endif
#if SDL_VERSION_ATLEAST(2, 0, 12)
    /* This hint tells SDL to allow the user to resize a borderless windoow.
    ** It also enables aero-snap on Windows apparently. */
    SDL_SetHint("SDL_BORDERLESS_RESIZABLE_STYLE", "1");
#endif
#if SDL_VERSION_ATLEAST(2, 0, 9)
    SDL_SetHint("SDL_MOUSE_DOUBLE_CLICK_RADIUS", "4");
#endif

    SDL_DisplayMode dm;
    SDL_GetCurrentDisplayMode(0, &dm);

    //SDL_WINDOWPOS_CENTERED   SDL_WINDOWPOS_UNDEFINED
    //
    if (window_width <= 0)
    {
        window_width = dm.w * 0.8;
    }
    if (window_height <= 0)
    {
        window_height = dm.h * 0.8;
    }

    //添加ImGui的版本号
    std::string full_title = std::string(title);
//    full_title.append(" - ImGui docking ");
//    full_title.append(IMGUI_VERSION);

// #ifndef __EMSCRIPTEN__
//     window_flags |= SDL_WINDOW_BORDERLESS;
// #endif

    auto window = SDL_CreateWindow(
            full_title.c_str(), SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, window_width, window_height,
            SDL_WINDOW_ALLOW_HIGHDPI | SDL_WINDOW_OPENGL| SDL_WINDOW_RESIZABLE | window_flags ); //
    // init_window_icon();
    if (!window) {
        fprintf(stderr, "Error creating window: %s", SDL_GetError());
        // exit(1);
        return nullptr;
    }
    return window;
}


int ImplApp::CreateRender(SDL_Window* window)
{
#if defined(IMGUI_IMPL_OPENGL_ES2)
    // GL ES 2.0 + GLSL 100
    const char *glsl_version = "#version 100";
#elif defined(__APPLE__)
    // GL 3.2 Core + GLSL 150
    const char *glsl_version = "#version 150";
#else
    // GL 3.0 + GLSL 130
    const char *glsl_version = "#version 130";
#endif

    auto gl_context = SDL_GL_CreateContext(window);
    SDL_GL_MakeCurrent(window, gl_context);
    // SDL_GL_SetSwapInterval(1); // Enable vsync

#ifndef __EMSCRIPTEN__
    //Check if Glew OpenGL loader is correct
    int err = glewInit();
    if (err != GLEW_OK)
    {
        std::cout << glewGetErrorString(err) << std::endl;
        fprintf(stderr, "Failed to initialize OpenGL loader!\n");
        return -1;
    }
#endif

    // Setup Dear ImGui context
    IMGUI_CHECKVERSION();
    // ImGui::CreateContext();
    auto imgui_context = OnImGuiContextCreate();
    ImGui::SetCurrentContext(imgui_context);

//    ImGui::GetIO().ConfigFlags |= ImGuiConfigFlags_ViewportsEnable ;
    ImGui::GetIO().ConfigFlags |= ImGuiConfigFlags_DockingEnable;
    ImGui::GetIO().BackendFlags |= ImGuiBackendFlags_RendererHasVtxOffset;

    //  // Setup Platform/Renderer backends
    ImGui_ImplSDL2_InitForOpenGL(window, gl_context);
    ImGui_ImplOpenGL3_Init(glsl_version);

    return 0;
}


void ImplApp::RunBefore()
{
    clock_start_ = clock();
    float default_fps = 60.0f;
    frame_rate_time_ = (1 / default_fps)*1000;
    frame_ = 0;
    is_done_ = false;
    clear_color_ = ImVec4(0.45f, 0.55f, 0.60f, 1.00f);
}

void ImplApp::RunAfter()
{


}

void ImplApp::RunLoop()
{
    while (!is_done_)
    {
        Run();
    }
}


void ImplApp::Run()
{
    long target_frame_time = frame_ * frame_rate_time_;
    clock_t clock_run_time = clock();
    long run_time = clock_run_time - clock_start_;
    //std::cout << "frame: " << frame << " runTime: " << run_time << " targetTime: " << target_frame_time << std::endl;

    if (run_time < target_frame_time)
    {
        SDL_Delay(target_frame_time - run_time);
//            Sleep(target_frame_time - run_time);
    }
    frame_++;


    //The surface contained by the sdl_window_
    // Poll and handle events (inputs, sdl_window_ resize, etc.)
    // You can read the io.WantCaptureMouse, io.WantCaptureKeyboard flags to tell if dear imgui wants to use your inputs.
    // - When io.WantCaptureMouse is true, do not dispatch mouse input data to your main application.
    // - When io.WantCaptureKeyboard is true, do not dispatch keyboard input data to your main application.
    // Generally you may always pass all inputs to dear imgui, and hide them from your application based on those two flags.

    SDL_Event event;
    while (SDL_PollEvent(&event))
    {
        if (event.type == SDL_QUIT)
            is_done_ = true;

        bool imgui_process_event = ImGui_ImplSDL2_ProcessEvent(&event);
        if (event.type == SDL_WINDOWEVENT)
        {
            if (event.window.event == SDL_WINDOWEVENT_CLOSE &&
                event.window.windowID == SDL_GetWindowID(sdl_window_)) {
                is_done_ = true;
            }
            else
            {
                switch (event.window.event)
                {
                    case SDL_WINDOWEVENT_FOCUS_GAINED:
                        clock_start_ = clock();
                        frame_ = 0;
                        frame_rate_time_ = (1 / 60) * 1000;
                        break;
                    case SDL_WINDOWEVENT_FOCUS_LOST:
                        clock_start_ = clock();
                        frame_ = 0;
                        //frame_rate_time = 1000;
                        frame_rate_time_ = (1 / 30.0f) * 1000;
                        break;
                }
            }
        }
        // PollEvent(event);
    }

    // Start the Dear ImGui frame
    ImGui_ImplOpenGL3_NewFrame();
    ImGui_ImplSDL2_NewFrame(sdl_window_);

    ImGui::NewFrame();
    if (!window_resize_dragging_)
    {
        ImGuiAppMenuDraw();
        OnImGuiDraw();
    }

    ImGui::Render();
    ImGuiIO &io = ImGui::GetIO();
    glViewport(0, 0, (int) io.DisplaySize.x, (int) io.DisplaySize.y);
    glClearColor(clear_color_.x, clear_color_.y, clear_color_.z, clear_color_.w);
    glClear(GL_COLOR_BUFFER_BIT);

    ImGui_ImplOpenGL3_RenderDrawData(ImGui::GetDrawData());
    SDL_GL_SwapWindow(sdl_window_);
}

void ImplApp::PollEvent(SDL_Event event)
{
    static int mouse_move_x = 0;
    static int mouse_move_y = 0;
    static int window_size_x= 0;
    static int window_size_y = 0;
    SDL_GetWindowSize(sdl_window_,&window_size_x,&window_size_y);

    if (event.type == SDL_WINDOWEVENT)
    {
    } else if (event.type == SDL_DROPFILE) {

    } else if (event.type == SDL_MOUSEBUTTONDOWN) {
        SDL_GetMouseState(&mouse_move_x,&mouse_move_y);
        if (mouse_move_y < drag_menu_height_)
        {
            window_move_dragging_ = true;
        }
        else if (mouse_move_x>window_size_x-resize_range_ &&  mouse_move_x>window_size_y-resize_range_)
        {
            window_resize_dragging_ = true;
        }
        mouse_pressing_ = true;

    } else if (event.type == SDL_MOUSEBUTTONUP) {
        mouse_pressing_ = false;
        window_move_dragging_ = false;
        window_resize_dragging_ = false;
    } else if (event.type == SDL_MOUSEMOTION) {
        int mouse_x, mouse_y;
        SDL_GetMouseState(&mouse_x,&mouse_y);
        if (mouse_move_x != mouse_x || mouse_move_y != mouse_y)
        {
            int x_0 = mouse_x - mouse_move_x;
            int y_0 = mouse_y - mouse_move_y;
            if (window_move_dragging_)
            {
                int window__x = 0;
                int window__y = 0;
                SDL_GetWindowPosition(sdl_window_, &window__x, &window__y);
                SDL_SetWindowPosition(sdl_window_, window__x + x_0,
                                      window__y + y_0);
                mouse_x -= x_0;
                mouse_y -= y_0;
            }
            mouse_move_x = mouse_x;
            mouse_move_y = mouse_y;


            if (mouse_x>window_size_x-resize_range_ &&  mouse_y>window_size_y-resize_range_)
            {
                if (!show_resize_cursor_)
                {
                    auto cursor = SDL_CreateSystemCursor(SDL_SYSTEM_CURSOR_SIZENWSE);
                    SDL_SetCursor(cursor);
                    show_resize_cursor_ = true;
                }
            }
            else
            {
                if (show_resize_cursor_ && !window_resize_dragging_)
                {
                    SDL_SetCursor(SDL_GetDefaultCursor());
                    show_resize_cursor_ = false;
                }
            }

            if(window_resize_dragging_)
            {
                window_size_x+= x_0;
                window_size_y+= y_0;
                SDL_SetWindowSize(sdl_window_,window_size_x,window_size_y);
            }

        }

    }

}

ImGuiContext* ImplApp::OnImGuiContextCreate()
{
    return  ImGui::CreateContext();
}

void ImplApp::ImGuiAppMenuDraw()
{
//    ImGui::BeginMainMenuBar();
//    OnImGuiAppMenuDraw();
//    ImGui::EndMainMenuBar();
//
//    if(!window_move_dragging_)
//    {
//        //更新拖拽的高度
//        drag_menu_height_ = ImGui::GetItemRectSize().y;
//    }
}

void ImplApp::OnImGuiAppMenuDraw()
{
    if(ImGui::MenuItem("exit"))
    {
        exit(0);
    }
}

void ImplApp::OnImGuiDraw()
{
    static bool demo_show=true;
    ImGui::ShowDemoWindow(&demo_show);
}