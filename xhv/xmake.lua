add_requires("libhv 1.2.6",{alias="libhv"},{configs = {ipv6 = true,mbedtls = true}})

add_requires("miniz")
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
    -- 定义了 MINIZ_NO_ARCHIVE_WRITING_APIS 和 MINIZ_NO_STDIO 以减少 miniz 的编译大小
    add_defines("MINIZ_NO_ARCHIVE_WRITING_APIS", "MINIZ_NO_STDIO")
    -- set_arch("x64")
    add_files("src/**.cpp")
    add_packages("libhv","miniz")
    if is_plat("windows") then
        add_defines("TEST")
    end

target("xhv_test")
    set_kind("binary")
    set_languages("cxx17")
    -- 定义了 MINIZ_NO_ARCHIVE_WRITING_APIS 和 MINIZ_NO_STDIO 以减少 miniz 的编译大小
    add_defines("MINIZ_NO_ARCHIVE_WRITING_APIS", "MINIZ_NO_STDIO")
    -- set_arch("x64")
    add_files("src/**.cpp")
    add_packages("libhv","miniz")
    if is_plat("windows") then
        add_defines("TEST")
    end


-- xmake f -p windows -a x64
-- 生成clion可识别的 compile_commands
-- xmake project -k compile_commands

-- ./xhv.exe -p 12233 -c test.config
-- config: http://127.0.0.1:12233/service