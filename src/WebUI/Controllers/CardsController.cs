using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using mrs.Application.Cards.Commands.CreateCard;
using mrs.Application.Cards.Commands.DeleteCard;
using mrs.Application.Cards.Commands.DeleteCards;
using mrs.Application.Cards.Commands.ImportCards;
using mrs.Application.Cards.Commands.UpdateCard;
using mrs.Application.Cards.Commands.UpdateCards;
using mrs.Application.Cards.Queries.CheckBarcodeExist;
using mrs.Application.Cards.Queries.CheckCardDeleted;
using mrs.Application.Cards.Queries.CheckCardForPointMigration;
using mrs.Application.Cards.Queries.CheckCardForRegisterKidClubs;
using mrs.Application.Cards.Queries.CheckCardForReissueLostCard;
using mrs.Application.Cards.Queries.CheckCardsAssigned;
using mrs.Application.Cards.Queries.CheckOldCard;
using mrs.Application.Cards.Queries.CheckValidNewCard;
using mrs.Application.Cards.Queries.ExportCards;
using mrs.Application.Cards.Queries.GetCard;
using mrs.Application.Cards.Queries.GetCards;
using mrs.Application.Cards.Queries.GetCardsWithPagination;
using mrs.Application.Common.Helpers;
using mrs.Application.Common.Helpers.AzureStorage;
using mrs.Application.Common.Interfaces;
using mrs.Application.Common.Models;
using mrs.Application.MigrateCard.Commands.AddMigrateRequest;
using mrs.Application.MissingCard.Commands.AddPendingRequestList;
using mrs.Application.MissingCard.Commands.ConfirmPendingRequest;
using mrs.Application.MissingCard.Commands.GetPendingRequestDetail;
using mrs.Application.MissingCard.Queries.GetRequestTypes;
using mrs.Application.MissingCard.Queries.SearchPendingRequestListWithPagination;
using mrs.Application.ReferPrepaidCard.Commands.GetPrepaidCardDetail;
using mrs.Application.ReferPrepaidCard.Queries.SearchPrepaidCard;
using mrs.WebUI.Filters;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace mrs.WebUI.Controllers
{
    [Authorize]
    public class CardsController : ApiControllerBase
    {
        private readonly IAzureStorageHelper _azureStorageHelpers;
        private static class LoggingMessage
        {
            public static readonly string BatchDeletePrepaidCard = "削除済みカードです。 ({0})";
            public static readonly string GetCard = "カードを正常に取得しました。 ({0})";
            public static readonly string CreateCard = "カードを正常に登録できました。 ({0})";
            public static readonly string DeleteCard = "カードを正常に削除できました。 ({0})";
            public static readonly string UpdateCard = "カードを正常に変更しました。 ({0})";
            public static readonly string CheckBarcodeExist = "バーコード存在をチェックしました。 ({0})";
            public static readonly string ImportCard = "カードを正常に取り込みしました。 ({0})";
            public static readonly string CheckCardsAssigned = "カードが正常にお客様に割り当てられたか確認しました。 ({0})";
            public static readonly string ValidateOldCard = "旧カードの有効性を正常に確認しました。 ({0})";
            public static readonly string ValidateOldCardNotOK = "旧カードの有効期限確認に失敗しました。({0})";
            public static readonly string ValidateNewCard = "新カードの有効性を正常に確認しました。 ({0})";
            public static readonly string ValidateNewCardNotOK = "新カードの有効期限確認に失敗しました。({0})";
            public static readonly string AddPendingRequest = "統合依頼保留データを正常に追加しました。({0})";
            public static readonly string SearchPendingRequest = "統合依頼保留データを正常に検索しました。";
            public static readonly string GetPendingRequest = "統合依頼保留データを正常に取得しました。({0})";
            public static readonly string ConfirmPendingRequestSuccessfully = "統合依頼保留データを正常に確認しました。({0})";
            public static readonly string ConfirmPendingRequestFailed = "統合依頼保留データの認証に失敗しました。({0})";
            public static readonly string GetCards = "カード一覧を正常に取得しました。";
            public static readonly string ExportCards = "Eカード一覧を正常に出力しました。";
            public static readonly string GetCardForEditing = "一括更新用のカード一覧を正常に取得しました。";
            public static readonly string GetCardForDeleting = "一括削除用のカード一覧を正常に取得しました。";
            public static readonly string AddMigrateRequest = "ポイント交換申請を正常に登録しました。({0})";
            public static readonly string SearchPrepaidCard = "プリペイドカードを正常に検索しました。";
            public static readonly string GetPrepaidCard = "プリペイドカードを正常に取得しました。({0})";
        }
        public CardsController(ICurrentUserService currentUserService, IIdentityService identityService, IConfiguration configuration)
        {
            _azureStorageHelpers = new AzureStorageHelper(currentUserService, identityService, configuration);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create(CreateCardCommand command)
        {
            var createdId = await Mediator.Send(command);
            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.CreateCard, command.MemberNo));
            return createdId;
        }

        [HttpPut("{id}")]
        [CustomAuthorizeFilter(RoleLevel.Level_8, RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateCardCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }

            var memberNo = await Mediator.Send(command);
            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.UpdateCard, memberNo));
            return NoContent();
        }

        [HttpGet("{id}")]
        [CustomAuthorizeFilter(RoleLevel.Level_6, RoleLevel.Level_7, RoleLevel.Level_8, RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<ActionResult<CardDto>> Get(int id)
        {
            var cardItem = await Mediator.Send(new GetCardQuery { Id = id });
            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.GetCard, cardItem.MemberNo));
            return cardItem;
        }

        [HttpPut("[action]")]
        public async Task<ActionResult> UpdateItemDetails(int id, UpdateCardCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }

            var memberNo = await Mediator.Send(command);
            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.UpdateCard, memberNo));
            return NoContent();
        }

        [HttpDelete("{id}")]
        [CustomAuthorizeFilter(RoleLevel.Level_8, RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<ActionResult> Delete(int id)
        {
            var memberNo = await Mediator.Send(new DeleteCardCommand { Id = id });
            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.DeleteCard, memberNo));
            return NoContent();
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<bool>> CheckExist(string memberNo)
        {
            var isExisted = await Mediator.Send(new CheckBarcodeExistQuery { MemberNo = memberNo });
            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.CheckBarcodeExist, memberNo));
            return isExisted;
        }

        [HttpPost("[action]")]
        [CustomAuthorizeFilter(RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<ActionResult<int>> ImportCards([FromForm] ImportCardsCommand command)
        {
            var result = await Mediator.Send(command);
            StringBuilder messageBuilder = new StringBuilder();
            foreach (var memberNo in result.Item2)
            {
                string messageOnly = string.Format(LoggingMessage.ImportCard, memberNo);
                string messageFull = await _azureStorageHelpers.ConfigureMessage(messageOnly);
                messageBuilder.AppendLine(messageFull);
            }
            await _azureStorageHelpers.SaveMultipleLogToBlob(messageBuilder.ToString());
            return result.Item1;
        }

        [HttpPost("[action]")]
        [DisableRequestSizeLimit]
        [CustomAuthorizeFilter(RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<ActionResult<bool>> EditCards(UpdateCardsCommand command)
        {
            var result = await Mediator.Send(command);
            StringBuilder messageBuilder = new StringBuilder();
            foreach (var memberNo in result.Item2)
            {
                string messageOnly = string.Format(LoggingMessage.UpdateCard, memberNo);
                string messageFull = await _azureStorageHelpers.ConfigureMessage(messageOnly);
                messageBuilder.AppendLine(messageFull);
            }
            await _azureStorageHelpers.SaveMultipleLogToBlob(messageBuilder.ToString());
            return result.Item1;
        }

        [HttpPost("[action]")]
        [DisableRequestSizeLimit]
        [CustomAuthorizeFilter(RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<ActionResult<bool>> DeleteCards(DeleteCardsCommand command)
        {
            var result = await Mediator.Send(command);
            StringBuilder messageBuilder = new StringBuilder();
            foreach(var memberNo in result.Item2)
            {
                string messageOnly = string.Format(LoggingMessage.BatchDeletePrepaidCard, memberNo);
                string messageFull = await _azureStorageHelpers.ConfigureMessage(messageOnly);
                messageBuilder.AppendLine(messageFull);
            }
            await _azureStorageHelpers.SaveMultipleLogToBlob(messageBuilder.ToString());
            return result.Item1;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<bool>> CheckCardsAssigned(string memberNo)
        {
            var result = await Mediator.Send(new CheckCardsAssignedQuery() { MemberNo = memberNo });
            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.CheckCardsAssigned, memberNo));
            return result;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<string>> CheckValidOldCard(string memberNo)
        {
            var result = await Mediator.Send(new CheckValidOldCardQuery() { MemberNo = memberNo });
            await _azureStorageHelpers.SaveLogToBlob(string.Format(result == CardCheckStatusCode.OK ? LoggingMessage.ValidateOldCard : LoggingMessage.ValidateOldCardNotOK, memberNo));
            return result;
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<string>> CheckValidCardForRegisterKidClubs(string memberNo)
        {
            var result = await Mediator.Send(new CheckValidCardForRegisterKidClubsQuery() { MemberNo = memberNo });
            await _azureStorageHelpers.SaveLogToBlob(string.Format(result == CardCheckStatusCode.OK ? LoggingMessage.ValidateOldCard : LoggingMessage.ValidateOldCardNotOK, memberNo));
            return result;
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<string>> CheckCardPointMigrationReceivePoint(string memberNo)
        {
            var result = await Mediator.Send(new CheckCardPointMigrationReceivePointQuery() { MemberNo = memberNo });
            await _azureStorageHelpers.SaveLogToBlob(string.Format(result == CardCheckStatusCode.OK ? LoggingMessage.ValidateOldCard : LoggingMessage.ValidateOldCardNotOK, memberNo));
            return result;
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<string>> CheckCardPointMigrationGivePoint(string memberNo, string memberNoReceivePoint)
        {
            var result = await Mediator.Send(new CheckCardPointMigrationGivePointQuery() { MemberNo = memberNo, MemberNoReceivePoint = memberNoReceivePoint });
            await _azureStorageHelpers.SaveLogToBlob(string.Format(result == CardCheckStatusCode.OK ? LoggingMessage.ValidateOldCard : LoggingMessage.ValidateOldCardNotOK, memberNo));
            return result;
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<string>> CheckCardForReissueLostCard(string memberNo)
        {
            var result = await Mediator.Send(new CheckCardForReissueLostCardQuery() { MemberNo = memberNo });
            await _azureStorageHelpers.SaveLogToBlob(string.Format(result == CardCheckStatusCode.OK ? LoggingMessage.ValidateOldCard : LoggingMessage.ValidateOldCardNotOK, memberNo));
            return result;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<string>> CheckValidNewCard(string memberNo)
        {
            var result = await Mediator.Send(new CheckValidNewCardQuery() { MemberNo = memberNo });
            await _azureStorageHelpers.SaveLogToBlob(string.Format(result == CardCheckStatusCode.OK ? LoggingMessage.ValidateNewCard : LoggingMessage.ValidateNewCardNotOK, memberNo));
            return result;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<int>> AddCardPendingList(AddPendingRequestListCommand command)
        {
            var createdId = await Mediator.Send(command);
            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.AddPendingRequest, createdId));
            return createdId;
        }

        [HttpGet("[action]")]
        [CustomAuthorizeFilter(RoleLevel.Level_2, RoleLevel.Level_3)]
        public async Task<ActionResult<PaginatedList<PendingRequestListDto>>> SearchPendingListWithPagination([FromQuery] SearchPendingListWithPaginationQuery query)
        {
            var pendingRequests = await Mediator.Send(query);
            await _azureStorageHelpers.SaveLogToBlob(LoggingMessage.SearchPendingRequest);
            return pendingRequests;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<PendingRequestDetailDto>> GetPendingCardDetail([FromQuery] GetPendingRequestDetailCommand query)
        {
            var pendingRequest = await Mediator.Send(query);
            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.GetPendingRequest, query.Id));
            return pendingRequest;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<bool>> ConfirmPendingCard(ConfirmPendingRequestCommand command)
        {
            var result = await Mediator.Send(command);
            await _azureStorageHelpers.SaveLogToBlob(string.Format(
                result ? LoggingMessage.ConfirmPendingRequestSuccessfully : LoggingMessage.ConfirmPendingRequestFailed,
                command.PendingId));
            return result;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<List<RequestTypeDto>>> GetListClassifyRequest()
        {
            var requestTypes = await Mediator.Send(new GetRequestTypesQuery());
            return requestTypes;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<bool>> CheckDeleted(int id)
        {
            var result = await Mediator.Send(new CheckCardDeletedQuery { Id = id });
            return result;
        }
        
        [HttpGet("[action]")]
        [CustomAuthorizeFilter(RoleLevel.Level_6, RoleLevel.Level_7, RoleLevel.Level_8, RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<ActionResult<PaginatedList<CardDto>>> GetCardsWithPagination(string sortCol = "", string sortType = "", string currentPage = "", string itemPerPage = "", string memberNo = "", string deviceCode = "", string expirationDate = "", string companyCode = "", string storeCode = "", string status = "", string acceptFrom = "", string acceptTo = "", string acceptBy = "")
        {
            GetCardsWithPaginationQuery param = new GetCardsWithPaginationQuery()
            {
                AcceptBy = acceptBy,
                AcceptFrom = acceptFrom,
                AcceptTo = acceptTo,
                CompanyCode = companyCode,
                DeviceCode = deviceCode,
                ExpiredAt = expirationDate,
                MemberNo = memberNo,
                PageNumber = currentPage,
                PageSize = itemPerPage,
                SortBy = sortCol,
                SortType = sortType,
                Status = status,
                StoreCode = storeCode
            };
            var cards = await Mediator.Send(param);
            await _azureStorageHelpers.SaveLogToBlob(LoggingMessage.GetCards);
            return cards;
        }

        [HttpGet("[action]")]
        [CustomAuthorizeFilter(RoleLevel.Level_7, RoleLevel.Level_8, RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<FileResult> GetCardsExport(string memberNo = "", string deviceCode = "", string expirationDate = "", string companyCode = "", string storeCode = "", string status = "", string acceptFrom = "", string acceptTo = "", string acceptBy = "")
        {
            ExportCardsQuery param = new ExportCardsQuery()
            {
                AcceptBy = acceptBy,
                AcceptFrom = acceptFrom,
                AcceptTo = acceptTo,
                CompanyCode = companyCode,
                DeviceCode = deviceCode,
                ExpiredAt = expirationDate,
                MemberNo = memberNo,
                Status = status,
                StoreCode = storeCode
            };
            var vm = await Mediator.Send(param);
            await _azureStorageHelpers.SaveLogToBlob(LoggingMessage.ExportCards);
            return File(vm.Content, vm.ContentType, vm.FileName);
        }

        [HttpGet("[action]")]
        [CustomAuthorizeFilter(RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<ActionResult<PaginatedList<CardDto>>> GetCardsEditWithPagination(string sortCol = "", string sortType = "", string currentPage = "", string itemPerPage = "", string memberNoFrom = "", string memberNoTo = "", string companyCode = "", string storeCode = "", string registeredFrom = "", string registeredTo = "", string registeredBy = "", string status = "", string expirationDate = "")
        {
            GetCardsEditWithPaginationQuery param = new GetCardsEditWithPaginationQuery()
            {
                CompanyCode = companyCode,
                ExpiredAt = expirationDate,
                MemberNoFrom = memberNoFrom,
                MemberNoTo = memberNoTo,
                RegisteredFrom = registeredFrom,
                RegisteredTo = registeredTo,
                RegisteredBy = registeredBy,
                PageNumber = currentPage,
                PageSize = itemPerPage,
                SortBy = sortCol,
                SortType = sortType,
                Status = status,
                StoreCode = storeCode
            };
            var cards = await Mediator.Send(param);
            await _azureStorageHelpers.SaveLogToBlob(LoggingMessage.GetCardForEditing);
            return cards;
        }

        [HttpGet("[action]")]
        [CustomAuthorizeFilter(RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<ActionResult<PaginatedList<CardDto>>> GetCardsDeleteWithPagination(string sortCol = "", string sortType = "", string currentPage = "", string itemPerPage = "", string memberNoFrom = "", string memberNoTo = "", string companyCode = "", string storeCode = "", string registeredFrom = "", string registeredTo = "", string registeredBy = "", string status = "", string expirationDate = "")
        {
            GetCardsDeleteWithPaginationQuery param = new GetCardsDeleteWithPaginationQuery()
            {
                CompanyCode = companyCode,
                ExpiredAt = expirationDate,
                MemberNoFrom = memberNoFrom,
                MemberNoTo = memberNoTo,
                RegisteredFrom = registeredFrom,
                RegisteredTo = registeredTo,
                RegisteredBy = registeredBy,
                PageNumber = currentPage,
                PageSize = itemPerPage,
                SortBy = sortCol,
                SortType = sortType,
                Status = status,
                StoreCode = storeCode
            };
            var cards = await Mediator.Send(param);
            await _azureStorageHelpers.SaveLogToBlob(LoggingMessage.GetCardForDeleting);
            return cards;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<int>> AddMigrateRequest(AddMigrateRequestCommand command)
        {
            var result = await Mediator.Send(command);
            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.AddMigrateRequest, result.ToString()));
            return result;
        }

        [HttpGet("[action]")]
        [CustomAuthorizeFilter(RoleLevel.Level_2, RoleLevel.Level_3)]
        public async Task<ActionResult<PaginatedList<PrepaidCardDto>>> SearchPrepaidCard([FromQuery] SearchPrepaidCardQuery query)
        {
            var prepaidCards = await Mediator.Send(query);
            await _azureStorageHelpers.SaveLogToBlob(LoggingMessage.SearchPrepaidCard);
            return prepaidCards;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<PrepaidCardDetailDto>> GetPrepaidCardDetail(int id)
        {
            var prepaidCard = await Mediator.Send(new GetPrepaidCardDetailCommand() { Id = id });
            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.GetPrepaidCard, prepaidCard.CustomerNo));
            return prepaidCard;
        }
    }
}
