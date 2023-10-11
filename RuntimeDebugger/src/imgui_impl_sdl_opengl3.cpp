
#include "imgui_impl_sdl_opengl3.h"
#include <windows.h> 
#include <SDL_syswm.h> 

 static char* glsl_version_;


ImGuiContext* CreateImGuiContext()
{
    return ImGui::CreateContext();
}

void ShowImGuiDemoWindow()
{
    ImGui::ShowDemoWindow();
}

SDL_Window* CreateSdlWindow(const char* title, int window_width, int window_height, Uint32 window_flags)
{
    // Decide GL+GLSL versions
#ifdef __APPLE__
    // GL 3.2 Core + GLSL 150
    glsl_version_ = "#version 150";
    SDL_GL_SetAttribute(SDL_GL_CONTEXT_FLAGS, SDL_GL_CONTEXT_FORWARD_COMPATIBLE_FLAG); // Always required on Mac
    SDL_GL_SetAttribute(SDL_GL_CONTEXT_PROFILE_MASK, SDL_GL_CONTEXT_PROFILE_CORE);
    SDL_GL_SetAttribute(SDL_GL_CONTEXT_MAJOR_VERSION, 3);
    SDL_GL_SetAttribute(SDL_GL_CONTEXT_MINOR_VERSION, 2);
#else
    // GL 3.0 + GLSL 130
    glsl_version_ = "#version 130";
    SDL_GL_SetAttribute(SDL_GL_CONTEXT_FLAGS, 0);
    SDL_GL_SetAttribute(SDL_GL_CONTEXT_PROFILE_MASK, SDL_GL_CONTEXT_PROFILE_CORE);
    SDL_GL_SetAttribute(SDL_GL_CONTEXT_MAJOR_VERSION, 3);
    SDL_GL_SetAttribute(SDL_GL_CONTEXT_MINOR_VERSION, 0);
#endif

    SDL_GL_SetAttribute(SDL_GL_DOUBLEBUFFER, 1);
    SDL_GL_SetAttribute(SDL_GL_DEPTH_SIZE, 24);
    SDL_GL_SetAttribute(SDL_GL_STENCIL_SIZE, 8);

    if (SDL_Init(SDL_INIT_VIDEO | SDL_INIT_EVENTS | SDL_INIT_GAMECONTROLLER) != 0)
    {
        std::cout << "Error initializing sdl: " << SDL_GetError() << std::endl;
        return nullptr;
    }
    SDL_EnableScreenSaver();
    SDL_EventState(SDL_DROPFILE, SDL_ENABLE);
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
    full_title.append(" - ImGui docking ");
    full_title.append(IMGUI_VERSION);

    auto window = SDL_CreateWindow(
            full_title.c_str(), SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, window_width, window_height,
         SDL_WINDOW_SHOWN | SDL_WINDOW_BORDERLESS | SDL_WINDOW_ALLOW_HIGHDPI | SDL_WINDOW_OPENGL | window_flags ); //SDL_WINDOW_RESIZABLE
    // init_window_icon();
    if (!window) {
        fprintf(stderr, "Error creating window: %s", SDL_GetError());
        // exit(1);
        return nullptr;
    }

    //SDL_SetWindowOpacity(window, 0.5f);

    return window;
}

struct Point
{
    int x;
    int y;
};

int CreateRender(SDL_Window* window,  IMGUI_INIT_CALLBACK imgui_init_cb,IMGUI_DRAW_CALLBACK imgui_draw_cb,SDL_EVENT_CALLBACK sdl_event_cb,bool* main_exit)
{
    auto gl_context = SDL_GL_CreateContext(window);
    SDL_GL_MakeCurrent(window, gl_context);
    // SDL_GL_SetSwapInterval(1); // Enable vsync

    //Check if Glew OpenGL loader is correct
    int err = glewInit();
    if (err != GLEW_OK)
    {
        std::cout << glewGetErrorString(err) << std::endl;
        fprintf(stderr, "Failed to initialize OpenGL loader!\n");
        return -1;
    }

    // Setup Dear ImGui context
    IMGUI_CHECKVERSION();
    // ImGui::CreateContext();
    auto imgui_context = imgui_init_cb();
    if (!imgui_context)
    {
        imgui_context = ImGui::CreateContext();
    }
    ImGui::SetCurrentContext(imgui_context);

//    ImGui::GetIO().ConfigFlags |= ImGuiConfigFlags_ViewportsEnable ;

  //  // Setup Platform/Renderer backends
    ImGui_ImplSDL2_InitForOpenGL(window, gl_context);
    ImGui_ImplOpenGL3_Init(glsl_version_);

    clock_t clock_start = clock();
    float default_fps = 60.0f;
    long frame_rate_time = (1 / default_fps)*1000;
    long frame = 0;

    ImVec4 clear_color_ = ImVec4(0.45f, 0.55f, 0.60f, 1.00f);
    bool is_done_ = false;

    int relative_mouse_x = 0;
    int relative_mouse_y = 0;
    int mouse_x = 0;
    int mouse_y = 0;
    int window_x = 0;
    int window_y = 0;
    int window_size_x = 0;
    int window_size_y = 0;
    bool dragging = false;
    bool resize = false;
    bool setWindow = false;

    bool drag_window ;

//    bool show_demo=true;
     while (!is_done_ && *main_exit)
     {

         long target_frame_time = frame * frame_rate_time;
         clock_t clock_run_time = clock();
         long run_time = clock_run_time - clock_start;
         //std::cout << "frame: " << frame << " runTime: " << run_time << " targetTime: " << target_frame_time << std::endl;

         if (run_time < target_frame_time)
         {
             Sleep(target_frame_time - run_time);
         }
         frame++;
         /*bool dragging = false;
         Point mousePos, windowPos, newWindowPos, newWindowSize, mWindowResizeOffset;
         HWND windowHandle;
         SDL_SysWMinfo windowInfo;
         SDL_GetWindowWMInfo(window, &windowInfo);
         windowHandle = windowInfo.info.win.window;*/
         //The surface contained by the window 
        // Poll and handle events (inputs, window resize, etc.)
        // You can read the io.WantCaptureMouse, io.WantCaptureKeyboard flags to tell if dear imgui wants to use your inputs.
        // - When io.WantCaptureMouse is true, do not dispatch mouse input data to your main application.
        // - When io.WantCaptureKeyboard is true, do not dispatch keyboard input data to your main application.
        // Generally you may always pass all inputs to dear imgui, and hide them from your application based on those two flags.

         SDL_Event event;
        int last_move_relative_mouse_x,last_move_relative_mouse_y;
        while (SDL_PollEvent(&event))
        {
            if (event.type == SDL_QUIT)
                is_done_ = true;

            //std::cout << "ImGui_ImplSDL2_ProcessEvent"<< event.type << std::endl;
            bool imgui_process_event = ImGui_ImplSDL2_ProcessEvent(&event);

            int mouse_x_0 = mouse_x;
            int mouse_y_0 = mouse_y;

            if (event.type == SDL_WINDOWEVENT) {
                if (event.window.event == SDL_WINDOWEVENT_CLOSE &&
                    event.window.windowID == SDL_GetWindowID(window)) {
                    is_done_ = true;
                }
                    /* else if (event.window.event == SDL_WINDOWEVENT_SIZE_CHANGED)
                     {

                     }*/
                else
                {
                    if (sdl_event_cb) {
                        sdl_event_cb(SDL_WINDOWEVENT, &event.window);
                    }

                    switch (event.window.event)
                    {
                        case SDL_WINDOWEVENT_FOCUS_GAINED:
                            clock_start = clock();
                            frame = 0;
                            frame_rate_time = (1 / default_fps) * 1000;
                            break;
                        case SDL_WINDOWEVENT_FOCUS_LOST:
                            clock_start = clock();
                            frame = 0;
                            //frame_rate_time = 1000;
                            frame_rate_time = (1 / 10.0f) * 1000;
                            break;
                        //鼠标进入 移出
                        case SDL_WINDOWEVENT_ENTER:
                            SDL_GetRelativeMouseState(&relative_mouse_x,&relative_mouse_y);
                            break;
                        case SDL_WINDOWEVENT_LEAVE:
                            relative_mouse_x= 0;
                            relative_mouse_y = 0;
                            break;
                    }


                }
            } else if (event.type == SDL_DROPFILE) {
                SDL_MaximizeWindow(window);

                if (sdl_event_cb) {
                    sdl_event_cb(SDL_DROPFILE, &event.drop);
                }
            } else if (event.type == SDL_MOUSEBUTTONDOWN) {

//                SDL_GetMouseState(&mouse_x, &mouse_y);
//                SDL_GetWindowPosition(window, &window_x, &window_y);
//                SDL_GetWindowSize(window, &window_size_x, &window_size_y);
//
//                std::cout << "SDL_MOUSEBUTTONDOWN " << " mouse_x:" << mouse_x << " mouse_y:" << mouse_y
//                          << " window_x:" << window_x << " window_y:" << window_y << " window_size_x:"
//                          << window_size_x << " window_size_y:" << window_size_y << std::endl;
//
//                if (mouse_x > window_size_x - 64 && mouse_x > window_size_y - 64) {
//                    resize = true;
//                    SDL_SetCursor(SDL_CreateSystemCursor(SDL_SYSTEM_CURSOR_SIZENWSE));
//                } else {
//                    dragging = true;
//                }
                dragging = true;

            } else if (event.type == SDL_MOUSEBUTTONUP) {
//                std::cout << "SDL_MOUSEBUTTONUP" << std::endl;
//                dragging = false;
//                resize = false;
//                SDL_SetCursor(SDL_CreateSystemCursor(SDL_SYSTEM_CURSOR_ARROW));
                dragging = false;
            } else if (event.type == SDL_MOUSEMOTION) {
//                if (dragging || resize)
//                {
//                    SDL_GetMouseState(&mouse_x_0, &mouse_y_0);
//                    int x_0 = mouse_x_0 - mouse_x;
//                    int y_0 = mouse_y_0 - mouse_y;
//                    bool reset = false;
//                    if (x_0 == 0 && y_0 == 0) {
//
//                    } else {
//                        reset = true;
//                        mouse_x = mouse_x_0 - x_0;
//                        mouse_y = mouse_y_0 - y_0;
////                        std::cout << "SDL_MOUSEMOTION " << "x:" << x_0 << " y:" << y_0 << "mouse_x_0:" << mouse_x_0
////                                  << "mouse_y_0:" << mouse_y_0 << std::endl;
//                        /*    window_new_x = window_x;
//                            window_new_y = window_y;*/
//
//                        //auto cursor = SDL_GetCursor();
//                        //mouse_x = mouse_x + x_0;
//                        //mouse_y = mouse_y + y_0;
//                        //window_x = window_x + x_0;
//                        //window_x = window_y + y_0;
//                    }
//
//                    if (reset) {
//
//                        if (dragging) {
//                            /*     mouse_x = mouse_x_0 - x_0;
//                                 mouse_y = mouse_x_0 - y_0;*/
//
//                            window_x = window_x + x_0;
//                            window_y = window_y + y_0;
//                            SDL_SetWindowPosition(window, window_x, window_y);
//                        }
//
//                        if (resize) {
//                            mouse_x = mouse_x_0;
//                            mouse_y = mouse_y_0;
//
//                            window_size_x = window_size_x + x_0;
//                            window_size_y = window_size_y + y_0;
//                            SDL_SetWindowSize(window, window_size_x, window_size_y);
//                        }
//                    }
//                }
//
//                int move_relative_mouse_x,move_relative_mouse_y;
//                SDL_GetRelativeMouseState(&move_relative_mouse_x,&move_relative_mouse_y);
//                std::cout << "move_relative_mouse_x:" << move_relative_mouse_x<< " move_relative_mouse_y:" << move_relative_mouse_y << std::endl;
//                int move_mouse_x = move_relative_mouse_x-relative_mouse_x;
//                int move_mouse_y = move_relative_mouse_y-relative_mouse_y;
//                relative_mouse_x = move_relative_mouse_x;
//                relative_mouse_y = move_relative_mouse_y;
                int move_relative_mouse_x,move_relative_mouse_y;
                SDL_GetRelativeMouseState(&move_relative_mouse_x, &move_relative_mouse_y);

                if (last_move_relative_mouse_x!=move_relative_mouse_x || last_move_relative_mouse_y!=move_relative_mouse_y)
                {
                    std::cout << "move_relative_mouse_x:" << move_relative_mouse_x<< " move_relative_mouse_y:" << move_relative_mouse_y << std::endl;
                    last_move_relative_mouse_x = move_relative_mouse_x;
                    last_move_relative_mouse_y = move_relative_mouse_y;

                    if (drag_window)
                    {
                        drag_window = false;
                    }
                    else
                    {
                        if (dragging)
                        {
                            int window__x = 0;
                            int window__y = 0;
                            SDL_GetWindowPosition(window, &window__x, &window__y);
                            SDL_SetWindowPosition(window, window__x + move_relative_mouse_x, window__y + move_relative_mouse_y);
                            drag_window = true;
                        }
                    }
                }


            }
//            SDL_Delay(1);
        }
        
         // Start the Dear ImGui frame
        ImGui_ImplOpenGL3_NewFrame();
        ImGui_ImplSDL2_NewFrame(window);

        ImGui::NewFrame();
        imgui_draw_cb();


         ImGui::Render();


         if (ImGui::GetIO().ConfigFlags & ImGuiConfigFlags_ViewportsEnable)
         {
             ImGui::UpdatePlatformWindows();
             ImGui::RenderPlatformWindowsDefault();
         }

         // ImGui::ShowDemoWindow(&show_demo);
        // ImGui::Render();
        
        // int window_width=0;
        // int window_height=0;
        // SDL_GetWindowSize(window,&window_width,&window_height);
        // glViewport(0, 0, window_width, window_height);
        ImGuiIO &io = ImGui::GetIO();
        glViewport(0, 0, (int)io.DisplaySize.x, (int)io.DisplaySize.y);
        glClearColor(clear_color_.x, clear_color_.y, clear_color_.z, clear_color_.w);
        glClear(GL_COLOR_BUFFER_BIT);

        // if(imgui_draw_render_cb)
        // {
        //     //RenderDrawData
        //     imgui_draw_render_cb();
        // }
        ImGui_ImplOpenGL3_RenderDrawData(ImGui::GetDrawData());


        SDL_GL_SwapWindow(window);
     }
    
     return 0;
}

void SDLSetWindowShow(SDL_Window* sdl_window)
{
    SDL_ShowWindow(sdl_window);
    //SDL_MaximizeWindow(sdl_window);
}

bool LoadTextureFromMemory(const unsigned char* buffer,int size, GLuint* out_texture, int* out_width, int* out_height)
{
    // Load from file
    int image_width = 0;
    int image_height = 0;
    unsigned char* image_data = stbi_load_from_memory(buffer, size ,&image_width, &image_height, NULL, 4);
    if (image_data == NULL)
        return false;

    // Create a OpenGL texture identifier
    GLuint image_texture;
    glGenTextures(1, &image_texture);
    glBindTexture(GL_TEXTURE_2D, image_texture);

    // Setup filtering parameters for display
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE); // This is required on WebGL for non power-of-two textures
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE); // Same

    // Upload pixels into texture
//#if defined(GL_UNPACK_ROW_LENGTH) && !defined(__EMSCRIPTEN__)
//    glPixelStorei(GL_UNPACK_ROW_LENGTH, 0);
//#endif
    glPixelStorei(GL_UNPACK_ROW_LENGTH, 0);

    glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, image_width, image_height, 0, GL_RGBA, GL_UNSIGNED_BYTE, image_data);
    stbi_image_free(image_data);

    *out_texture = image_texture;
    *out_width = image_width;
    *out_height = image_height;

    return true;
}

bool LoadTextureFromFile(const char* filename, GLuint* out_texture, int* out_width, int* out_height)
{
  // Load from file
    int image_width = 0;
    int image_height = 0;
    unsigned char* image_data = stbi_load(filename, &image_width, &image_height, NULL, 4);
    if (image_data == NULL)
        return false;

    // Create a OpenGL texture identifier
    GLuint image_texture;
    glGenTextures(1, &image_texture);
    glBindTexture(GL_TEXTURE_2D, image_texture);

    // Setup filtering parameters for display
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE); // This is required on WebGL for non power-of-two textures
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE); // Same

    // Upload pixels into texture
//#if defined(GL_UNPACK_ROW_LENGTH) && !defined(__EMSCRIPTEN__)
//    glPixelStorei(GL_UNPACK_ROW_LENGTH, 0);
//#endif
    glPixelStorei(GL_UNPACK_ROW_LENGTH, 0);

    glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, image_width, image_height, 0, GL_RGBA, GL_UNSIGNED_BYTE, image_data);
    stbi_image_free(image_data);

    *out_texture = image_texture;
    *out_width = image_width;
    *out_height = image_height;

    return true;
}

void DeleteTexture(GLuint* out_texture)
{
    if (out_texture)
    {
        glDeleteTextures(1, out_texture);
    }
}


void LinkCallback(ImGui::MarkdownLinkCallbackData data_)
{
    std::string url(data_.link, data_.linkLength);
    if (!data_.isImage)
    {
        ShellExecuteA(NULL, "open", url.c_str(), NULL, NULL, SW_SHOWNORMAL);
    }
}

inline ImGui::MarkdownImageData ImageCallback(ImGui::MarkdownLinkCallbackData data_)
{
    // In your application you would load an image based on data_ input. Here we just use the imgui font texture.
    ImTextureID image = ImGui::GetIO().Fonts->TexID;
    // > C++14 can use ImGui::MarkdownImageData imageData{ true, false, image, ImVec2( 40.0f, 20.0f ) };
    ImGui::MarkdownImageData imageData;
    imageData.isValid = true;
    imageData.useLinkCallback = false;
    imageData.user_texture_id = image;
    imageData.size = ImVec2(40.0f, 20.0f);

    // For image resize when available size.x > image width, add
    ImVec2 const contentSize = ImGui::GetContentRegionAvail();
    if (imageData.size.x > contentSize.x)
    {
        float const ratio = imageData.size.y / imageData.size.x;
        imageData.size.x = contentSize.x;
        imageData.size.y = contentSize.x * ratio;
    }

    return imageData;
}

static MARKDOWN_HEADING_CALLBACK* heading_callback_;
void ExampleMarkdownFormatCallback(const ImGui::MarkdownFormatInfo& markdownFormatInfo_, bool start_)
{
    // Call the default first so any settings can be overwritten by our implementation.
    // Alternatively could be called or not called in a switch statement on a case by case basis.
    // See defaultMarkdownFormatCallback definition for furhter examples of how to use it.
    ImGui::defaultMarkdownFormatCallback(markdownFormatInfo_, start_);

    switch (markdownFormatInfo_.type)
    {
        // example: change the colour of heading level 2
    case ImGui::MarkdownFormatType::HEADING:
    {
        if (heading_callback_)
        {
            heading_callback_(markdownFormatInfo_.level, start_);
        }
        /*if (markdownFormatInfo_.level == 2)
        {
            if (start_)
            {
                ImGui::PushStyleColor(ImGuiCol_Text, ImGui::GetStyle().Colors[ImGuiCol_TextDisabled]);
            }
            else
            {
                ImGui::PopStyleColor();
            }
        }*/
        break;
    }
    default:
    {
        break;
    }
    }
}
void MarkDown(const char* text, size_t size, ImGui::MarkdownImageCallback *image_callback, MARKDOWN_HEADING_CALLBACK* heading_callback)
{
    static ImGui::MarkdownConfig mdConfig;

    heading_callback_ = heading_callback;
    mdConfig.linkCallback = LinkCallback;
    mdConfig.tooltipCallback = NULL;
    mdConfig.imageCallback = image_callback;
    //mdConfig.linkIcon = ICON_FA_LINK;
    //mdConfig.headingFormats[0] = { H1, true };
    //mdConfig.headingFormats[1] = { H2, true };
    //mdConfig.headingFormats[2] = { H3, false };
    mdConfig.userData = NULL;
    mdConfig.formatCallback = ExampleMarkdownFormatCallback;

    //ImGui::Markdown(text,size, mdConfig);

    ImGui::Markdown(text, size, mdConfig);
}


void SetClipboard(const char* text)
{
#if defined _WIN32
    HWND hWnd = NULL;
    OpenClipboard(hWnd);
    EmptyClipboard();
    HANDLE hHandle = GlobalAlloc(GMEM_FIXED, 1024);
    char* pData = (char*)GlobalLock(hHandle);
    strcpy(pData, text);
    SetClipboardData(CF_TEXT,hHandle);
    GlobalUnlock(hHandle);
    CloseClipboard();
#endif
}

// void RenderDrawData(struct ImDrawData* draw_data)
// {
//     ImGui_ImplOpenGL3_RenderDrawData(draw_data);
// }
