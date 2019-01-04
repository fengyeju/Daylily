﻿using Daylily.Bot;
using Daylily.Bot.Message;
using Daylily.Bot.Session;
using Daylily.Common.Utils.LoggerUtils;
using Daylily.Common.Utils.StringUtils;
using Daylily.CoolQ.Message;
using Daylily.Osu.Database.BLL;
using Daylily.Osu.Database.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Daylily.CoolQ;

namespace Daylily.Plugin.Osu.Cabbage
{
    internal static class CabbageCommon
    {
        public static readonly ConcurrentQueue<CoolQRouteMessage> MessageQueue = new ConcurrentQueue<CoolQRouteMessage>();
        public static Task TaskQuery;

        public static void Query()
        {
            while (MessageQueue.Count != 0)
            {
                if (!MessageQueue.TryDequeue(out var routeMsg))
                    continue;
                var cmd = routeMsg.Command;

                const long cabbageId = 1020640876;
                string uname;
                if (cmd == "statme" || cmd == "bpme" || cmd == "mybp" || cmd == "costme" || cmd == "mycost")
                {
                    BllUserRole bllUserRole = new BllUserRole();
                    List<TblUserRole> userInfo = bllUserRole.GetUserRoleByQq(long.Parse(routeMsg.UserId));
                    if (userInfo.Count == 0)
                        DaylilyCore.Current.Dispatcher.SendMessage(routeMsg.ToSource(DefaultReply.IdNotBound, true));

                    uname = userInfo[0].CurrentUname;
                }
                else
                    uname = routeMsg.ArgString;

                using (Session session = new Session(25000, new CqIdentity(cabbageId, MessageType.Private), cabbageId))
                {
                    DaylilyCore.Current.Dispatcher.SendMessage(
                        new CoolQRouteMessage($"!{cmd.Replace("my", "").Replace("me", "")} {uname}",
                            new CqIdentity(cabbageId, MessageType.Private)));
                    try
                    {
                        CoolQRouteMessage result = (CoolQRouteMessage)session.GetMessage();
                        session.Timeout = 600;
                        CoolQRouteMessage result2 = null;
                        try
                        {
                            result2 = (CoolQRouteMessage)session.GetMessage();
                        }
                        catch
                        {
                            // ignored
                        }

                        ImageInfo[] imgList =
                            CoolQCode.GetImageInfo(result.RawMessage) ?? CoolQCode.GetImageInfo(result2?.RawMessage);

                        if (imgList == null)
                        {
                            DaylilyCore.Current.Dispatcher.SendMessage(routeMsg.ToSource(result.RawMessage));
                            if (result2 != null)
                                DaylilyCore.Current.Dispatcher.SendMessage(routeMsg.ToSource(result2.RawMessage));
                            continue;
                        }
                        //throw new IndexOutOfRangeException("查询失败：" + result.Message);
                        var message = CoolQCode.DecodeToString(result.RawMessage);
                        foreach (var item in imgList)
                        {
                            var str = new FileImage(new Uri(item.Url));
                            StringFinder sf = new StringFinder(message);
                            sf.FindNext("[图片]");
                            string str1 = sf.Cut();
                            if (sf.FindNext("[图片]", false) > message.Length - 1)
                            {
                                message = str1 + str;
                                continue;
                            }

                            sf.FindToLast();
                            string str2 = sf.Cut();
                            message = str1 + str + str2;
                        }

                        DaylilyCore.Current.Dispatcher.SendMessage(
                            routeMsg.ToSource(message + "\r\n（查询由白菜支持）"));
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        string msg = e.Message;
                        DaylilyCore.Current.Dispatcher.SendMessage(routeMsg.ToSource(msg, true));
                    }
                    catch (TimeoutException)
                    {
                        string msg = "查询失败，白菜没有搭理人家..";
                        DaylilyCore.Current.Dispatcher.SendMessage(routeMsg.ToSource(msg, true));
                    }
                    catch (Exception ex)
                    {
                        string msg = "查询失败，未知错误。";
                        Logger.Exception(ex);
                        DaylilyCore.Current.Dispatcher.SendMessage(routeMsg.ToSource(msg, true));
                    } // catch
                } // using
            } // while
        } //void
    }
}
