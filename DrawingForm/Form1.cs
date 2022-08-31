using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DrawingForm
{
    public partial class Form1 : Form
    {
        List<Reg> access = new List<Reg>();
        const string _conStr = "Server=server_name;Database=DrawingForm;Integrated Security=True;";
        public Form1()
        {
            InitializeComponent();


        }

        public void drawAll()
        {
            int x = 0;

            for (int i = 0; i < 7; i++)
            {
                int y = 0;

                for (int j = 0; j < 23; j++)
                {
                    drawOne(x, y,getAccess(i,j,0));

                    y += 20;
                }

                x += 40;
            }
        }

        public void drawOne(int x, int y, Boolean isAllowed)
        {
            Color cl = Color.White;

            if (isAllowed) cl = Color.Black;

            System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(cl);
            System.Drawing.Graphics formGraphics;
            formGraphics = this.CreateGraphics();
            formGraphics.FillRectangle(myBrush, new Rectangle(x, y, 40, 20));
            myBrush.Dispose();
            formGraphics.Dispose();
        }

        public Boolean getAccess(int day, int hour, int category)
        {
            int x = access.Where(x => x.day == day && x.hour == hour && x.category == category).Count();
            return x>0;
        }

        public Point getWH(int x, int y)
        {

            Point p = new Point { X = x / 40, Y = y/20};


            return p;
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            Point p = getWH(e.X, e.Y);

            if(p.X<7 && p.Y<24)
            {
                if (access.Exists(x => x.day == p.X && x.hour == p.Y))
                {
                    access.RemoveAll(x => x.day == p.X && x.hour == p.Y);
                }
                else
                {
                    access.Add(new Reg { id = -1, day = p.X, hour = p.Y, category = 0 });
                }
                drawAll();
            }

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            drawAll();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string con = _conStr;
            SqlConnection connection = new SqlConnection(con);
            connection.Open();

            MessageBox.Show("ok!");

            string query = "select * from regs";

            var list = connection.Query<Reg>(query);

            foreach(Reg r in list)
            {
                Reg reg = new Reg { id = r.id, day = r.day, hour = r.hour };
                access.Add(reg);
            }

            drawAll();

            connection.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            string con = _conStr;
            SqlConnection connection = new SqlConnection(con);
            connection.Open();

            string query = "delete from regs";
            connection.Query<Reg>(query);

            foreach (Reg r in access)
            {
                query = "insert into regs (ID, DAY, HOUR) values (" + r.id + "," + r.day + "," + r.hour + ")";
                connection.Query<Reg>(query);
                
            }

            connection.Close();

        }
    }

        
    
}
