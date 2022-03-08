using System;
using System.Collections.Generic;
using System.Linq;
using Tribal.Backend.CreditLine.Domain;
using Tribal.Backend.CreditLine.Domain.Enums;
using Tribal.Backend.CreditLine.Domain.Models;
using Tribal.Backend.CreditLine.Infrastructure.DataRepositories;

namespace Tribal.Backend.CreditLine.Application
{
    public class CreditLineRequestService
    {
        private const int RATIO_CASHBALANCE = 3;
        private const int RATIO_MONTHLYREVENUE = 5;

        private const int TIMES_TOFAIL = 3;

        CreditRepository _creditRepository;

        public CreditLineRequestService(CreditRepository creditRepository)
        {
            _creditRepository = creditRepository ?? throw new ArgumentNullException(nameof(creditRepository), "Argument can not be null");
        }

        public CreditLineResponseModel DetermineCreditLine(CreditLineRequestModel request)
        {
            CreditLineResponseModel response = null;
            decimal authorizedCreditLine;
            bool isAccepted;
            try
            {
                authorizedCreditLine = CalculateCreditLine(request.MonthlyRevenue, request.CashBalance, request.FoundingType);
                isAccepted = AcceptCreditLine(request.RequestedCreditLine, authorizedCreditLine);

                response = new();
                response.AuthorizedCreditLine = authorizedCreditLine;
                response.IsAccepted = isAccepted;
            }
            catch (ArgumentNullException ex)
            {
                response = null;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error in DetermineCreditLine method.", ex);
            }

            return response;
        }

        public decimal CalculateCreditLine(decimal monthlyRevenue, decimal cashBalance, string foundingType)
        {
            try
            {
                decimal creditLineByMonthlyRevenue = monthlyRevenue / RATIO_MONTHLYREVENUE;
                decimal creditLineByCashBalance = cashBalance / RATIO_CASHBALANCE;
                bool checkFoundingTypeSME;

                if (String.IsNullOrWhiteSpace(foundingType))
                    throw new ArgumentNullException(nameof(foundingType));
                checkFoundingTypeSME = foundingType == FoundingTypeEnum.SME.ToString();

                decimal recommendedCreditLine;
                if (checkFoundingTypeSME)
                {
                    recommendedCreditLine = creditLineByMonthlyRevenue;
                }
                else
                {
                    recommendedCreditLine = Math.Max(creditLineByMonthlyRevenue, creditLineByCashBalance);
                }

                return recommendedCreditLine;

            }
            catch (ArgumentNullException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error in CalculateCreditLine", ex);
            }
        }

        public bool AcceptCreditLine(decimal requestedCreditLine, decimal recommendedCreditLine)
        {
            bool passedCreditLine = false;

            passedCreditLine = recommendedCreditLine > requestedCreditLine;

            return passedCreditLine;
        }

        public CreditLineResponseModel ObtainConsumerCreditLine(string credential)
        {
            CreditLineResponseModel response = new();

            try
            {
                Guid customerId = Guid.Parse(credential);
                CustomerCreditLine item = _creditRepository.Find(x => x.CustomerId == customerId)
                                                          .OrderBy(x => x.CreatedAt)
                                                          .First();

                if (item != null)
                {
                    response.AuthorizedCreditLine = item.AcceptedCreditLine;
                    response.IsAccepted = item.AcceptedStatus;
                }
            }
            catch (Exception)
            {
                response = null;
            }

            return response;
        }

        public void SaveConsumerCreditLine(CreditLineResponseModel creditLine, string credential, DateTime requestedDate)
        {
            try
            {
                if (creditLine != null)
                {

                    Guid customerId = Guid.Parse(credential);
                    CustomerCreditLine item = new()
                    {
                        AcceptedCreditLine = creditLine.AuthorizedCreditLine,
                        AcceptedStatus = creditLine.IsAccepted,
                        CreatedAt = requestedDate,
                        CustomerId = customerId,
                        Guid = Guid.NewGuid()
                    };

                    _creditRepository.Insert(item);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in SaveCreditLine method", ex);
            }
        }

        public bool CheckRejectedApplications(string credential)
        {
            bool response = false;
            int count = TIMES_TOFAIL;
            int actualCount = 0;

            try
            {
                Guid customerId = Guid.Parse(credential);

                // Take his last 3 credit lines and check if these was rejected
                List<CustomerCreditLine> rejectedCreditLines = _creditRepository.Find(x => x.CustomerId == customerId)
                                                          .OrderBy(x => x.CreatedAt)
                                                          .Take(count)
                                                          .Where(creditLine => creditLine.AcceptedStatus == false)
                                                          .ToList();
                actualCount = rejectedCreditLines.Count;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in CheckRejectedApplications method", ex);
            }

            response = actualCount >= count;
            return response;
        }
    }
}
