﻿// -----------------------------------------------------------------------
//  <copyright file="ServiceLocator.cs" company="OSharp开源团队">
//      Copyright (c) 2014-2018 OSharp. All rights reserved.
//  </copyright>
//  <site>http://www.osharp.org</site>
//  <last-editor>郭明锋</last-editor>
//  <last-date>2018-03-09 21:57</last-date>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using OSharp.Data;
using OSharp.Exceptions;


namespace OSharp.Dependency
{
    /// <summary>
    /// 应用程序服务定位器，仅适合于<see cref="ServiceLifetime.Singleton"/>与<see cref="ServiceLifetime.Transient"/>生命周期类型的服务
    /// </summary>
    public sealed class ServiceLocator
    {
        private static readonly Lazy<ServiceLocator> InstanceLazy = new Lazy<ServiceLocator>(() => new ServiceLocator());
        private IServiceProvider _provider;

        private IServiceCollection _services;

        /// <summary>
        /// 初始化一个<see cref="ServiceLocator"/>类型的新实例
        /// </summary>
        private ServiceLocator()
        { }

        /// <summary>
        /// 获取 服务器定位器实例
        /// </summary>
        public static ServiceLocator Instance => InstanceLazy.Value;

        /// <summary>
        /// 获取 ServiceProvider是否为可用
        /// </summary>
        public bool IsProviderEnabled => _provider != null;

        /// <summary>
        /// 获取 <see cref="ServiceLifetime.Scoped"/>生命周期的服务提供者
        /// </summary>
        public IServiceProvider ScopedProvider
        {
            get
            {
                IScopedServiceResolver scopedResolver = _provider.GetService<IScopedServiceResolver>();
                return scopedResolver != null && scopedResolver.ResolveEnabled
                    ? scopedResolver.ScopedProvider
                    : null;
            }
        }

        /// <summary>
        /// 设置应用程序服务集合
        /// </summary>
        internal void TrySetServiceCollection(IServiceCollection services)
        {
            Check.NotNull(services, nameof(services));
            if (_services == null)
            {
                _services = services;
            }
        }

        /// <summary>
        /// 设置应用程序服务提供者
        /// </summary>
        public void TrySetApplicationServiceProvider(IServiceProvider provider)
        {
            Check.NotNull(provider, nameof(provider));
            if (_provider == null)
            {
                _provider = provider;
            }
        }

        /// <summary>
        /// 执行<see cref="ServiceLifetime.Scoped"/>生命周期的业务逻辑
        /// 1.当前处理<see cref="ServiceLifetime.Scoped"/>生命周期外，使用CreateScope创建<see cref="ServiceLifetime.Scoped"/>生命周期的ServiceProvider来执行，并释放资源
        /// 2.当前处于<see cref="ServiceLifetime.Scoped"/>生命周期内，直接使用<see cref="ServiceLifetime.Scoped"/>的ServiceProvider来执行
        /// </summary>
        public void ExcuteScopedWork(Action<IServiceProvider> action)
        {
            if (_provider == null)
            {
                throw new OsharpException("Root级别的IServiceProvider不存在，无法执行Scoped业务");
            }
            IServiceProvider scopedProvider = ScopedProvider;
            IServiceScope newScope = null;
            if (scopedProvider == null)
            {
                newScope = _provider.CreateScope();
                scopedProvider = newScope.ServiceProvider;
            }
            try
            {
                action(scopedProvider);
            }
            finally
            {
                newScope?.Dispose();
            }
        }

        /// <summary>
        /// 异步执行<see cref="ServiceLifetime.Scoped"/>生命周期的业务逻辑
        /// 1.当前处理<see cref="ServiceLifetime.Scoped"/>生命周期外，使用CreateScope创建<see cref="ServiceLifetime.Scoped"/>生命周期的ServiceProvider来执行，并释放资源
        /// 2.当前处于<see cref="ServiceLifetime.Scoped"/>生命周期内，直接使用<see cref="ServiceLifetime.Scoped"/>的ServiceProvider来执行
        /// </summary>
        public async Task ExcuteScopedWorkAsync(Func<IServiceProvider, Task> action)
        {
            if (_provider == null)
            {
                throw new OsharpException("Root级别的IServiceProvider不存在，无法执行Scoped业务");
            }
            IServiceProvider scopedProvider = ScopedProvider;
            IServiceScope newScope = null;
            if (scopedProvider == null)
            {
                newScope = _provider.CreateScope();
                scopedProvider = newScope.ServiceProvider;
            }
            try
            {
                await action(scopedProvider);
            }
            finally
            {
                newScope?.Dispose();
            }
        }

        /// <summary>
        /// 执行<see cref="ServiceLifetime.Scoped"/>生命周期的业务逻辑，并获取返回值
        /// 1.当前处理<see cref="ServiceLifetime.Scoped"/>生命周期外，使用CreateScope创建<see cref="ServiceLifetime.Scoped"/>生命周期的ServiceProvider来执行，并释放资源
        /// 2.当前处于<see cref="ServiceLifetime.Scoped"/>生命周期内，直接使用<see cref="ServiceLifetime.Scoped"/>的ServiceProvider来执行
        /// </summary>
        public TResult ExcuteScopedWork<TResult>(Func<IServiceProvider, TResult> func)
        {
            if (_provider == null)
            {
                throw new OsharpException("Root级别的IServiceProvider不存在，无法执行Scoped业务");
            }
            IServiceProvider scopedProvider = ScopedProvider;
            IServiceScope newScope = null;
            if (scopedProvider == null)
            {
                newScope = _provider.CreateScope();
                scopedProvider = newScope.ServiceProvider;
            }
            try
            {
                return func(scopedProvider);
            }
            finally
            {
                newScope?.Dispose();
            }
        }

        /// <summary>
        /// 执行<see cref="ServiceLifetime.Scoped"/>生命周期的业务逻辑，并获取返回值
        /// 1.当前处理<see cref="ServiceLifetime.Scoped"/>生命周期外，使用CreateScope创建<see cref="ServiceLifetime.Scoped"/>生命周期的ServiceProvider来执行，并释放资源
        /// 2.当前处于<see cref="ServiceLifetime.Scoped"/>生命周期内，直接使用<see cref="ServiceLifetime.Scoped"/>的ServiceProvider来执行
        /// </summary>
        public async Task<TResult> ExcuteScopedWorkAsync<TResult>(Func<IServiceProvider, Task<TResult>> func)
        {
            if (_provider == null)
            {
                throw new OsharpException("Root级别的IServiceProvider不存在，无法执行Scoped业务");
            }
            IServiceProvider scopedProvider = ScopedProvider;
            IServiceScope newScope = null;
            if (scopedProvider == null)
            {
                newScope = _provider.CreateScope();
                scopedProvider = newScope.ServiceProvider;
            }
            try
            {
                return await func(scopedProvider);
            }
            finally
            {
                newScope?.Dispose();
            }
        }

        /// <summary>
        /// 获取所有已注册的<see cref="ServiceDescriptor"/>对象
        /// </summary>
        public IEnumerable<ServiceDescriptor> GetServiceDescriptors()
        {
            Check.NotNull(_services, nameof(_services));
            return _services;
        }

        /// <summary>
        /// 解析指定类型的服务实例
        /// </summary>
        public T GetService<T>()
        {
            Check.NotNull(_services, nameof(_services));
            Check.NotNull(_provider, nameof(_provider));

            IScopedServiceResolver scopedResolver = _provider.GetService<IScopedServiceResolver>();
            if (scopedResolver != null && scopedResolver.ResolveEnabled)
            {
                return scopedResolver.GetService<T>();
            }
            return _provider.GetService<T>();
        }

        /// <summary>
        /// 解析指定类型的服务实例
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        public object GetService(Type serviceType)
        {
            Check.NotNull(_services, nameof(_services));
            Check.NotNull(_provider, nameof(_provider));

            IScopedServiceResolver scopedResolver = _provider.GetService<IScopedServiceResolver>();
            if (scopedResolver != null && scopedResolver.ResolveEnabled)
            {
                return scopedResolver.GetService(serviceType);
            }
            return _provider.GetService(serviceType);
        }

        /// <summary>
        /// 解析指定类型的所有服务实例
        /// </summary>
        public IEnumerable<T> GetServices<T>()
        {
            Check.NotNull(_services, nameof(_services));
            Check.NotNull(_provider, nameof(_provider));

            IScopedServiceResolver scopedResolver = _provider.GetService<IScopedServiceResolver>();
            if (scopedResolver != null && scopedResolver.ResolveEnabled)
            {
                return scopedResolver.GetServices<T>();
            }
            return _provider.GetServices<T>();
        }

        /// <summary>
        /// 解析指定类型的所有服务实例
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            Check.NotNull(_services, nameof(_services));
            Check.NotNull(_provider, nameof(_provider));

            IScopedServiceResolver scopedResolver = _provider.GetService<IScopedServiceResolver>();
            if (scopedResolver != null && scopedResolver.ResolveEnabled)
            {
                return scopedResolver.GetServices(serviceType);
            }
            return _provider.GetServices(serviceType);
        }
    }
}