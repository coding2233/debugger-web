#ifndef HV_HTTPD_ROUTER_H
#define HV_HTTPD_ROUTER_H

#include "hv/HttpService.h"

class Router {
public:
    static void Register(hv::HttpService& router);
};

#endif // HV_HTTPD_ROUTER_H
