﻿using System.Collections.Generic;
using AutumnBox.OpenFramework.Content;
using AutumnBox.OpenFramework.Extension;
using AutumnBox.OpenFramework.LeafExtension.Attributes;
using AutumnBox.OpenFramework.LeafExtension.Internal;
using AutumnBox.OpenFramework.Wrapper;

namespace AutumnBox.OpenFramework.LeafExtension
{
    /// <summary>
    /// Leaf模块
    /// </summary>
    public abstract class LeafExtensionBase : EmptyExtension, IClassExtension
    {
        /// <summary>
        /// 上下文
        /// </summary>
        internal Context Context { get; set; }

        private readonly LSignalDistributor signalDistributor;
        private readonly LeafEntryExecutor executor;
        private readonly LeafPropertyInjector injector;

        /// <summary>
        /// 构造函数
        /// </summary>
        public LeafExtensionBase()
        {

            //初始化上下文
            Context = new LeafContext();

            //初始化API注入器
            ApiAllocator apiAllocator = new ApiAllocator(Context, GetType());

            //执行切面
            ExecuteBca();

            //注入属性
            injector = new LeafPropertyInjector(this, apiAllocator);
            injector.Inject();

            //构造入口点执行器
            executor = new LeafEntryExecutor(this, apiAllocator);

            //注册信号接收系统
            signalDistributor = new LSignalDistributor(this);
            signalDistributor.ScanReceiver();
        }

        /// <summary>
        /// 执行切面函数
        /// </summary>
        private void ExecuteBca()
        {
            var scanner = new ClassExtensionScanner(GetType());
            scanner.Scan(ClassExtensionScanner.ScanOption.BeforeCreatingAspect);
            var bcAspects = scanner.BeforeCreatingAspects;
            bool canContinue = true;
            BeforeCreatingAspectArgs bcaArgs = new BeforeCreatingAspectArgs(Context, GetType());
            foreach (var aspect in bcAspects)
            {
                aspect.BeforeCreating(bcaArgs, ref canContinue);
                if (!canContinue)
                {
                    throw new ClassExtensionBase.AspectPreventedException();
                }
            }
        }

        /// <summary>
        /// 入口函数,继承者无需关心
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [LDoNotScan]
        public int Main(Dictionary<string, object> data)
        {
            return executor.Execute(data) ?? 0;
        }

        /// <summary>
        /// 接收信号
        /// </summary>
        /// <param name="signalName"></param>
        /// <param name="value"></param>
        public void ReceiveSignal(string signalName, object value = null)
        {
            signalDistributor.Receive(signalName, value);
        }
    }
}
