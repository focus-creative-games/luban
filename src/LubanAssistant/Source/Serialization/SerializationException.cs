#define CPU_SUPPORT_MEMORY_NOT_ALIGN  //CPU 是否支持读取非对齐内存

using System;


/// <summary>
/// TODO  
/// 1. 整理代码
/// 2. 优化序列化 (像这样 data[endPos + 1] = (byte)(x >> 8) 挨个字节赋值总感觉很低效，能优化吗)
/// </summary>


namespace Bright.Serialization
{
    public class SerializationException : Exception
    {
        public SerializationException() { }
        public SerializationException(string msg) : base(msg) { }

        public SerializationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
