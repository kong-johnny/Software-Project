using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Xiaoya.Classroom.Models
{
    public class Room
    {
        public string Name { get; set; }
        private List<bool> _hasLecture = new List<bool>(12);
        public List<bool> HasLecture { get => _hasLecture; }

        private readonly Color RED = Color.FromArgb(255, 255, 180, 180);
        private readonly Color RED2 = Color.FromArgb(255, 255, 200, 200);

        private readonly Color GREEN = Color.FromArgb(255, 180, 255, 180);
        private readonly Color GREEN2 = Color.FromArgb(255, 200, 255, 200);

        private List<Brush> _colors = new List<Brush>();
        public List<Brush> Colors { get => _colors; }

        public Room(string name)
        {
            Name = name;
        }

        public void UpdateColors()
        {
            foreach (var has in _hasLecture)
            {
                var brush = new LinearGradientBrush()
                {
                    StartPoint = new Windows.Foundation.Point(0, 1),
                    EndPoint = new Windows.Foundation.Point(0, 0)
                };
                brush.GradientStops.Add(new GradientStop()
                {
                    Color = has ? RED : GREEN,
                    Offset = 0
                });
                brush.GradientStops.Add(new GradientStop()
                {
                    Color = has ? RED2 : GREEN2,
                    Offset = 1
                });

                _colors.Add(brush);
            }
        }
    }
}
