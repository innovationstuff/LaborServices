using System.Net;
using System.Net.Http;

namespace LaborServices.Web.Models
{
    public class APIResponseModel<T>
    {
        public T Result { get; set; }
        public  HttpStatusCode StatusCode { get; set; }
        public  string StatusMessage { get; set; }
    }
}