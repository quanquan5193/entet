using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using mrs.Application.Common.Helpers;
using mrs.Application.Common.Interfaces;
using mrs.Application.Common.Models;
using mrs.Application.InquiryKidClub.Commands.GetInquiryKidClubDetail;
using mrs.Application.InquiryKidClub.Queries.SearchInquiryKidClubWithPagination;
using mrs.Application.Members.Commands.LeaveGroupRequest;
using mrs.Application.Members.Commands.RegisterKidsClub;
using mrs.Application.Members.Commands.RegisterMember;
using mrs.Application.Members.Commands.UpdateMember;
using mrs.Application.Members.Commands.UpdateMemberNo;
using mrs.Application.Members.Queries.GetMemberInfo;
using mrs.Application.Members.Queries.PersonIncharge;
using mrs.Application.Members.Queries.PersonInchargeCMS;
using mrs.Application.Members.Queries.UpdateRemarkPIC;
using mrs.Application.Members.Queries.ValidateCardForRegister;
using mrs.Application.ZipCodes.Queries.GetInfoZipcode;
using mrs.Application.ZipCodes.Queries.ImportZipcode;
using mrs.Domain.Enums;
using mrs.WebUI.Filters;
using mrs.Application.Common.Helpers.AzureStorage;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace mrs.WebUI.Controllers
{
    [Authorize]
    public class MembersController : ApiControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private const string folderZipcode = "ZipcodeFile";
        private readonly IAzureStorageHelper _azureStorageHelpers;
        private static class LoggingMessage
        {
            public static readonly string ImportZipcode = "郵便番号を正常にインポートしました。";
            public static readonly string ValidateCardForRegister = "カードの有効性を正常に確認しました。({0})";
            public static readonly string MemberAddNew = "会員登録申請を正常に新規登録しました。({0})";
            public static readonly string MemberSwitchCard = "カード変更申請を正常に登録しました。({0})";
            public static readonly string MemberReIssued = "カード紛失再発行申請を正常に登録しました。({0})";
            public static readonly string MemberChangeInfo = "会員情報変更申請を正常に登録しました。({0})";
            public static readonly string MemberLeaveGroup = "会員退会申請を正常に登録しました。({0})";
            public static readonly string MemberPointMigrate = "ポイント交換申請を正常に登録しました。({0})";
            public static readonly string MemberRegisterKidClub = "お子様会員登録申請を正常に作成しました。({0})";
            public static readonly string UpdateRemarkAndPIC = "担当者と備考を正常に更新しました。({0})";
            public static readonly string SearchInquiryKidClub = "お子様会員登録申請一覧を正常に検索しました。";
            public static readonly string GetInquiryKidClub = "お子様会員登録申請を正常に取得しました。({0})";
        }

        public MembersController(IWebHostEnvironment env, ICurrentUserService currentUserService, IIdentityService identityService, IConfiguration configuration)
        {
            _env = env;
            _azureStorageHelpers = new AzureStorageHelper(currentUserService, identityService, configuration);
        }

        [HttpPost]
        [CustomAuthorizeFilter(RoleLevel.Level_2, RoleLevel.Level_3)]
        public async Task<ActionResult<int>> Create(RegisterMemberCommand command)
        {
            var createdId = await Mediator.Send(command);
            var message = GetActivityLogMessage(RequestTypeEnum.New, createdId);
            await _azureStorageHelpers.SaveLogToBlob(message);
            return createdId;
        }

        [HttpGet]
        public async Task<ActionResult<MemberInfoDto>> GetCardsWithMemberNo(string memberNo)
        {
            var memberInfo = await Mediator.Send(new GetMemberInfoByMemberNoQuery() { MemberNo = memberNo });
            return memberInfo;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<bool>> ImportZipCode()
        {
            var name = User.Identity?.Name;
            var zipcodeCsv = Path.Combine(_env.WebRootPath, folderZipcode);
            var importResult = await Mediator.Send(new ImportZipcodeQuery() { ZipcodeDirectory = zipcodeCsv, Name = name });
            await _azureStorageHelpers.SaveLogToBlob(LoggingMessage.ImportZipcode);
            return importResult;
        }

        [HttpGet("[action]")]
        [CustomAuthorizeFilter(RoleLevel.Level_2, RoleLevel.Level_3)]
        public async Task<ActionResult<CardValidationDto>> ValidateCardForRegister(string memberNo)
        {
            var validationResult = await Mediator.Send(new ValidateCardForRegisterQuery() { MemberNo = memberNo });
            string resultMess = validationResult.IsValidated ? "" : string.Format(" ({0})", validationResult.Message);
            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.ValidateCardForRegister + resultMess, memberNo));
            return validationResult;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<ZipcodeDto>> GetInfoZipcode(string zipcode)
        {
            var zipCodeInfo = await Mediator.Send(new GetListZipcodeQuery() { Zipcode = zipcode });
            return zipCodeInfo;
        }

        [HttpPut("[action]")]
        [CustomAuthorizeFilter(RoleLevel.Level_2, RoleLevel.Level_3)]
        public async Task<ActionResult<int>> UpdateMember(UpdateMemberCommand command)
        {
            var memberId = await Mediator.Send(command);
            var message = GetActivityLogMessage(command.UpdateType, memberId);
            await _azureStorageHelpers.SaveLogToBlob(message);
            return memberId;
        }

        [HttpPut("[action]")]
        [CustomAuthorizeFilter(RoleLevel.Level_2, RoleLevel.Level_3)]
        public async Task<ActionResult<int>> UpdateMemberNo(UpdateMemberNoCommand command)
        {
            if (command.NewMemberNo.Equals(command.OldMemberNo)) return BadRequest("New MemberNo must not equal Old MemberNo ");
            var memberId = await Mediator.Send(command);
            var message = GetActivityLogMessage(command.UpdateType, memberId);
            await _azureStorageHelpers.SaveLogToBlob(message);
            return memberId;
        }

        [HttpPut("[action]")]
        [CustomAuthorizeFilter(RoleLevel.Level_2, RoleLevel.Level_3)]
        public async Task<ActionResult<int>> UpdateRemarkPIC(UpdateRemarkPICQuery command)
        {
            var updatedId = await Mediator.Send(command);
            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.UpdateRemarkAndPIC, updatedId));
            return updatedId;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<List<PersonInchargeDto>>> GetListPersonIncharge(GetListPersonInchargeQuery command)
        {
            var listPIC = await Mediator.Send(command);
            return listPIC;
        }

        [HttpPut("[action]")]
        [CustomAuthorizeFilter(RoleLevel.Level_2, RoleLevel.Level_3)]
        public async Task<int> RequestLeaveGroup(LeaveGroupRequestCommand command)
        {
            var memberId = await Mediator.Send(command);
            var message = GetActivityLogMessage(RequestTypeEnum.LeaveGroup, memberId);
            await _azureStorageHelpers.SaveLogToBlob(message);
            return memberId;
        }

        [HttpPut("[action]")]
        [CustomAuthorizeFilter(RoleLevel.Level_2, RoleLevel.Level_3)]
        public async Task<int> RegisterKidsClub(RegisterKidsClubCommand command)
        {
            var memberId = await Mediator.Send(command);
            var message = GetActivityLogMessage(RequestTypeEnum.Kid, memberId);
            await _azureStorageHelpers.SaveLogToBlob(message);
            return memberId;
        }

        [HttpGet("[action]")]
        [CustomAuthorizeFilter(RoleLevel.Level_2, RoleLevel.Level_3)]
        public async Task<PaginatedList<InquiryKidClubDto>> SearchInquiryKidClubWithPagination([FromQuery] SearchInquiryKidClubWithPaginationQuery query)
        {
            var inquiryKidClubs = await Mediator.Send(query);
            await _azureStorageHelpers.SaveLogToBlob(LoggingMessage.SearchInquiryKidClub);
            return inquiryKidClubs;
        }

        [HttpGet("[action]")]
        public async Task<InquiryKidClubDetailDto> GetInquiryKidClubDetail([FromQuery] GetInquiryKidClubDetailCommand command)
        {
            var inquiryKidClubDetail = await Mediator.Send(command);
            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.GetInquiryKidClub, command.Id));
            return inquiryKidClubDetail;
        }

        /// <summary>
        /// Get activity log message from update type
        /// </summary>
        /// <param name="updateType"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private string GetActivityLogMessage(RequestTypeEnum updateType, int id)
        {
            var message = "";
            switch (updateType)
            {
                case RequestTypeEnum.New:
                    message = LoggingMessage.MemberAddNew;
                    break;
                case RequestTypeEnum.Switch:
                    message = LoggingMessage.MemberSwitchCard;
                    break;
                case RequestTypeEnum.ReIssued:
                    message = LoggingMessage.MemberReIssued;
                    break;
                case RequestTypeEnum.ChangeCard:
                    message = LoggingMessage.MemberChangeInfo;
                    break;
                case RequestTypeEnum.LeaveGroup:
                    message = LoggingMessage.MemberLeaveGroup;
                    break;
                case RequestTypeEnum.PMigrate:
                    message = LoggingMessage.MemberPointMigrate;
                    break;
                case RequestTypeEnum.Kid:
                    message = LoggingMessage.MemberRegisterKidClub;
                    break;
            }

            return string.Format(message, id);
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<List<PersonInchargeDto>>> GetListPersonInchargeCMS([FromQuery] GetListPersonInchargeCMSQuery query)
        {
            var listPIC = await Mediator.Send(query);
            return listPIC;
        }
    }
}
