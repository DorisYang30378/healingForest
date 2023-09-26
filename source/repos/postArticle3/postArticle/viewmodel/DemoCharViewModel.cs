using postArticle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace postArticle.viewmodel
{
    public class DemoCharViewModel
    {
        //有哪些聊天室
        public IEnumerable<Chatroom> ChatRoom { get; set; }
        //顯示當前聊天紀錄
        public IEnumerable<ChatroomLog> ChatRoomLog { get; set; }

        public string ChatRoomName { get; set; }

        //未使用
        public string MyUser { get; set; }
        public string OtherUser { get; set; }
        //
        //訊息
        public string Log { get; set; }
        //目標ID
        public int OtherUserId { get; set; }
        //自己的id
        public int myUserId { get; set; }
    }
}