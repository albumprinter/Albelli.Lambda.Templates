﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Albelli.Lambda.Templates.Core;
using Albelli.Lambda.Templates.Core.Executors;
using Albelli.Lambda.Templates.Core.Pipelines;
using Albelli.Lambda.Templates.Core.Routing;
using Amazon.Lambda.AspNetCoreServer;
using Amazon.Lambda.AspNetCoreServer.Internal;
using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Albelli.Lambda.Templates.Sns
{
    public abstract class SnsProxyFunction<TEntity, TStartup> : PipelinedAspNetCoreFunction<SNSEvent.SNSRecord, StatusProxyResponse>
        where TStartup : class
    {
        private Action<IServiceCollection> _messageRouterConfigurator = s => s.AddSingleton<IMessageRouter, BodyTypeMessageRouter>();
        private IServiceProvider _serviceProvider;

        protected SnsProxyFunction()
            : base()
        {

        }

        protected SnsProxyFunction(StartupMode startupMode)
            : base(startupMode)
        {

        }

        protected void ConfigureMessageRouter<TMessageRouter>()
        where TMessageRouter : class, IMessageRouter
        {
            _messageRouterConfigurator = s => s.AddSingleton<IMessageRouter, TMessageRouter>();
        }

        protected override void Init(IWebHostBuilder builder) => builder
            .ConfigureServices(_messageRouterConfigurator)
            .UseStartup<TStartup>();

        protected override void PostCreateHost(IHost webHost)
        {
            base.PostCreateHost(webHost);

            _serviceProvider = webHost.Services;
        }

        protected override void MarshallRequest(InvokeFeatures features, SNSEvent.SNSRecord snsRecord, ILambdaContext lambdaContext)
        {
            var requestFeatures = (IHttpRequestFeature)features;
            requestFeatures.Scheme = "https";
            requestFeatures.Method = "POST";

            var messageRouter = _serviceProvider.GetService<IMessageRouter>();
            var path = $"/{messageRouter.GetPath<TEntity>().Trim('/')}/";

            requestFeatures.Path = path;
            requestFeatures.PathBase = string.Empty;

            requestFeatures.Headers["Content-Type"] = "application/json; charset=utf-8";

            requestFeatures.Body = new MemoryStream(Encoding.UTF8.GetBytes(snsRecord.Sns.Message));

            const string contentLengthHeaderName = "Content-Length";
            if (!requestFeatures.Headers.ContainsKey(contentLengthHeaderName))
            {
                requestFeatures.Headers[contentLengthHeaderName] = requestFeatures.Body.Length.ToString(CultureInfo.InvariantCulture);
            }

            PostMarshallRequestFeature(requestFeatures, snsRecord, lambdaContext);
        }

        protected override StatusProxyResponse MarshallResponse(IHttpResponseFeature responseFeatures, ILambdaContext lambdaContext, int statusCodeIfNotSet = 200)
        {
            var response = new StatusProxyResponse
            {
                StatusCode = (HttpStatusCode)(responseFeatures.StatusCode != 0 ? responseFeatures.StatusCode : statusCodeIfNotSet)
            };

            PostMarshallResponseFeature(responseFeatures, response, lambdaContext);
            return response;
        }

        [UsedImplicitly]
        public async Task FunctionHandlerAsync(SNSEvent snsEvent, ILambdaContext lambdaContext)
        {
            await SnsEventExecutor.Execute(snsEvent.Records, record => ExecuteRecord(record, lambdaContext));
        }

        private async Task ExecuteRecord(SNSEvent.SNSRecord snsRecord, ILambdaContext lambdaContext)
        {
            SnsRecordPipelineHandlers.Foreach(handler => handler.HookBefore(snsRecord, lambdaContext));
            var response = await base.FunctionHandlerAsync(snsRecord, lambdaContext);
            SnsRecordPipelineHandlers.ForeachReverse(handler => handler.HookAfter(snsRecord, lambdaContext));

            lambdaContext.Logger.Log($"SNS record {snsRecord.Sns.MessageId} handled with status: {response.StatusCode}");
        }

        public List<ISnsRecordPipelineHandler> SnsRecordPipelineHandlers { get; } = new List<ISnsRecordPipelineHandler>();
        public ICollectionExecutor SnsEventExecutor { get; protected set; } = new SequentialCollectionExecutor();

        public void ChooseSequentialExecutionMode()
        {
            SnsEventExecutor = new SequentialCollectionExecutor();
        }

        public void ChooseConcurrentExecutionMode(int batchSize)
        {
            SnsEventExecutor = new ConcurrentCollectionExecutor(batchSize);
        }
    }
}