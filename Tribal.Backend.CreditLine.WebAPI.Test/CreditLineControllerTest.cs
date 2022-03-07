using Microsoft.AspNetCore.Mvc;
using System;
using Tribal.Backend.CreditLine.Application;
using Tribal.Backend.CreditLine.Domain;
using Tribal.Backend.CreditLine.Domain.Models;
using Tribal.Backend.CreditLine.WebAPI.Communication;
using Tribal.Backend.CreditLine.WebAPI.Controllers;
using Xunit;

namespace Tribal.Backend.CreditLine.WebAPI.Test
{
    public class CreditLineControllerTest
    {
        CreditLineController _creditLineController;
        CreditLineRequestService _creditLineRequestService;

        public CreditLineControllerTest()
        {
            _creditLineRequestService = new();
            _creditLineController = new(_creditLineRequestService);
        }


        [Fact]
        public void DetermineCreditLineWhenRequestIsNull()
        {
            CreditLineRequestModel request = null;

            ActionResult<ObjectResponse<CreditLineResponseModel>> actionResult = _creditLineController.DetermineCreditLine(request);
            var result = actionResult.Result as BadRequestObjectResult;
            Assert.Equal("KO", (result.Value as ObjectResponse<CreditLineResponseModel>).StatusResponse.Status);
        }

        [Fact]
        public void DetermineCreditLineWhenIsBadRequest()
        {
            CreditLineRequestModel request = new()
            {
                FoundingType = "",
                CashBalance = 0,
                MonthlyRevenue = -3.14M,
                RequestedCreditLine = 999999,
                RequestedDate = DateTime.Now
            };

            ActionResult<ObjectResponse<CreditLineResponseModel>> actionResult = _creditLineController.DetermineCreditLine(request);
            var result = actionResult.Result as BadRequestObjectResult;
            Assert.Equal("KO", (result.Value as ObjectResponse<CreditLineResponseModel>).StatusResponse.Status);
        }


        [Fact]
        public void DetermineCreditLineWhenFoundingTypeIsNull()
        {
            CreditLineRequestModel request = new()
            {
                FoundingType = "",
                CashBalance = 0,
                MonthlyRevenue = 0,
                RequestedCreditLine = 1000,
                RequestedDate = DateTime.Now
            };

            ActionResult<ObjectResponse<CreditLineResponseModel>> actionResult = _creditLineController.DetermineCreditLine(request);
            var result = actionResult.Result as BadRequestObjectResult;
            Assert.Equal("KO", (result.Value as ObjectResponse<CreditLineResponseModel>).StatusResponse.Status);
        }


        [Fact]
        public void DetermineCreditLineWhenFoundingTypeIsSME()
        {
            CreditLineRequestModel request = new()
            {
                FoundingType = "SME",
                CashBalance = 0,
                MonthlyRevenue = 1000,
                RequestedCreditLine = 199,
                RequestedDate = DateTime.Now
            };

            ActionResult<ObjectResponse<CreditLineResponseModel>> actionResult = _creditLineController.DetermineCreditLine(request);
            var result = actionResult.Result as OkObjectResult;
            Assert.Equal(200, (result.Value as ObjectResponse<CreditLineResponseModel>).DataResponse.AuthorizedCreditLine);
        }


        [Fact]
        public void DetermineCreditLineWhenFoundingTypeIsStartup()
        {
            CreditLineRequestModel request = new()
            {
                FoundingType = "Startup",
                CashBalance = 3000,
                MonthlyRevenue = 1000,
                RequestedCreditLine = 999,
                RequestedDate = DateTime.Now
            };

            ActionResult<ObjectResponse<CreditLineResponseModel>> actionResult = _creditLineController.DetermineCreditLine(request);
            var result = actionResult.Result as OkObjectResult;
            Assert.Equal(1000, (result.Value as ObjectResponse<CreditLineResponseModel>).DataResponse.AuthorizedCreditLine);
        }
    }
}
