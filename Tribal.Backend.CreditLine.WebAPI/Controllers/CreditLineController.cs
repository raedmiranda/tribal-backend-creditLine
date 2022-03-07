using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using Tribal.Backend.CreditLine.Application;
using Tribal.Backend.CreditLine.Domain;
using Tribal.Backend.CreditLine.Domain.Models;
using Tribal.Backend.CreditLine.WebAPI.Communication;

namespace Tribal.Backend.CreditLine.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreditLineController : ControllerBase
    {
        CreditLineRequestService _creditLineService;
        public CreditLineController(CreditLineRequestService creditLineService)
        {
            _creditLineService = creditLineService;
        }

        public ActionResult<ObjectResponse<CreditLineResponseModel>> DetermineCreditLine(CreditLineRequestModel creditLineRequest)
        {
            ActionResult<ObjectResponse<CreditLineResponseModel>> result = null;
            ObjectResponse<CreditLineResponseModel> response = new ObjectResponse<CreditLineResponseModel>();
            CreditLineResponseModel serviceResponse = null;
            try
            {
                response.DataRequest = creditLineRequest;
                response.StatusResponse = new StatusResponse();
                response.StatusResponse.Status = "OK";

                if (creditLineRequest == null)
                {
                    response.DataResponse = null;
                    response.StatusResponse = new StatusResponse()
                    {
                        Message = "Request parameter cannot be null",
                        Status = "KO"
                    };
                    result = BadRequest(response);
                }
                else
                {
                    serviceResponse = _creditLineService.DetermineCreditLine(creditLineRequest);
                    if (serviceResponse != null)
                    {
                        response.DataResponse = serviceResponse;
                        response.StatusResponse = new StatusResponse()
                        {
                            Message = "Information credit line performed!",
                            Status = "OK"
                        };
                    }
                    else
                    {
                        response.StatusResponse = new StatusResponse()
                        {
                            Message = "Credit line information can't be done!",
                            Status = "KO"
                        };
                    }

                    if (response.StatusResponse.Status == "OK")
                    {
                        result = Ok(response);
                    }
                    else
                    {
                        result = BadRequest(response);
                    }
                }
            }
            catch (Exception ex)
            {
                response.DataResponse = null;
                response.StatusResponse = new StatusResponse()
                {
                    Message = ex.Message,
                    Status = "KO"
                };

                if (Response != null) Response.StatusCode = StatusCodes.Status500InternalServerError;
                result = StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            return result;
        }
    }
}
