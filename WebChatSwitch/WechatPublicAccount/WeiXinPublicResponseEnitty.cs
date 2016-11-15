using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeChatPublicAccount
{

    public class GeneralResponse
    {
        public int errcode { get; set; }

        public string errmsg { get; set; }
    }

    #region Profile
    public class AccessTokenResponse : GeneralResponse
    {
        public string access_token { get; set; }
        public string expires_in { get; set; }
    }

    public class JsTicketResponse : GeneralResponse
    {
        public string ticket { get; set; }
        public string expires_in { get; set; }
    }

    public class JsInitResponse
    {
        public string appId { get; set; }
        public string timestamp { get; set; }
        public string nonceStr { get; set; }
        public string signature { get; set; }
    }

    #endregion

    #region Groups
    public class CreateGroupResponse : GeneralResponse
    {
        public Group group { get; set; }
    }

    public class GetGroupResponse : GeneralResponse
    {
        public List<Group> groups { get; set; }
    }

    public class GetGroupIdResponse : GeneralResponse
    {
        public int groupid { get; set; }
    }

    public class Group
    {
        public int id { get; set; }
        public string name { get; set; }
        public int count { get; set; }
    }
    #endregion

    #region MessageTemplate

    public class SendMessageTemplateResponse : GeneralResponse
    {
        public string msgid { get; set; }
    }

    #endregion

    #region Menu

    public class MenuConditionalResponse : GeneralResponse
    {
        public int menuid { get; set; }
    }

    #endregion

    #region User
    public class UserResponse : GeneralResponse
    {
        public int subscribe { get; set; }
        public string openid { get; set; }
        public string nickname { get; set; }
        public int sex { get; set; }
        public string language { get; set; }
        public string city { get; set; }
        public string province { get; set; }
        public string country { get; set; }
        public string headimgurl { get; set; }
        public long subscribe_time { get; set; }
        public string unionid { get; set; }
        public string remark { get; set; }
        public int groupid { get; set; }
    }

    #endregion

    #region Async Task

    public class AsyncResponse : GeneralResponse
    {
        public string jobid { get; set; }
    }


    public class AsyncInviteUserItem
    {
        public string userid { get; set; }
        public string errcode { get; set; }
        public string errmsg { get; set; }
        public string invitetype { get; set; }
    }

    public class AsyncSyncUserItem
    {
        public string action { get; set; }
        public string userid { get; set; }
        public string errcode { get; set; }
        public string errmsg { get; set; }
    }

    public class AsyncSyncDepartmentItem
    {
        public string action { get; set; }
        public string partyid { get; set; }
        public string errcode { get; set; }
        public string errmsg { get; set; }

    }

    public class AsyncJobGeneralResponse : GeneralResponse
    {
        public int status { get; set; }
        public string type { get; set; }
        public int total { get; set; }
        public int percentage { get; set; }
        public int remaintime { get; set; }


    }

    public class AsyncJobInviteUserResponse
    {
        public string status { get; set; }
        public string type { get; set; }
        public string total { get; set; }
        public string percentage { get; set; }
        public string remaintime { get; set; }
        public List<AsyncSyncUserItem> result { get; set; }

    }

    public class AsyncJobSyncUserResponse
    {
        public string status { get; set; }
        public string type { get; set; }
        public string total { get; set; }
        public string percentage { get; set; }
        public string remaintime { get; set; }
        public List<AsyncSyncUserItem> result { get; set; }

    }

    public class AsyncJobSyncDepartmentResponse
    {
        public string status { get; set; }
        public string type { get; set; }
        public string total { get; set; }
        public string percentage { get; set; }
        public string remaintime { get; set; }
        public List<AsyncSyncUserItem> result { get; set; }

    }

    #endregion

    #region Material
    public class MediaUploadResponse
    {
        public string type { get; set; }
        public string media_id { get; set; }
        public string created_at { get; set; }
    }
    #endregion

    #region App
    public class AppAllowUserItem
    {
        public string userid { get; set; }

        public string status { get; set; }
    }

    public class AppAllowTagItem
    {
        public List<int> tagid { get; set; }
    }

    public class AppAllowDepartmentItem
    {
        public List<int> partyid { get; set; }
    }

    public class AppStatusResponse : GeneralResponse
    {

        public int agentid { get; set; }
        public string name { get; set; }
        public string square_logo_url { get; set; }
        public string round_logo_url { get; set; }
        public string description { get; set; }

        //public List<AppAllowUserItem> allow_userinfos { get; set; }

        public AppAllowDepartmentItem allow_partys { get; set; }

        public AppAllowTagItem allow_tags { get; set; }

        public string close { get; set; }
        public string redirect_domain { get; set; }
        public string report_location_flag { get; set; }
        public string isreportuser { get; set; }
        public string isreportenter { get; set; }
    }

    public class AppItem
    {
        public string agentid { get; set; }
        public string name { get; set; }
        public string square_logo_url { get; set; }
        public string round_logo_url { get; set; }
    }

    public class AppListResponse : GeneralResponse
    {
        public List<AppItem> agentlist { get; set; }
    }

    #endregion

    #region Message

    public class MessageSendingResponse : GeneralResponse
    {
        public string invaliduser { get; set; }
        public string invalidparty { get; set; }
        public string invalidtag { get; set; }

    }

    #endregion
}
