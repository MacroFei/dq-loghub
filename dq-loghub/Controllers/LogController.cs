using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aliyun.Api.LogService;
using Aliyun.Api.LogService.Domain.Log;
using dq_loghub.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Quartz;

namespace dq_loghub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        // GET: api/Log
        [HttpGet]
        public String Get()
        {
            // 1. 构建 `ILogServiceClient` 实例：
            var client = LogServiceClientFactory.BuildSimpleClient();

            // 2.查询相应日志库中的日志数据
            string logstore = "net-logstore";
            var asyncTask = client.GetLogsAsync
           (
               // 「必填参数」作为方法的普通必须参数。
               logstore,
               //查询的时间区间
               DateBuilder.DateOf(11, 45, 30, 1, 1, 2018),
               DateTimeOffset.Now,
               
               // 「可选参数」作为方法的可选参数，可通过命名参数方式指定。
               offset: 1,
               line: 100
           );
            // 在普通控制台的环境下同步等待结果直接调用即可。
            var response = asyncTask.Result;

            var result = response
                          // 此方法会确保返回的响应失败时候抛出`LogServiceException`。
                          .EnsureSuccess()
                          // 此处获取Result是安全的。
                          .Result;
            //3.直接输出Log信息
            Console.WriteLine($"RequestId：{response.RequestId}");
            Console.WriteLine($"日志总数：{result.Count}");
            Console.WriteLine($"首条日志：{result.Logs.FirstOrDefault()}");

            //直接返回String数据 方便测试
            return $"RequestId：{response.RequestId}" + $"日志总数：{result.Count}" + $"首条日志：{result.Logs.FirstOrDefault()}";

        }



        // POST: api/Log
        [HttpPost]
        public string Post()
        {
            // 1. 构建 `ILogServiceClient` 实例：
            var client = LogServiceClientFactory.BuildSimpleClient();

            // 2.模拟Log日志
            // 注意!!此处时间为LogHub查询时对应的时间
            var rawLogs = new[]
            {
                    "2020-02-17 12:34:56 INFO id=1 status=foo",
                    "2020-02-18 14:00:00 INFO id=2 status=bar",
                    "2020-02-16 12:34:58 INFO id=1 status=foo",
                    "2020-02-17 23:59:00 WARN id=1 status=foo",
                    "2020-02-04 23:59:00 WARN id=1 status=foo"
                };

            // 3.将原始日志转换为 LogInfo 
            var parsedLogs = rawLogs
                .Select(x =>
                {
                    var components = x.Split(' ');

                    var date = components[0];
                    var time = components[1];
                    var level = components[2];
                    var id = components[3].Split('=');
                    var status = components[4].Split('=');

                    var logInfo = new LogInfo
                    {
                        Contents =
                        {
                                {"level", level},
                                {id[0], id[1]},
                                {status[0], status[1]},
                        },
                        Time = DateTimeOffset.ParseExact($"{date} {time}", "yyyy-MM-dd HH:mm:ss", null)
                    };

                    return logInfo;
                })
                .ToList();
            var logGroupInfo = new LogGroupInfo
            {
                Topic = "topic_test",
                LogTags =
                    {
                        {"example", "true"},
                    },
                Logs = parsedLogs,
                Source = "dq"
            };

            // 4.将日志发送给对应的日志库
            string logstore = "net-logstore";
            var response = client.PostLogStoreLogsAsync(new PostLogsRequest(logstore, logGroupInfo));

            // 该方法无返回值必要，success仅为方便测试
            return "success";
        }


    }
}
