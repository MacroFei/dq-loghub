using Aliyun.Api.LogService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dq_loghub.Client
{
    public class LogServiceClientFactory
    {
        private static string endpoint = "cn-hangzhou.log.aliyuncs.com";
        private static string projectName = "net-project-test";
        private static string accessKeyId = "****";
        private static string accessKeySecret = "****";

        //private static string endpoint = "<你的外网域名>";
        //private static string projectName = "<你的项目名称>";
        //private static string accessKeyId = "<访问阿里云API的密钥Id>";
        //private static string accessKeySecret = "<访问阿里云API的密钥Secret>";
        //private static string logstore = "<日志库名称>";


        public static ILogServiceClient BuildSimpleClient()
        {
            return LogServiceClientBuilders.HttpBuilder
                // 服务入口<endpoint>及项目名<projectName>。
                .Endpoint(endpoint, projectName)
                // 访问密钥信息。
                .Credential(accessKeyId, accessKeySecret)
                .Build();
        }
    }
}
