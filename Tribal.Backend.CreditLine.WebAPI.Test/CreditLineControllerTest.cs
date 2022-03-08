using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Tribal.Backend.CreditLine.Application;
using Tribal.Backend.CreditLine.Domain;
using Tribal.Backend.CreditLine.Domain.Models;
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


        [Fact]
        public void DetermineCreditLine_AcceptedAndLastAttempt()
        {
            var expectedCustomerIdValue = Guid.NewGuid().ToString();

            _creditContext = new List<CustomerCreditLine>();

            AddCreditLines(expectedCustomerIdValue, true);
            AddUserLogs(expectedCustomerIdValue);
            InitializeRepositories();

            CreditLineRequestModel request = new()
            {
                FoundingType = "Startup",
                CashBalance = 3000,
                MonthlyRevenue = 1000,
                RequestedCreditLine = 999,
                RequestedDate = DateTime.Now
            };

            ActionResult<ObjectResponse<CreditLineResponseModel>> actionResult = _creditLineController.DetermineCreditLine(request, expectedCustomerIdValue);
            var result = actionResult.Result as ObjectResult;
            Assert.Equal(429, result.StatusCode);
        }

        [Fact]
        public void DetermineCreditLine_AcceptedAndNotTooMany()
        {
            var expectedCustomerIdValue = Guid.NewGuid().ToString();

            _creditContext = new List<CustomerCreditLine>();

            AddCreditLines(expectedCustomerIdValue, true, 1);
            AddUserLogs(expectedCustomerIdValue, 1);
            InitializeRepositories();

            CreditLineRequestModel request = new()
            {
                FoundingType = "Startup",
                CashBalance = 3000,
                MonthlyRevenue = 1000,
                RequestedCreditLine = 999,
                RequestedDate = DateTime.Now
            };

            ActionResult<ObjectResponse<CreditLineResponseModel>> actionResult = _creditLineController.DetermineCreditLine(request, expectedCustomerIdValue);
            var result = actionResult.Result as OkObjectResult;
            Assert.Equal(1000, (result.Value as ObjectResponse<CreditLineResponseModel>).DataResponse.AuthorizedCreditLine);
        }

        [Fact]
        public void DetermineCreditLine_RejectedAndTooMany()
        {
            var expectedCustomerIdValue = Guid.NewGuid().ToString();

            _creditContext = new List<CustomerCreditLine>();

            AddCreditLines(expectedCustomerIdValue, false, 1);
            AddUserLogs(expectedCustomerIdValue, 1);
            InitializeRepositories();

            CreditLineRequestModel request = new()
            {
                FoundingType = "Startup",
                CashBalance = 3000,
                MonthlyRevenue = 1000,
                RequestedCreditLine = 999,
                RequestedDate = DateTime.Now
            };

            ActionResult<ObjectResponse<CreditLineResponseModel>> actionResult = _creditLineController.DetermineCreditLine(request, expectedCustomerIdValue);
            var result = actionResult.Result as ObjectResult;
            Assert.Equal(429, result.StatusCode);
        }

        [Fact]
        public void DetermineCreditLine_RejectedAndAgent()
        {
            var expectedCustomerIdValue = Guid.NewGuid().ToString();

            _creditContext = new List<CustomerCreditLine>();

            AddCreditLines(expectedCustomerIdValue, false);
            AddUserLogs(expectedCustomerIdValue);
            InitializeRepositories();

            CreditLineRequestModel request = new()
            {
                FoundingType = "Startup",
                CashBalance = 3000,
                MonthlyRevenue = 1000,
                RequestedCreditLine = 999,
                RequestedDate = DateTime.Now
            };


            ActionResult<ObjectResponse<CreditLineResponseModel>> actionResult = _creditLineController.DetermineCreditLine(request, expectedCustomerIdValue);
            var result = actionResult.Result as ObjectResult;
            var expectedMessage = "A sales agent will contact you.";
            Assert.Equal(expectedMessage, (result.Value as ObjectResponse<CreditLineResponseModel>).StatusResponse.Message);
        }



        private void AddCreditLines(string expectedCustomerIdValue, bool status, int count = 3)
        {
            if (count >= 1)
                _creditContext.Add(new CustomerCreditLine()
                {
                    AcceptedCreditLine = 1000,
                    AcceptedStatus = status,
                    CreatedAt = DateTime.Now.AddMinutes(-0.5),
                    CustomerId = Guid.Parse(expectedCustomerIdValue),
                    Guid = new Guid()
                });
            if (count >= 2)
                _creditContext.Add(new CustomerCreditLine()
                {
                    AcceptedCreditLine = 5000,
                    AcceptedStatus = status,
                    CreatedAt = DateTime.Now.AddMinutes(-1),
                    CustomerId = Guid.Parse(expectedCustomerIdValue),
                    Guid = new Guid()
                });
            if (count >= 3)
                _creditContext.Add(new CustomerCreditLine()
                {
                    AcceptedCreditLine = 8000,
                    AcceptedStatus = status,
                    CreatedAt = DateTime.Now.AddMinutes(-1.5),
                    CustomerId = Guid.Parse(expectedCustomerIdValue),
                    Guid = new Guid()
                });
        }

        private void AddUserLogs(string expectedCustomerIdValue, int count = 3)
        {
            if (count >= 1)
                _userLogContext.Add(new UserRequest()
                {
                    RequestType = "DETERMINECREDITLINE",
                    CreatedAt = DateTime.Now.AddMinutes(-0.5),
                    CustomerId = Guid.Parse(expectedCustomerIdValue),
                    Guid = new Guid()
                });
            if (count >= 2)
                _userLogContext.Add(new UserRequest()
                {
                    RequestType = "DETERMINECREDITLINE",
                    CreatedAt = DateTime.Now.AddMinutes(-1),
                    CustomerId = Guid.Parse(expectedCustomerIdValue),
                    Guid = new Guid()
                });
            if (count >= 3)
                _userLogContext.Add(new UserRequest()
                {
                    RequestType = "DETERMINECREDITLINE",
                    CreatedAt = DateTime.Now.AddMinutes(-1.5),
                    CustomerId = Guid.Parse(expectedCustomerIdValue),
                    Guid = new Guid()
                });
        }
    }
}
