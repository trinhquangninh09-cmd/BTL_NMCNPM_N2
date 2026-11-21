using System;

namespace TourManager.BLL
{
    /// <summary>Lỗi nghiệp vụ có thông báo hiển thị trực tiếp cho người dùng.</summary>
    public class BusinessException : Exception
    {
        public BusinessException(string message) : base(message) { }
    }
}
