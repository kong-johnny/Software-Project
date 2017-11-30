using CXHttpNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xiaoya.Classroom.Models;

namespace Xiaoya.Classroom
{
    public class ClassroomClient
    {
        public static async Task<List<Building>> GetBuildings()
        {
            var buildings = new List<Building>();

            try
            {
                var res = await CXHttp.Connect("http://123.206.51.151:1221/buildings").UseProxy(false).Get();
                var body = await res.Content();
                body = body.Trim();

                if (body == "error" || body == "")
                    return buildings;

                string[] roomInfo = body.Split(',');

                for (int i = 0; i < roomInfo.Length; i += 2)
                {
                    buildings.Add(new Building(roomInfo[i], roomInfo[i + 1]));
                }
            }
            catch
            {
                buildings.Add(new Building(null, "获取失败"));
            }

            return buildings;
        }

        public static async Task<List<Room>> GetRooms(string buildingId)
        {
            var room = new List<Room>();

            try
            {
                var res = await CXHttp.Connect("http://123.206.51.151:1221/building/" + buildingId).UseProxy(false).Get();
                var body = await res.Content();
                body = body.Trim();

                if (body == "error")
                    return room;

                string[] roomInfoArr = body.Split(';');

                foreach (var info in roomInfoArr)
                {
                    if (info.Trim() == "") continue;
                    string[] roomInfo = info.Trim().Split(',');
                    Room item = new Room(roomInfo[0]);
                    for (int i = 5; i < 17; ++i)
                    {
                        item.HasLecture.Add(roomInfo[i].Trim() == "0");
                    }
                    item.UpdateColors();
                    room.Add(item);
                }
            }
            catch
            {
                room.Add(new Room("获取失败"));
            }

            return room.OrderBy(o => o.Name).ToList();
        }
    }
}
