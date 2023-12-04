#include "hv/hv.h"
#include "hv/hssl.h"
#include "hv/hmain.h"
#include "hv/iniparser.h"

#include "hv/HttpServer.h"
#include "hv/WebSocketServer.h"
#include "hv/htime.h"
#include "hv/EventLoop.h"

#include "router.h"
#include "httpd_export.h"
#include "runtime_debugger_web_data.h"

#include <mbedtls/entropy.h>
#include <mbedtls/ctr_drbg.h>
#include <mbedtls/bignum.h>
#include <mbedtls/x509_crt.h>
#include <mbedtls/rsa.h>
#include <mbedtls/error.h>
#include <mbedtls/pk.h>
#include <mbedtls/x509_csr.h>

#include <stdio.h>
#include <string.h>

hv::HttpServer  g_http_server;
hv::HttpService g_http_service;
RuntimeDebuggerWebData g_runtime_debugger_web_data;

static void print_version();
static void print_help();

static int  parse_confile(const char* confile);

// short options
static const char options[] = "hvc:ts:dp:";
// long options
static const option_t long_options[] = {
    {'h', "help",       NO_ARGUMENT},
    {'v', "version",    NO_ARGUMENT},
    {'c', "confile",    REQUIRED_ARGUMENT},
    {'t', "test",       NO_ARGUMENT},
    {'s', "signal",     REQUIRED_ARGUMENT},
    {'d', "daemon",     NO_ARGUMENT},
    {'p', "port",       REQUIRED_ARGUMENT}
};
static const char detail_options[] = R"(
  -h|--help                 Print this information
  -v|--version              Print version
  -c|--confile <confile>    Set configure file, default etc/{program}.conf
  -t|--test                 Test configure file and exit
  -s|--signal <signal>      Send <signal> to process,
                            <signal>=[start,stop,restart,status,reload]
  -d|--daemon               Daemonize
  -p|--port <port>          Set listen port
)";

void print_version() {
    printf("%s version %s\n", g_main_ctx.program_name, hv_compile_version());
}

void print_help() {
    printf("Usage: %s [%s]\n", g_main_ctx.program_name, options);
    printf("Options:\n%s\n", detail_options);
}

int parse_confile(const char* confile) {
    IniParser ini;
    int ret = ini.LoadFromFile(confile);
    if (ret != 0) {
        printf("Load confile [%s] failed: %d\n", confile, ret);
        exit(-40);
    }

    // logfile
    std::string str = ini.GetValue("logfile");
    if (!str.empty()) {
        strncpy(g_main_ctx.logfile, str.c_str(), sizeof(g_main_ctx.logfile));
    }
    hlog_set_file(g_main_ctx.logfile);
    // loglevel
    str = ini.GetValue("loglevel");
    if (!str.empty()) {
        hlog_set_level_by_str(str.c_str());
    }
    // log_filesize
    str = ini.GetValue("log_filesize");
    if (!str.empty()) {
        hlog_set_max_filesize_by_str(str.c_str());
    }
    // log_remain_days
    str = ini.GetValue("log_remain_days");
    if (!str.empty()) {
        hlog_set_remain_days(atoi(str.c_str()));
    }
    // log_fsync
    str = ini.GetValue("log_fsync");
    if (!str.empty()) {
        logger_enable_fsync(hlog, hv_getboolean(str.c_str()));
    }
    hlogi("%s version: %s", g_main_ctx.program_name, hv_compile_version());
    hlog_fsync();

    // worker_processes
    int worker_processes = 0;
#ifdef DEBUG
    // Disable multi-processes mode for debugging
    worker_processes = 0;
#else
    str = ini.GetValue("worker_processes");
    if (str.size() != 0) {
        if (strcmp(str.c_str(), "auto") == 0) {
            worker_processes = get_ncpu();
            hlogd("worker_processes=ncpu=%d", worker_processes);
        }
        else {
            worker_processes = atoi(str.c_str());
        }
    }
#endif
    g_http_server.worker_processes = LIMIT(0, worker_processes, MAXNUM_WORKER_PROCESSES);
    // worker_threads
    int worker_threads = 0;
    str = ini.GetValue("worker_threads");
    if (str.size() != 0) {
        if (strcmp(str.c_str(), "auto") == 0) {
            worker_threads = get_ncpu();
            hlogd("worker_threads=ncpu=%d", worker_threads);
        }
        else {
            worker_threads = atoi(str.c_str());
        }
    }
    g_http_server.worker_threads = LIMIT(0, worker_threads, 64);

    // http_port
    int port = 0;
    const char* szPort = get_arg("p");
    if (szPort) {
        port = atoi(szPort);
    }
    if (port == 0) {
        port = ini.Get<int>("port");
    }
    if (port == 0) {
        port = ini.Get<int>("http_port");
    }

    g_http_server.port = port;

    // https_port
    if (HV_WITH_SSL) {
        g_http_server.https_port = ini.Get<int>("https_port");
    }
    if (g_http_server.port == 0 && g_http_server.https_port == 0) {
        printf("Please config listen port!\n");
        exit(-10);
    }

    // base_url
    str = ini.GetValue("base_url");
    if (str.size() != 0) {
        g_http_service.base_url = str;
    }
    // document_root
    str = ini.GetValue("document_root");
    if (str.size() != 0) {
        g_http_service.document_root = str;
    }
    // home_page
    str = ini.GetValue("home_page");
    if (str.size() != 0) {
        g_http_service.home_page = str;
    }
    // error_page
    str = ini.GetValue("error_page");
    if (str.size() != 0) {
        g_http_service.error_page = str;
    }
    // index_of
    str = ini.GetValue("index_of");
    if (str.size() != 0) {
        g_http_service.index_of = str;
    }
    // limit_rate
    str = ini.GetValue("limit_rate");
    if (str.size() != 0) {
        g_http_service.limit_rate = atoi(str.c_str());
    }
    // ssl
    if (g_http_server.https_port > 0) {
        std::string crt_file = ini.GetValue("ssl_certificate");
        std::string key_file = ini.GetValue("ssl_privatekey");
        std::string ca_file = ini.GetValue("ssl_ca_certificate");
        hlogi("SSL backend is %s", hssl_backend());
        hssl_ctx_init_param_t param;
        memset(&param, 0, sizeof(param));
        param.crt_file = crt_file.c_str();
        param.key_file = key_file.c_str();
        param.ca_file = ca_file.c_str();
        param.endpoint = HSSL_SERVER;
        if (hssl_ctx_init(&param) == NULL) {
            hloge("SSL certificate verify failed!");
            exit(0);
        }
        else {
            hlogi("SSL certificate verify ok!");
        }
    }

    hlogi("parse_confile('%s') OK", confile);
    return 0;
}

static void on_reload(void* userdata) {
    hlogi("reload confile [%s]", g_main_ctx.confile);
    parse_confile(g_main_ctx.confile);
}

int main002(int argc, char** argv) {
    // g_main_ctx
    main_ctx_init(argc, argv);
    //int ret = parse_opt(argc, argv, options);
    int ret = parse_opt_long(argc, argv, long_options, ARRAY_SIZE(long_options));
    if (ret != 0) {
        print_help();
        exit(ret);
    }

    // help
    if (get_arg("h")) {
        print_help();
        exit(0);
    }

    // version
    if (get_arg("v")) {
        print_version();
        exit(0);
    }

    // parse_confile
//    const char* confile = get_arg("c");
//    if (confile) {
//        strncpy(g_main_ctx.confile, confile, sizeof(g_main_ctx.confile));
//    }
//    parse_confile(g_main_ctx.confile);
//
//    // test
//    if (get_arg("t")) {
//        printf("Test confile [%s] OK!\n", g_main_ctx.confile);
//        exit(0);
//    }

    // signal
//    signal_init(on_reload);
//    const char* signal = get_arg("s");
//    if (signal) {
//        signal_handle(signal);
//    }

#ifdef OS_UNIX
    // daemon
    if (get_arg("d")) {
        // nochdir, noclose
        int ret = daemon(1, 1);
        if (ret != 0) {
            printf("daemon error: %d\n", ret);
            exit(-10);
        }
    }
#endif

    // pidfile
//    create_pidfile();

    // http_server
    Router::Register(g_http_service);
    g_http_server.registerHttpService(&g_http_service);
    g_http_server.run();
    return ret;
}

int main(int argc, char** argv)
{
//    FILE* file = fopen("debugger.zip","rb");
//    // 检查文件是否成功打开
//    if (file == NULL) {
//        perror("Could not open the file");
//        return EXIT_FAILURE;
//    }
//    const int BUFFER_SIZE = 367001600;
//    char read_buffer[BUFFER_SIZE];
//    // 将文件内容读取到缓冲区中
//    size_t bytes_read = fread(read_buffer, sizeof(unsigned char), BUFFER_SIZE, file);
//    // 检查是否读取成功
//    if (ferror(file)) {
//        perror("Error reading from file");
//        fclose(file);
//        return EXIT_FAILURE;
//    }
//    BindRuntimeDebuggerWebData((const uint8_t *) read_buffer, bytes_read);


    int ret = CreateHttpService(2233,".");
    BindWebSocketService(NULL,NULL,NULL);
    RunHttpService(true);
    return  ret;
}

int CreateHttpService(int port,const char* document_root_dir)
{
    int worker_processes=0;
    int worker_threads=0;
    g_http_server.worker_processes = LIMIT(0, worker_processes, MAXNUM_WORKER_PROCESSES);
    g_http_server.worker_threads = LIMIT(0, worker_threads, 64);
    g_http_server.port = port;

    g_http_service.document_root = document_root_dir;
    Router::Register(g_http_service);
    g_http_server.registerHttpService(&g_http_service);
    return 0;
}

void BindHttpsService(int https_port,const char* ssl_certificate,const char* ssl_private_key,const char* ssl_ca_certificate)
{
    hv::HttpServer &server = g_http_server;
    // 配置和启动 HTTPS 服务
    server.https_port = https_port;

    hlogi("SSL backend is %s", hssl_backend());
    hssl_ctx_init_param_t param;
    memset(&param, 0, sizeof(param));
    param.crt_file = ssl_certificate;
    param.key_file = ssl_private_key;
    if (ssl_ca_certificate)
    {
        param.ca_file = ssl_ca_certificate;
    }
    param.endpoint = HSSL_SERVER;
    if (hssl_ctx_init(&param) == NULL) {
        hloge("SSL certificate verify failed!");
        exit(0);
    }
    else {
        hlogi("SSL certificate verify ok!");
    }
}

void RunHttpService(bool  wait)
{
   if(wait)
   {
       g_http_server.run();
   }
   else
   {
       g_http_server.start();
}
}

void StopHttpService()
{
    g_http_server.stop();
}

static OnWebSocketOpen on_open_;
static OnWebSocketMessage on_message_;
static OnWebSocketClose on_close_;

void BindWebSocketService(OnWebSocketOpen on_open,OnWebSocketMessage on_message,OnWebSocketClose on_close)
{
    on_open_ = on_open;
    on_message_ =  on_message;
    on_close_ = on_close;

    static WebSocketService ws;
    ws.onopen = [&](const WebSocketChannelPtr& channel, const std::string& url) {
        printf("onopen: GET %s\n", url.c_str());
        if (on_open_)
        {
            on_open_(channel, url.c_str());
        }
        // send(time) every 1s
//         hv::setInterval(1000, [channel](hv::TimerID id) {
//             if (channel->isConnected()) {
//                 char str[DATETIME_FMT_BUFLEN] = {0};
//                 datetime_t dt = datetime_now();
//                 datetime_fmt(&dt, str);
//                 channel->send(str);
//             } else {
//                 hv::killTimer(id);
//             }
//         });
    };
    ws.onmessage = [&](const WebSocketChannelPtr& channel, const std::string& msg) {
        printf("onmessage: %d\n", (int)msg.size());
        const uint8_t* data = (const uint8_t*)msg.c_str();
        if(on_message_)
        {
            on_message_(channel, data, msg.size());
        }
    };
    ws.onclose = [&](const WebSocketChannelPtr& channel) {
        printf("onclose\n");
        if(on_close_)
        {
            on_close_(channel);
        }
    };

    g_http_server.ws = &ws;
}

void WebSocketSendBinary(const WebSocketChannelPtr& channel,const uint8_t* data,int size)
{
    if(channel)
    {
        if (channel->isConnected())
        {
            channel->send((const char*)data,size,WS_OPCODE_BINARY);
        } 
    }
}

void WebSocketSend(const WebSocketChannelPtr& channel,const char* message)
{
    if(channel)
    {
        if (channel->isConnected())
        {
            channel->send(message);
        }
    }
}

void WebSocketClose(const WebSocketChannelPtr& channel)
{
    if (channel)
    {
        channel->close();
    }
}

void BindRuntimeDebuggerWebData(const uint8_t* data,size_t size)
{
    RuntimeDebuggerWebData * web_data = &g_runtime_debugger_web_data;
    web_data->ExtractToMap((const char*) data,size);

    hv::HttpService& router = g_http_service;
    router.GET("/debugger*",[web_data,router](const HttpContextPtr& ctx){
        ctx->writer->Begin();

        std::string request_path = ctx->request->Path();
        if (request_path=="/debugger/")
        {
            request_path = "/debugger/index.html";
        }
        std::string file_name = request_path.substr(10);
        std::string file_data = web_data->GetFileData(file_name);
        int file_size = file_data.empty() ? 0: file_data.size();
        if(file_size>0)
        {
            http_content_type content_type = CONTENT_TYPE_NONE;
            const char* suffix = hv_suffixname(file_name.c_str());
            if (suffix) {
                content_type = http_content_type_enum_by_suffix(suffix);
            }
            if (content_type == CONTENT_TYPE_NONE || content_type == CONTENT_TYPE_UNDEFINED) {
                content_type = APPLICATION_OCTET_STREAM;
            }

            ctx->writer->WriteHeader("Content-Type", http_content_type_str(content_type));
            ctx->writer->WriteHeader("Content-Length", file_size);
            // ctx->writer->WriteHeader("Transfer-Encoding", "chunked");
            ctx->writer->EndHeaders();

            int nwrite = ctx->writer->WriteBody(file_data.data(), file_size);
            ctx->writer->End();
            return 200;
        }
        else
        {
            ctx->writer->WriteStatus(HTTP_STATUS_NOT_FOUND);
            ctx->writer->WriteHeader("Content-Type", "text/html");
            ctx->writer->WriteBody("<center><h1>404 Not Found</h1></center>");
            ctx->writer->End();
            return 404;
        }
    });
}

int GenerateSignedCertificate(int key_size,int exponent,const char *cert_filename, const char *key_filename)
{
    int ret;
    mbedtls_pk_context key;
    mbedtls_x509write_cert cert;
    mbedtls_entropy_context entropy;
    mbedtls_ctr_drbg_context ctr_drbg;
    char buf[4096];
    const char *pers = "cert_gen";

    mbedtls_entropy_init(&entropy);
    mbedtls_ctr_drbg_init(&ctr_drbg);
    mbedtls_pk_init(&key);
    mbedtls_x509write_crt_init(&cert);

    // Seed the random number generator
    ret = mbedtls_ctr_drbg_seed(&ctr_drbg, mbedtls_entropy_func, &entropy, (const unsigned char *)pers, strlen(pers));
    if (ret != 0) {
        printf("mbedtls_ctr_drbg_seed returned %d\n", ret);
    }

    if(ret == 0)
    {
        // Generate a keypair
        ret = mbedtls_pk_setup(&key, mbedtls_pk_info_from_type(MBEDTLS_PK_RSA));
        if (ret != 0)
        {
            printf("mbedtls_pk_setup returned %d\n", ret);
        }
    }

    if (ret == 0)
    {
        ret = mbedtls_rsa_gen_key(mbedtls_pk_rsa(key), mbedtls_ctr_drbg_random, &ctr_drbg, key_size, exponent);
        if (ret != 0)
        {
            printf("mbedtls_rsa_gen_key returned %d\n", ret);
        }
    }

    if (ret == 0)
    {
        // Write the key to a PEM string
        ret = mbedtls_pk_write_key_pem(&key, (unsigned char *) buf, sizeof(buf));
        if (ret != 0)
        {
            printf("mbedtls_pk_write_key_pem returned %d\n", ret);
        }
    }

    if (ret == 0)
    {
        // Save the key to a file
        FILE *f = fopen(key_filename, "w");
        if (f == NULL)
        {
            ret = 1;
        }
        fputs(buf, f);
        fclose(f);
    }

    if (ret == 0)
    {
        // Set up the certificate issuer and subject
        ret = mbedtls_x509write_crt_set_subject_name(&cert, "CN=Self-signed certificate,O=libhv,C=US");
        if (ret != 0)
        {
            printf("mbedtls_x509write_crt_set_subject_name returned %d\n", ret);
        }
    }

    if (ret == 0)
    {
        mbedtls_x509write_crt_set_issuer_name(&cert, "CN=Self-signed certificate,O=libhv,C=US");

        mbedtls_x509write_crt_set_version(&cert, MBEDTLS_X509_CRT_VERSION_3);
        mbedtls_x509write_crt_set_md_alg(&cert, MBEDTLS_MD_SHA256);

        // Set the validity period
        mbedtls_x509write_crt_set_validity(&cert, "20230101000000", "20331231235959");

        // Set the key
        mbedtls_x509write_crt_set_subject_key(&cert, &key);
        mbedtls_x509write_crt_set_issuer_key(&cert, &key);

        // Write the certificate to a PEM string
        ret = mbedtls_x509write_crt_pem(&cert, (unsigned char *) buf, sizeof(buf), mbedtls_ctr_drbg_random, &ctr_drbg);
        if (ret != 0)
        {
            printf("mbedtls_x509write_crt_pem returned %d\n", ret);
        }
    }

    if (ret == 0)
    {
        // Save the certificate to a file
        FILE* f = fopen(cert_filename, "w");
        if (f == NULL)
        {
            ret = 1;
        }
        fputs(buf, f);
        fclose(f);
    }

    printf("Self-signed certificate and private key generated successfully!\n");
//    ret = 0;

    mbedtls_x509write_crt_free(&cert);
    mbedtls_pk_free(&key);
    mbedtls_ctr_drbg_free(&ctr_drbg);
    mbedtls_entropy_free(&entropy);

    return ret;
}