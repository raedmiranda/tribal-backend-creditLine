# tribal-backend-creditLine [![.NET](https://github.com/raedmiranda/tribal-backend-creditLine/actions/workflows/dotnet.yml/badge.svg)](https://github.com/raedmiranda/tribal-backend-creditLine/actions/workflows/dotnet.yml)  
Backend technical test to get credit line

## Domain
We've the following entities: 
### Models
* CreditLineRequestModel: the value object with the request data (special property => string FoundingType, decimal RequestedCreditLine)
* CreditLineResponseModel: the value object with the response data (special property => decimal AuthorizedCreditLine)
* CustomerCreditLine: the CreditLineResponseModel entity to be stored associated with a CustomerId 
* UserRequest: the log entity to be stored for each request the customer has made.
### Interfaces.Repositories
We work with Repository pattern, so we implement one for each type.
* IRepository: generic repository interface.
* ICreditRepository: IRepository of CustomerCreditLine.
* IUserLogRepository: IRepository of UserRequest.

## Infrastructure.DataRepositories
Implementing the repository interfaces with list of elements (List<T>) as a mockup database.
* Repository: implementation of IRepository.
 > Get and Update methods have not been implemented.
* CreditRepository: implementation of ICreditRepository using List<CustomerCreditLine> as its context.
* UserLogRepository: implementation of IUserLogRepository using List<UserRequest> as its context.

## Application
* CreditLineRequestService: application service with following methods:
  * CreditLineResponseModel DetermineCreditLine(CreditLineRequestModel request).
  * decimal CalculateCreditLine(decimal monthlyRevenue, decimal cashBalance, string foundingType).
  * bool AcceptCreditLine(decimal requestedCreditLine, decimal recommendedCreditLine).
  * CreditLineResponseModel ObtainConsumerCreditLine(string credential).
  * void SaveConsumerCreditLine(CreditLineResponseModel creditLine, string credential, DateTime requestedDate).
  * bool CheckRejectedApplications(string credential).
* UserLogService: application service with following methods:
  * bool CheckAttemptCounter(string credential, DateTime requestTime, bool lastApplicationWasAccepted).
  * void SaveRequest(CreditLineRequestModel creditLineRequest, string credential)

## WebAPI.Controllers
* CreditLineController: principal controller for calculate credit line sending a request with 'credential' header.
  
## WebAPI.Test
Execute the unit test solution to check the code coverage and responses in every path of the main controller.

### WebAPI.Test last result
  
 Nombre del grupo: CreditLineControllerTest  
 Duración: 0:00:00.049  
 0 pruebas no superadas  
 0 pruebas omitidas  
 9 pruebas superadas  

Nombre de la prueba:	Tribal.Backend.CreditLine.WebAPI.Test.CreditLineControllerTest.DetermineCreditLine_RequestIsNull  
Salida de la prueba:	Correcta

Nombre de la prueba:	Tribal.Backend.CreditLine.WebAPI.Test.CreditLineControllerTest.DetermineCreditLine_IsBadRequest  
Salida de la prueba:	Correcta

Nombre de la prueba:	Tribal.Backend.CreditLine.WebAPI.Test.CreditLineControllerTest.DetermineCreditLine_FoundingTypeIsNull  
Salida de la prueba:	Correcta

Nombre de la prueba:	Tribal.Backend.CreditLine.WebAPI.Test.CreditLineControllerTest.DetermineCreditLine_FoundingTypeIsSME  
Salida de la prueba:	Correcta

Nombre de la prueba:	Tribal.Backend.CreditLine.WebAPI.Test.CreditLineControllerTest.DetermineCreditLine_FoundingTypeIsStartup  
Salida de la prueba:	Correcta

Nombre de la prueba:	Tribal.Backend.CreditLine.WebAPI.Test.CreditLineControllerTest.DetermineCreditLine_AcceptedAndLastAttempt  
Salida de la prueba:	Correcta

Nombre de la prueba:	Tribal.Backend.CreditLine.WebAPI.Test.CreditLineControllerTest.DetermineCreditLine_AcceptedAndNotTooMany  
Salida de la prueba:	Correcta

Nombre de la prueba:	Tribal.Backend.CreditLine.WebAPI.Test.CreditLineControllerTest.DetermineCreditLine_RejectedAndTooMany  
Salida de la prueba:	Correcta

Nombre de la prueba:	Tribal.Backend.CreditLine.WebAPI.Test.CreditLineControllerTest.DetermineCreditLine_RejectedAndAgent  
Salida de la prueba:	Correcta




