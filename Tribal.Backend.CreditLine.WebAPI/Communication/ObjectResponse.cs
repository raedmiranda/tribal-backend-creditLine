namespace Tribal.Backend.CreditLine.WebAPI.Communication
{
    public class ObjectResponse<T>
    {
        public T DataResponse { get; set; }

        public object DataRequest { get; set; }

        public StatusResponse StatusResponse { get; set; }
    }
}
