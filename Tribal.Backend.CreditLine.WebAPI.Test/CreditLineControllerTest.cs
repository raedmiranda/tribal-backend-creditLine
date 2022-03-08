using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Tribal.Backend.CreditLine.Application;
using Tribal.Backend.CreditLine.Domain;
using Tribal.Backend.CreditLine.Domain.Models;
using Tribal.Backend.CreditLine.Infrastructure;
using Tribal.Backend.CreditLine.Infrastructure.DataRepositories;
using Tribal.Backend.CreditLine.WebAPI.Communication;
using Tribal.Backend.CreditLine.WebAPI.Controllers;
using Xunit;

namespace Tribal.Backend.CreditLine.WebAPI.Test
{
    public class CreditLineControllerTest
    {
        List<CustomerCreditLine> _creditContext;
        CreditRepository _creditRepository;
        CreditLineRequestService _creditLineRequestService;
        List<UserRequest> _userLogContext;
        UserLogRepository _userLogRepository;
        UserLogService _userLogService;
        CreditLineController _creditLineController;

        public CreditLineControllerTest()
        {
            _creditContext = new List<CustomerCreditLine>();
            _userLogContext = new List<UserRequest>();
            InitializeRepositories();
        }

        private void InitializeRepositories()
        {
            _creditRepository = new CreditRepository(_creditContext);
            _creditLineRequestService = new CreditLineRequestService(_creditRepository);

            _userLogRepository = new UserLogRepository(_userLogContext);
            _userLogService = new UserLogService(_userLogRepository);

            _creditLineController = new(_creditLineRequestService, _userLogService);
        }



        [Fact]
        public void DetermineCreditLine_RequestIsNull()
        {
            CreditLineRequestModel request = null;

            var expectedCustomerIdValue = Guid.NewGuid().ToString();

            ActionResult<ObjectResponse<CreditLineResponseModel>> actionResult = _creditLineController.DetermineCreditLine(request, expectedCustomerIdValue);
            var result = actionResult.Result as BadRequestObjectResult;
            Assert.Equal("KO", (result.Value as ObjectResponse<CreditLineResponseModel>).StatusResponse.Status);
        }

        [Fact]
        public void DetermineCreditLine_IsBadRequest()
        {
            CreditLineRequestModel request = new()
            {
                FoundingType = "",
                CashBalance = 0,
                MonthlyRevenue = -3.14M,
                RequestedCreditLine = 999999,
                RequestedDate = DateTime.Now
            };

            var expectedCustomerIdValue = Guid.NewGuid().ToString();

            ActionResult<ObjectResponse<CreditLineResponseModel>> actionResult = _creditLineController.DetermineCreditLine(request, expectedCustomerIdValue);
            var result = actionResult.Result as BadRequestObjectResult;
            Assert.Equal("KO", (result.Value as ObjectResponse<CreditLineResponseModel>).StatusResponse.Status);
        }


        [Fact]
        public void DetermineCreditLine_FoundingTypeIsNull()
        {
            CreditLineRequestModel request = new()
            {
                FoundingType = "",
                CashBalance = 0,
                MonthlyRevenue = 0,
                RequestedCreditLine = 1000,
                RequestedDate = DateTime.Now
            };

            var expectedCustomerIdValue = Guid.NewGuid().ToString();

            ActionResult<ObjectResponse<CreditLineResponseModel>> actionResult = _creditLineController.DetermineCreditLine(request, expectedCustomerIdValue);
            var result = actionResult.Result as BadRequestObjectResult;
            Assert.Equal("KO", (result.Value as ObjectResponse<CreditLineResponseModel>).StatusResponse.Status);
        }


        [Fact]
        public void DetermineCreditLine_FoundingTypeIsSME()
        {
            CreditLineRequestModel request = new()
            {
                FoundingType = "SME",
                CashBalance = 0,
                MonthlyRevenue = 1000,
                RequestedCreditLine = 199,
                RequestedDate = DateTime.Now
            };

            var expectedCustomerIdValue = Guid.NewGuid().ToString();

            ActionResult<ObjectResponse<CreditLineResponseModel>> actionResult = _creditLineController.DetermineCreditLine(request, expectedCustomerIdValue);
            var result = actionResult.Result as OkObjectResult;
            Assert.Equal(200, (result.Value as ObjectResponse<CreditLineResponseModel>).DataResponse.AuthorizedCreditLine);
        }


        [Fact]
        public void DetermineCreditLine_FoundingTypeIsStartup()
        {
            CreditLineRequestModel request = new()
            {
                FoundingType = "Startup",
                CashBalance = 3000,
                MonthlyRevenue = 1000,
                RequestedCreditLine = 999,
                RequestedDate = DateTime.Now
            };

            var expectedCustomerIdValue = Guid.NewGuid().ToString();

            ActionResult<ObjectResponse<CreditLineResponseModel>> actionResult = _creditLineController.DetermineCreditLine(request, expectedCustomerIdValue);
            var result = actionResult.Result as OkObjectResult;
            Assert.Equal(1000, (result.Value as ObjectResponse<CreditLineResponseModel>).DataResponse.AuthorizedCreditLine);
        }
    }
}
