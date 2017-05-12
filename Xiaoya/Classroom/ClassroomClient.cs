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

            var res = await CXHttp.Connect("http://202.112.88.59:8082/buildings").Get();
            var body = await res.Content();
            body = body.Trim();

            if (body == "error")
                return buildings;

            string[] roomInfo = body.Split(',');

            for (int i = 0; i < roomInfo.Length; i += 2)
            {
                buildings.Add(new Building(roomInfo[i], roomInfo[i + 1]));
            }

            return buildings;
        }

        public static async Task<List<Room>> GetRoom(string buildingId)
        {
            var room = new List<Room>();

            var res = await CXHttp.Connect("http://202.112.88.59:8082/building/" + buildingId).Get();
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

            return room.OrderBy(o => o.Name).ToList();
        }
    }
}
