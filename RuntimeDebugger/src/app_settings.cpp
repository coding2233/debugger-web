#include "app_settings.h"

// 将虚拟文件系统与持久化存储同步
void AppSettings::SyncFileSystem() 
{
    #ifdef __EMSCRIPTEN__
        EM_ASM(
            FS.syncfs(false, function(err) {
                if (err) {
                    console.error('Failed to sync filesystem', err);
                } else {
                    console.log('Filesystem synced');
                }
            });
        );
    #endif
}

void AppSettings::SaveFile(const char* data, int size, const char* filename) 
{
    #ifdef __EMSCRIPTEN__
        EM_ASM_({
            var data = HEAPU8.subarray($0, $0 + $1);
            var blob = new Blob([data], { type: "octet/stream" });
            var url = window.URL.createObjectURL(blob);
            var a = document.createElement("a");
            a.href = url;
            a.download = UTF8ToString($2);
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
            document.body.removeChild(a);
        }, data, size, filename);
    #endif
}