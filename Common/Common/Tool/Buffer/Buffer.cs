using System;

namespace Start
{
    /// <summary>
    /// 动态字节缓冲器
    /// </summary>
    public class Buffer
    {
        /// <summary>
        /// 缓冲区是否为空?
        /// </summary>
        public bool IsEmpty => _data == null || _size == 0;

        /// <summary>
        /// 字节存储器缓冲器（具体数据）
        /// </summary>
        public byte[] Data => _data;

        /// <summary>
        /// 字节存储器缓冲容量（容量大小）
        /// </summary>
        public long Capacity => _data.Length;

        /// <summary>
        /// 内存缓冲区大小（已存数据大小）
        /// </summary>
        public long Size => _size;

        /// <summary>
        /// 字节存储器缓冲偏移量
        /// </summary>
        public long Offset => _offset;

        public byte this[long index]
        {
            get
            {
                if (index < 0 || index >= _size)
                    throw new IndexOutOfRangeException($"Index {index} is out of range [0, {_size}).");
                return _data[index];
            }
        }

        private byte[] _data;
        private long _size;
        private long _offset;

        public Buffer()
        {
            _data = Array.Empty<byte>();
            _size = 0;
            _offset = 0;
        }

        public Buffer(long capacity)
        {
            _data = new byte[capacity];
            _size = 0;
            _offset = 0;
        }

        public Buffer(byte[] data)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
            _size = data.Length;
            _offset = 0;
        }

        /// <summary>
        /// 清空缓冲区
        /// </summary>
        /// <param name="clearData">是否清空数据（用于安全清理敏感数据）</param>
        public void Clear(bool clearData = false)
        {
            if (clearData && _data != null && _size > 0)
            {
                Array.Clear(_data, 0, (int)_size);
            }
            _size = 0;
            _offset = 0;
        }

        /// <summary>
        /// 移除给定偏移量和大小的缓冲区
        /// </summary>
        /// <param name="offset">偏移量</param>
        /// <param name="size">大小</param>
        /// <exception cref="ArgumentException"></exception>
        public void Remove(long offset, long size)
        {
            if (offset < 0)
                throw new ArgumentException("Offset cannot be negative.", nameof(offset));
            if (size < 0)
                throw new ArgumentException("Size cannot be negative.", nameof(size));
            if (offset + size > _size)
                throw new ArgumentException("Offset + size exceeds buffer size.", nameof(size));

            long copyLength = _size - offset - size;
            if (copyLength > 0)
            {
                Array.Copy(_data, offset + size, _data, offset, copyLength);
            }
            _size -= size;

            if (_offset >= offset + size)
            {
                _offset -= size;
            }
            else if (_offset > offset)
            {
                _offset = offset;
            }
        }

        /// <summary>
        /// 保留给定容量的缓冲区 (扩容)
        /// </summary>
        /// <param name="capacity"></param>
        /// <exception cref="ArgumentException"></exception>
        public void Reserve(long capacity)
        {
            if (capacity < 0)
                throw new ArgumentException("Invalid reserve capacity!", nameof(capacity));
            if (capacity > int.MaxValue)
                throw new ArgumentException($"Capacity cannot exceed {int.MaxValue}.", nameof(capacity));

            if (capacity > Capacity)
            {
                int newCapacity = (int)(Capacity == 0 ? capacity : Math.Max(capacity, Math.Min(2 * Capacity, int.MaxValue)));
                byte[] data = new byte[newCapacity];
                Array.Copy(_data, 0, data, 0, _size);
                _data = data;
            }
        }

        /// <summary>
        /// 调整当前缓冲区的大小
        /// </summary>
        /// <param name="size"></param>
        public void Resize(long size)
        {
            Reserve(size);
            _size = size;
            if (_offset > _size)
                _offset = _size;
        }

        /// <summary>
        /// 增加偏移
        /// </summary>
        /// <param name="offset">偏移</param>
        public void Shift(long offset)
        {
            if (offset < 0)
                throw new ArgumentException("Offset cannot be negative.", nameof(offset));
            _offset += offset;
            if (_offset > _size)
                _offset = _size;
        }

        /// <summary>
        /// 减少偏移
        /// </summary>
        /// <param name="offset">偏移</param>
        public void Unshift(long offset)
        {
            if (offset < 0)
                throw new ArgumentException("Offset cannot be negative.", nameof(offset));
            _offset -= offset;
            if (_offset < 0)
                _offset = 0;
        }

        /// <summary>
        /// 附加一个字节
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public long Append(byte value)
        {
            Reserve(_size + 1);
            _data[_size] = value;
            _size += 1;
            return 1;
        }

        /// <summary>
        /// 附加一个字节组
        /// </summary>
        /// <param name="buffer">字节组</param>
        /// <returns></returns>
        public long Append(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            Reserve(_size + buffer.Length);
            Array.Copy(buffer, 0, _data, _size, buffer.Length);
            _size += buffer.Length;
            return buffer.Length;
        }

        /// <summary>
        /// 附加一个字节组的一部分
        /// </summary>
        /// <param name="buffer">字节组</param>
        /// <param name="offset">偏移</param>
        /// <param name="size">大小</param>
        /// <returns></returns>
        public long Append(byte[] buffer, long offset, long size)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (offset < 0)
                throw new ArgumentException("Offset cannot be negative.", nameof(offset));
            if (size < 0)
                throw new ArgumentException("Size cannot be negative.", nameof(size));
            if (offset + size > buffer.Length)
                throw new ArgumentException("Offset + size exceeds buffer length.", nameof(size));

            Reserve(_size + size);
            Array.Copy(buffer, offset, _data, _size, size);
            _size += size;
            return size;
        }
    }
}