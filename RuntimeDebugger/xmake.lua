add_requires("libsdl",{alias="sdl"})
-- add_requires("libcurl",{alias="curl"})
-- add_requires("libgit2", {configs = {shared = true}})

-- if is_plat("windows") then
-- add_requires("glew")
-- add_requires("opengl")
-- -- vs开发使用
-- add_rules("mode.release","mode.debug")
-- target("RuntimeDebugger_Windows")
--     set_license("MIT")
--     set_kind("binary")
--     set_languages("cxx17")
--     set_arch("x64")
--     add_files("imgui/**.cpp", "src/**.cpp")
--     add_includedirs("imgui","imgui/backends","src")
--     add_packages("sdl","glew","opengl")
-- end


target("RuntimeDebugger")
    set_license("MIT")
    set_kind("binary")
    set_languages("cxx17")
    set_arch("x64")
    add_files("imgui/**.cpp", "src/**.cpp")
    add_includedirs("imgui","imgui/backends","src")
    add_packages("sdl")

-- 生成clion可识别的 compile_commands
-- xmake project -k compile_commands
