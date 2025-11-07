# 常见问题 (FAQ)

## 🤔 一般问题

### Synapse 是什么？

Synapse 是一个 AI 驱动的自进化代码框架，它能让你的 .NET 应用在运行时自动监控性能、分析瓶颈并生成优化建议。

### Synapse 适合我吗？

如果你：
- ✅ 关注应用性能
- ✅ 想要自动化性能优化
- ✅ 需要 A/B 测试不同实现
- ✅ 希望 AI 辅助代码优化

那么 Synapse 很适合你！

### Synapse 是免费的吗？

是的！Synapse 框架本身是 MIT 开源协议，完全免费。但你需要：
- Gemini API Key（有免费额度）
- 或其他 AI 提供商的 API（未来支持）

### Synapse 会自动修改我的代码吗？

**不会！** Synapse 只会：
1. 监控性能
2. 生成优化建议
3. 保存为 JSON 报告

你需要**手动审查和应用**这些建议。

---

## 🚀 使用问题

### 如何开始使用？

查看 [快速开始指南](QUICKSTART.md)，只需 3 步：
1. 安装包
2. 配置服务
3. 添加 `[Evolvable]` 特性

### 需要什么环境？

- .NET 8.0 或更高版本
- Gemini API Key
- 任何支持 .NET 的操作系统

### 如何获取 Gemini API Key？

1. 访问 [Google AI Studio](https://makersuite.google.com/app/apikey)
2. 登录 Google 账号
3. 创建 API Key
4. 复制到配置文件

### 性能监控有开销吗？

非常小！使用 `using` 模式的 `PerformanceTracker`，开销 < 1ms。

### AI 生成的代码安全吗？

**需要审查！** 我们建议：
- ✅ 始终审查 AI 生成的代码
- ✅ 在测试环境验证
- ✅ 使用代码审查流程
- ❌ 不要盲目应用到生产环境

---

## ⚙️ 配置问题

### 如何配置性能阈值？

```json
{
  "Synapse": {
    "PerformanceThreshold": 50.0,  // 50ms
    "MinSampleSize": 100           // 100 次执行
  }
}
```

### 如何禁用 AI 功能？

```json
{
  "Synapse": {
    "EnableAI": false
  }
}
```

或者不调用 `AddSynapseAI()`。

### 如何更改优化频率？

```json
{
  "Synapse": {
    "AI": {
      "OptimizationIntervalHours": 24  // 24 小时
    }
  }
}
```

### 如何使用环境变量？

```bash
export Synapse__AI__ApiKey="your-key"
export Synapse__EnableAI="true"
export Synapse__PerformanceThreshold="30.0"
```

---

## 🔧 技术问题

### 支持哪些 .NET 版本？

- ✅ .NET 8.0
- ✅ .NET 9.0（未来）
- ❌ .NET Framework（不支持）

### 支持哪些 AI 提供商？

当前：
- ✅ Gemini AI

计划中：
- 🔜 OpenAI
- 🔜 Claude
- 🔜 Azure OpenAI

### 可以在生产环境使用吗？

可以，但建议：
1. 先在测试环境充分验证
2. 禁用自动切换（`AutoSwitch: false`）
3. 手动审查所有优化
4. 逐步推广

### 如何回滚优化？

```json
{
  "Synapse": {
    "Genes": {
      "MethodName": "DEFAULT"  // 切换回默认实现
    }
  }
}
```

### 支持分布式场景吗？

当前版本主要针对单机场景。分布式支持在路线图中。

---

## 🐛 故障排查

### AI 不生成优化建议？

检查：
1. ✅ API Key 是否正确
2. ✅ 执行次数是否达到阈值
3. ✅ 性能是否低于阈值
4. ✅ 是否在优化间隔内（24小时）
5. ✅ 日志中是否有错误

### 编译失败？

查看 `.synapse/suggestions/*.json` 中的错误信息。常见原因：
- 缺少 using 语句
- 类型不匹配
- 依赖缺失

### 性能没有提升？

可能原因：
- 样本数不足
- 阈值设置不合理
- 瓶颈不在代码本身（如数据库、网络）
- AI 建议未被应用

### 日志太多？

调整日志级别：
```json
{
  "Logging": {
    "LogLevel": {
      "Synapse": "Warning"  // 或 "Error"
    }
  }
}
```

---

## 📊 性能问题

### 监控开销有多大？

- 内存：< 10MB
- CPU：< 1%
- 延迟：< 1ms per call

### AI 生成需要多久？

- 通常 5-10 秒
- 取决于网络和 API 响应时间

### 可以监控多少个方法？

理论上无限制，但建议：
- 关注核心热点方法
- 避免监控所有方法

---

## 🤝 贡献问题

### 如何贡献代码？

查看 [贡献指南](CONTRIBUTING.md)。

### 如何报告 Bug？

使用 [Bug 报告模板](.github/ISSUE_TEMPLATE/bug_report.md)。

### 如何提出新功能？

使用 [功能请求模板](.github/ISSUE_TEMPLATE/feature_request.md)。

---

## 📚 更多资源

- [完整文档](README.md)
- [快速开始](QUICKSTART.md)
- [示例项目](Examples/WebApi.Example)
- [GitHub Discussions](https://github.com/your-repo/synapse/discussions)

---

**还有问题？** 在 [Discussions](https://github.com/your-repo/synapse/discussions) 提问！
