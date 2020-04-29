using System;
using System.Collections.Generic;
using System.Text;

namespace Jory.NetCore.Model.Models
{
    public class ResultModel
    {
        public bool Success { get; set; } = false;
        public int Code { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

        public static ResultModel GetSuccess(string msg, object data = null)
        {
            return new ResultModel() { Success = true, Code = 200, Message = msg, Data = data };
        }

        public static ResultModel GetFail(string msg, object data = null)
        {
            return new ResultModel() { Success = false, Code = -1, Message = msg, Data = data };
        }
    }

    public class ResultModel<T>
    {
        public bool Success { get; set; } = false;
        public int Code { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public static ResultModel GetSuccess(string msg, T data = default)
        {
            return new ResultModel() { Success = true, Code = 200, Message = msg, Data = data };
        }

        public static ResultModel GetFail(string msg, T data = default)
        {
            return new ResultModel() { Success = false, Code = -1, Message = msg, Data = data };
        }
    }
}
