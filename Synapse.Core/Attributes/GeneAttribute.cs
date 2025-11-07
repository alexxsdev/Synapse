using System;

namespace Synapse.Core.Attributes
{
    /// <summary>
    /// 标记方法为一个基因实现
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class GeneAttribute : Attribute
    {
        /// <summary>
        /// 基因唯一标识符
        /// </summary>
        public string Id { get; }
        
        /// <summary>
        /// 基因描述
        /// </summary>
        public string? Description { get; set; }
        
        /// <summary>
        /// 优先级（数字越小优先级越高）
        /// </summary>
        public int Priority { get; set; } = 100;
        
        /// <summary>
        /// 是否为默认基因
        /// </summary>
        public bool IsDefault { get; set; } = false;
        
        public GeneAttribute(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("基因 ID 不能为空", nameof(id));
            
            Id = id;
        }
    }
}
