add_requires("libsdl",{alias="sdl"})

-- add_requires("libcurl",{alias="curl"})
-- add_requires("libgit2", {configs = {shared = true}})

-- xmake f -p wasm
if is_plat("wasm") then
    -- xmake f -v -y -p wasm --links=websocket.js
    target("RuntimeDebugger")
        set_license("MIT")
        set_kind("binary")
        set_languages("cxx17")
        set_arch("x64")
        add_files("imgui/**.cpp", "src/**.cpp")
        add_includedirs("imgui","imgui/backends","src")
        add_files("protobuf-wasm/lib/libprotobuf-c.a","protobuf-wasm/lib/libprotobuf.a")
        add_includedirs("protobuf-wasm/include")
        add_includedirs("protocol")
        add_files("protocol/**.c")
        add_files("easywsclient/emscripten_easywsclient.cpp")
        add_includedirs("easywsclient")
        add_packages("sdl")
else
    -- xmake f -p windows -a x64
    add_requires("glew")
    add_requires("opengl")
    add_requires("protobuf-c")
    -- add_requires("libcurl")
    -- vs开发使用
    add_rules("mode.release","mode.debug")
    target("RuntimeDebugger")
        set_license("MIT")
        set_kind("binary")
        set_languages("cxx17")
        set_arch("x64")
        add_files("imgui/**.cpp", "src/**.cpp")
        add_includedirs("imgui","imgui/backends","src")
        add_includedirs("protocol")
        add_files("protocol/**.c")
        add_files("easywsclient/easywsclient.cpp")
        add_includedirs("easywsclient")
        add_packages("sdl","glew","opengl","protobuf-c")
end

-- 生成clion可识别的 compile_commands
-- xmake project -k compile_commands
