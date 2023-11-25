add_requires("libhv 1.2.6",{alias="libhv"})
add_requires("libzip")
-- add_requires("opengl")
-- add_requires("libcurl",{alias="curl"})

-- vs开发使用
-- add_rules("mode.release","mode.debug")

target("xhv")
    if is_plat("iphoneos") then
        set_kind("static")
    else
        set_kind("shared")
    end
    set_languages("cxx17")
    -- set_arch("x64")
    add_files("src/**.cpp")
    add_packages("libhv","libzip")
    if is_plat("windows") then
        add_defines("TEST")
    end

target("xhv_test")
    set_kind("binary")
    set_languages("cxx17")
    -- set_arch("x64")
    add_files("src/**.cpp")
    add_packages("libhv","libzip")
    if is_plat("windows") then
        add_defines("TEST")
    end


-- xmake f -p windows -a x64
-- 生成clion可识别的 compile_commands
-- xmake project -k compile_commands

-- ./xhv.exe -p 12233 -c test.config
-- config: http://127.0.0.1:12233/service