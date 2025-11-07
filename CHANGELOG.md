# 更新日志

所有重要的项目更改都将记录在此文件中。

格式基于 [Keep a Changelog](https://keepachangelog.com/zh-CN/1.0.0/)，
并且本项目遵循 [语义化版本](https://semver.org/lang/zh-CN/)。

## [未发布]

### 计划中
- OpenAI 提供商支持
- Claude 提供商支持
- 可视化监控面板
- 自动化测试生成
- 分布式场景支持

## [1.0.0] - 2024-11-07

### 新增
- 🎉 首次发布
- ✨ 核心框架（Synapse.Core）
  - `[Evolvable]` 和 `[Gene]` 特性系统
  - 实时性能监控（P50/P95/P99）
  - 动态基因加载和缓存
  - 进化引擎和后台服务
- 🤖 AI 集成（Synapse.AI）
  - Gemini AI 代码生成器
  - Roslyn 动态编译器
  - 源代码提取和分析
  - 结构化优化报告（JSON）
- 🔄 热加载支持
  - 无需重启即可加载新代码
  - 多版本并行运行
- 💾 持久化缓存
  - 基因保存到磁盘
  - 重启后自动恢复
- 📊 优化频率控制
  - 防止重复优化（24小时间隔）
  - 可配置优化阈值
- 📝 完整文档
  - README.md
  - QUICKSTART.md
  - 示例项目（WebApi.Example）
- 🛠️ 工具脚本
  - build.ps1 - 构建脚本
  - test.ps1 - 测试脚本

### 技术栈
- .NET 8.0
- Gemini AI API
- Roslyn 编译器
- ASP.NET Core

### 已知问题
- XML 文档注释不完整（不影响功能）
- 仅支持 Gemini AI（其他提供商开发中）

## [0.1.0] - 2024-11-01

### 新增
- 🚧 初始原型
- 基础性能监控
- 简单的基因切换

---

## 版本说明

### 版本号格式
- **主版本号**：不兼容的 API 更改
- **次版本号**：向后兼容的功能新增
- **修订号**：向后兼容的问题修正

### 标签说明
- `新增` - 新功能
- `变更` - 现有功能的变更
- `弃用` - 即将移除的功能
- `移除` - 已移除的功能
- `修复` - Bug 修复
- `安全` - 安全相关的修复

---

[未发布]: https://github.com/your-repo/synapse/compare/v1.0.0...HEAD
[1.0.0]: https://github.com/your-repo/synapse/releases/tag/v1.0.0
[0.1.0]: https://github.com/your-repo/synapse/releases/tag/v0.1.0
