﻿using System.Threading.Tasks;
using Daylily.Bot.Backend;
using Daylily.Bot.Message;
using Daylily.CoolQ.Message;

namespace Daylily.Plugin.Osu.Cabbage
{
    [Name("Bp查询（白菜）")]
    [Author("yf_extension")]
    [Version(0, 0, 1, PluginVersion.Alpha)]
    [Help("详情问白菜。")]
    [Command("bpme", "bp")]
    public class Bp : CommandPlugin
    {


        public override void OnInitialized(string[] args)
        {
        }

        public override CoolQRouteMessage OnMessageReceived(CoolQRouteMessage routeMsg)
        {
            CabbageCommon.MessageQueue.Enqueue(routeMsg);

            bool isTaskFree = CabbageCommon.TaskQuery == null || CabbageCommon.TaskQuery.IsCanceled ||
                              CabbageCommon.TaskQuery.IsCompleted;
            if (isTaskFree)
                CabbageCommon.TaskQuery = Task.Run(() => CabbageCommon.Query());

            return null;
        }
    }
}
