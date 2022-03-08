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
        UserLogService _userLogService;

        public CreditLineController(CreditLineRequestService creditLineService, UserLogService userLogService)
        {
            _creditLineService = creditLineService;
            _userLogService = userLogService;
        }

        public ActionResult<ObjectResponse<CreditLineResponseModel>> DetermineCreditLine(CreditLineRequestModel creditLineRequest, [FromHeader(Name = "credential")] string credential)
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
                    CreditLineResponseModel actualCreditLine = _creditLineService.ObtainConsumerCreditLine(credential);
                    bool lastAttemptWasSuccessful = actualCreditLine != null && actualCreditLine.IsAccepted;
                    bool areTooManyRequest = _userLogService.CheckAttemptCounter(credential, creditLineRequest.RequestedDate, lastAttemptWasSuccessful);

                    if (lastAttemptWasSuccessful && areTooManyRequest)
                    {
                        serviceResponse = null;
                        // response null - result 429
                    }
                    else if (lastAttemptWasSuccessful && !areTooManyRequest)
                    {
                        serviceResponse = actualCreditLine;
                        // response actual - result OK
                    }
                    else if (!lastAttemptWasSuccessful && areTooManyRequest)
                    {
                        serviceResponse = null;
                        // response null - result 429
                    }
                    else if (!lastAttemptWasSuccessful && !areTooManyRequest)
                    {
                        bool checkRejectedAttempts = _creditLineService.CheckRejectedApplications(credential);

                        if (checkRejectedAttempts)
                            throw new UnauthorizedAccessException();
                        // response null - result "Agent"
                        else
                        {
                            serviceResponse = _creditLineService.DetermineCreditLine(creditLineRequest);
                            _creditLineService.SaveConsumerCreditLine(serviceResponse, credential, creditLineRequest.RequestedDate);
                        }
                    }


                    if (serviceResponse != null)
                    {
                        response.DataResponse = serviceResponse;
                        response.StatusResponse = new StatusResponse()
                        {
                            Message = "Information credit line performed!",
                            Status = "OK"
                        };
                        result = Ok(response);
                    }
                    else
                    {
                        response.StatusResponse = new StatusResponse()
                        {
                            Message = "Credit line information can't be done!",
                            Status = "KO"
                        };
                        result = areTooManyRequest ? StatusCode(StatusCodes.Status429TooManyRequests, response) : BadRequest(response);
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                response.DataResponse = null;
                response.StatusResponse = new StatusResponse()
                {
                    Message = "A sales agent will contact you.",
                    Status = "KO"
                };

                result = StatusCode(StatusCodes.Status429TooManyRequests, response);
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
            finally
            {
                _userLogService.SaveRequest(creditLineRequest, credential);
            }

            return result;
        }
    }
}
