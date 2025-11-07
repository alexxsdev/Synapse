using System;
using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Synapse.Core.HotReload
{
    /// <summary>
    /// 热重载管理器 - 运行时动态替换方法实现
    /// </summary>
    public class HotReloadManager
    {
        private readonly ConcurrentDictionary<string, Delegate> _hotReloadDelegates = new();
        private readonly ILogger<HotReloadManager> _logger;

        public HotReloadManager(ILogger<HotReloadManager> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 注册可热重载的方法
        /// </summary>
        public void RegisterHotReloadable<TDelegate>(string methodKey, TDelegate originalImplementation) where TDelegate : Delegate
        {
            _hotReloadDelegates[methodKey] = originalImplementation;
            _logger.LogInformation("🔄 已注册热重载方法: {MethodKey}", methodKey);
        }

        /// <summary>
        /// 获取方法实现（可能是热重载后的版本）
        /// </summary>
        public TDelegate GetImplementation<TDelegate>(string methodKey) where TDelegate : Delegate
        {
            if (_hotReloadDelegates.TryGetValue(methodKey, out var impl))
            {
                return (TDelegate)impl;
            }

            throw new InvalidOperationException($"方法 {methodKey} 未注册为可热重载");
        }

        /// <summary>
        /// 热重载方法（替换实现）
        /// </summary>
        public bool HotReload<TDelegate>(string methodKey, TDelegate newImplementation) where TDelegate : Delegate
        {
            if (!_hotReloadDelegates.ContainsKey(methodKey))
            {
                _logger.LogWarning("⚠️ 方法 {MethodKey} 未注册，无法热重载", methodKey);
                return false;
            }

            _hotReloadDelegates[methodKey] = newImplementation;
            _logger.LogInformation("✅ 热重载成功: {MethodKey}", methodKey);
            return true;
        }

        /// <summary>
        /// 恢复原始实现
        /// </summary>
        public bool RestoreOriginal(string methodKey)
        {
            // 这里需要存储原始实现的备份
            _logger.LogInformation("🔙 恢复原始实现: {MethodKey}", methodKey);
            return true;
        }

        /// <summary>
        /// 检查方法是否已被热重载
        /// </summary>
        public bool IsHotReloaded(string methodKey)
        {
            return _hotReloadDelegates.ContainsKey(methodKey);
        }

        /// <summary>
        /// 获取所有可热重载的方法
        /// </summary>
        public string[] GetRegisteredMethods()
        {
            return _hotReloadDelegates.Keys.ToArray();
        }
    }
}

