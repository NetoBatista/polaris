using System.Net;

namespace Polaris.Domain.Model
{
    public class ResponseBaseModel
    {
        public int StatusCode { get; set; }
        public object? Value { get; set; }

        public static ResponseBaseModel Ok(object? Value = null)
        {
            return new ResponseBaseModel
            {
                StatusCode = (int)HttpStatusCode.OK,
                Value = Value
            };
        }

        public static ResponseBaseModel BadRequest(object? Value = null)
        {
            return new ResponseBaseModel
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Value = Value
            };
        }
    }
}
