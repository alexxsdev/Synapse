# 贡献指南

感谢你考虑为 Synapse 做出贡献！

## 🤝 如何贡献

### 报告 Bug

如果你发现了 bug，请：

1. 检查 [Issues](https://github.com/your-repo/synapse/issues) 确认问题未被报告
2. 创建新 Issue，包含：
   - 清晰的标题和描述
   - 重现步骤
   - 预期行为 vs 实际行为
   - 环境信息（.NET 版本、OS 等）
   - 相关日志或截图

### 提出新功能

1. 先在 [Discussions](https://github.com/your-repo/synapse/discussions) 讨论
2. 获得反馈后创建 Feature Request Issue
3. 描述功能的用途和价值

### 提交代码

#### 开发环境

- .NET 8.0 SDK
- Visual Studio 2022 或 VS Code
- Git

#### 步骤

1. **Fork 仓库**
   ```bash
   git clone https://github.com/your-username/synapse.git
   cd synapse
   ```

2. **创建分支**
   ```bash
   git checkout -b feature/your-feature-name
   ```

3. **开发**
   - 遵循现有代码风格
   - 添加 XML 文档注释
   - 编写单元测试
   - 确保编译通过

4. **测试**
   ```bash
   dotnet build
   dotnet test
   ```

5. **提交**
   ```bash
   git add .
   git commit -m "feat: add amazing feature"
   ```

6. **推送**
   ```bash
   git push origin feature/your-feature-name
   ```

7. **创建 Pull Request**
   - 清晰描述改动
   - 关联相关 Issue
   - 等待 Code Review

## 📝 代码规范

### C# 代码风格

- 使用 4 空格缩进
- 遵循 [C# 编码约定](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- 公共 API 必须有 XML 文档注释
- 使用有意义的变量名

### Commit 消息规范

遵循 [Conventional Commits](https://www.conventionalcommits.org/)：

```
<type>(<scope>): <subject>

<body>

<footer>
```

**类型：**
- `feat`: 新功能
- `fix`: Bug 修复
- `docs`: 文档更新
- `style`: 代码格式（不影响功能）
- `refactor`: 重构
- `test`: 测试相关
- `chore`: 构建/工具相关

**示例：**
```
feat(ai): add OpenAI provider support

- Implement OpenAI code generator
- Add configuration options
- Update documentation

Closes #123
```

## 🧪 测试

- 所有新功能必须有单元测试
- 测试覆盖率应 > 80%
- 运行测试：`dotnet test`

## 📚 文档

- 更新相关 README
- 添加代码示例
- 更新 CHANGELOG

## ✅ Pull Request 检查清单

- [ ] 代码编译通过
- [ ] 所有测试通过
- [ ] 添加了必要的测试
- [ ] 更新了文档
- [ ] 遵循代码规范
- [ ] Commit 消息符合规范
- [ ] 没有合并冲突

## 🎯 优先级

我们特别欢迎以下贡献：

- 🐛 Bug 修复
- 📝 文档改进
- 🧪 测试覆盖
- 🌐 多语言支持
- 🎨 示例项目
- 🔌 新的 AI 提供商

## 💬 获取帮助

- [GitHub Discussions](https://github.com/your-repo/synapse/discussions)
- [Discord](https://discord.gg/your-server)
- Email: your-email@example.com

## 📜 行为准则

请遵守我们的 [行为准则](CODE_OF_CONDUCT.md)。

---

再次感谢你的贡献！🎉
