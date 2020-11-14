using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Week08_Gyartosor
{
    public partial class Form1 : Form
    {
        private List<Abstractions.Toy> _toys = new List<Abstractions.Toy>();

        private Entities.CarFactory _IToyFactory;
        public Entities.CarFactory IToyFactory
        {
            get { return _IToyFactory; }
            set { _IToyFactory = value; }
        }

        public Form1()
        {
            InitializeComponent();
            IToyFactory = new Entities.CarFactory();
        }

        private void CreateTimer_Tick(object sender, EventArgs e)
        {
            var toy = IToyFactory.CreateNew();
            _toys.Add(toy);
            toy.Left = -toy.Width;
            mainPanel.Controls.Add(toy);
        }

        private void ConveyorTimer_Tick(object sender, EventArgs e)
        {
            var maxPosition = 0;
            foreach (var toy in _toys)
            {
                toy.MoveToy();
                if (toy.Left > maxPosition)
                    maxPosition = toy.Left;
            }

            if (maxPosition > 1000)
            {
                var oldestToy = _toys[0];
                mainPanel.Controls.Remove(oldestToy);
                _toys.Remove(oldestToy);
            }
        }
    }
}
