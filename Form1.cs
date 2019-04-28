using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Net;

namespace _2bBasedApp
{
    public partial class Home : Form
    {
        Context context = new Context();
        public Home()
        {
            InitializeComponent();
        }
        private HttpClient api;
        User loggedInUser;
  
        private void Form1_Load(object sender, EventArgs e)
        {
            api = new HttpClient
            {
                BaseAddress = new Uri("https://db.ygoprodeck.com/api/")
            };
            panel1.BackColor = Color.FromArgb(51, 51, 51);
            panel2.BackColor = Color.FromArgb(51, 51, 51);
            panel3.BackColor = Color.FromArgb(51, 51, 51);
            panel4.BackColor = Color.FromArgb(18, 18, 18);
            panel6.BackColor = Color.FromArgb(18, 18, 18);
            panel7.BackColor = Color.FromArgb(18, 18, 18);
            panel5.BackColor = Color.FromArgb(28, 28, 28);
            nextcardauction();
            //viewUpComingAuctionCards();
            loadSubMenu();

            int x = 286, y = 164, z = 1;
            int x1 = 286, y1 = 579;
            foreach(var i in context.CardTypes)
            {
                cardTypeComboBox.Items.Add(i.Name);
            }

            //reccomended cards generator
            for (int i = 0; i < 12; i++)
            {
                PictureBox pb = new PictureBox()
                {
                    //Name = sale.Card.Name + sale.Card.Rarity,
                    Location = new Point(x, y),
                    Height = 150,
                    Width = 101,
                    Image = Image.FromFile("D:\\Pictures\\1109501.jpg"),
                };
                pb.SizeMode = PictureBoxSizeMode.StretchImage;
                x += 150;
                if (i == 5)
                {
                    y += 168;
                    x = 286;
                }
                panel6.Controls.Add(pb);
            }

            //following cards generator
            for (int i = 0; i < 12; i++)
            {
                PictureBox pb1 = new PictureBox()
                {
                    Tag = z,
                    Location = new Point(x1, y1),
                    Height = 150,
                    Width = 101,
                    Image = Image.FromFile("D:\\Pictures\\1109501.jpg"),
                };
                pb1.SizeMode = PictureBoxSizeMode.StretchImage;
                z += 1;
                x1 += 150;
                if (i == 5)
                {
                    y1 += 168;
                    x1 = 286;
                }
                panel6.Controls.Add(pb1);
            }
        }

        

        private void loadSubMenu()
        {
            int x = 4, y = 6;
            foreach(SubMenu sm in context.SubMenus)
            {
                Button bt = new Button()
                {
                    Name = sm.Name,
                    Text = sm.Name,
                    Height = 77,
                    Width = 279,
                    Location = new Point(x, y),
                };
                bt.Font = new Font("Microsoft Sans Serif", 13, FontStyle.Regular);
                bt.FlatStyle = FlatStyle.Flat;
                bt.FlatAppearance.BorderSize = 0;
                bt.MouseClick += new MouseEventHandler(formjump);
                if(bt.Name == this.Name)
                {
                    bt.ForeColor = Color.FromArgb(255, 0, 0);
                    bt.BackColor = Color.FromArgb(51, 51, 51);
                }
                else
                {
                    bt.ForeColor = Color.FromArgb(255, 255, 255);
                    bt.BackColor = Color.FromArgb(28, 28, 28);
                }
                panel5.Controls.Add(bt);
                y += 96;
            }
        }

        private void formjump(object sender, MouseEventArgs e)
        {
            string menuName = (sender as Button).Name;
            if (menuName == "Profile")
            {
                Profile profile = new Profile();
                this.Hide();
                profile.ShowDialog();
                this.Close();
            }
            //var form = Activator.CreateInstance(Type.GetType(string + menuName)) as Form;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if(panel5.Tag != "open")
            {
                panel5.Tag = "open";
                panel5.Visible = true;
                panel6.Location = new Point(157, 0);
                panel7.Location = new Point(1362, 0);
                panel8.Location = new Point(1368, 146);
            }
            else
            {
                panel5.Tag = "closed";
                panel5.Visible = false;
                panel6.Location = new Point(1, 0);
                panel7.Location = new Point(1185, 0);
                panel8.Location = new Point(1179, 146);
            }
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            loggedInUser = context.Users.Where(x => x.Username == usernameTextBox.Text & x.Password == passwordTextBox.Text | 
            x.Email == usernameTextBox.Text & x.Password == passwordTextBox.Text).FirstOrDefault();
            if(loggedInUser != null)
            {
                panel2.Visible = true;
                panel3.Visible = false;
                UserDP.Image = Image.FromFile(loggedInUser.DisplayPicture);
                Username.Text = $"Hi {loggedInUser.FirstName}!";
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(progressBar1.Value != 0)
            {
                progressBar1.Value -= 1;
            }
            else
            {
                cardsoldauction();
                nextcardauction();
            }
        }

        private void cardsoldauction()
        {
            Sale auctionSale = context.Sales.Where(x => x.ForAuction == true & x.Sold == false).FirstOrDefault();
            auctionSale.Sold = true;
            context.SaveChanges();

            auctionSale = context.Sales.Include("Card").Where(x => x.ForAuction == true & x.Sold == false).FirstOrDefault();
            Card auctionCard = auctionSale.Card;
            retailPrice.Text = "Average $" + auctionCard.PriceAverage.ToString();
            auctionPB.Image = Image.FromFile(auctionCard.Image);
            progressBar1.Value = 600;
            button3.Enabled = true;
        }

        private void nextcardauction()
        {
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            //viewUpComingAuctionCards();

            if (panel8.Tag != "open")
            {
                panel8.Visible = true;
                panel8.Tag = "open";
            }
            else
            {
                panel8.Visible = false;
                panel8.Tag = "";
            }
        }

        //private void viewUpComingAuctionCards()
        //{
        //    int x = 3, y = 3;
        //    foreach(Control c in panel8.Controls)
        //    {
        //        panel8.Controls.Remove(c);
        //    }
        //    foreach (Sale sale in context.Sales.Include("Card").Where(a => a.ForAuction == true & a.Sold == false))
        //    {
        //        PictureBox newpb = new PictureBox()
        //        {
        //            Image = Image.FromFile(sale.Card.Image),
        //            Name = sale.Card.Name + sale.Card.Rarity,
        //            Width = 61,
        //            Height = 90,
        //            Location = new Point(x, y),
        //        };
        //        x += 67;
        //        if (x > 420)
        //        {
        //            x = 3;
        //            y += 96;
        //        }
        //        if (x > 420 & y > 400)
        //        {
        //            break;
        //        }
        //        newpb.SizeMode = PictureBoxSizeMode.StretchImage;
        //        panel8.Controls.Add(newpb);
        //    }
        //}

        async Task<List<apiCard>> awaitmethod()
        {
            CardArray returnval = new CardArray()
            {
                card = null
            };


            HttpResponseMessage response = await api.GetAsync($"v4/cardinfo.php?"); //name={textBox1.Text}
            if (response.IsSuccessStatusCode)
            {
                var rsp = await response.Content.ReadAsStringAsync();

                rsp = rsp.Substring(1, rsp.Length - 2);
                List<apiCard> li = JArray.Parse(rsp).ToObject<List<apiCard>>();
                return li;
            }
            return null;
        }

        async Task<List<apiCard>> awaitmethod2()
        {
            CardArray returnval = new CardArray()
            {
                card = null
            };


            HttpResponseMessage response = await api.GetAsync($"v4/cardinfo.php?name={textBox1.Text}");
            if (response.IsSuccessStatusCode)
            {
                var rsp = await response.Content.ReadAsStringAsync();

                rsp = rsp.Substring(1, rsp.Length - 2);
                List<apiCard> li = JArray.Parse(rsp).ToObject<List<apiCard>>();
                return li;
            }
            return null;
        }

        async void searchButton_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            if (textBox1.Text == "")
            {
                List<apiCard> test = await awaitmethod();
                if (test != null)
                {
                    foreach (var i in test)
                    {
                        listBox1.Items.Add(i.name);
                    }
                }
            }
            else
            {
                List<apiCard> test = await awaitmethod2();
                if (test != null)
                {
                    foreach (var i in test)
                    {
                        using (WebClient client = new WebClient())
                        {
                            client.DownloadFile(new Uri(i.image_url), $@"D:\Pictures\yugioh\{i.name}.png");
                        }
                        listBox1.Items.Add("Name: " + i.name);
                        listBox1.Items.Add("Level: " + i.level);
                        listBox1.Items.Add("Archtype: " + i.archetype);
                        listBox1.Items.Add("Attribute: " + i.attribute);
                        listBox1.Items.Add("Attack: " + i.atk);
                        listBox1.Items.Add("Defense: " + i.def);
                        auctionPB.Image = Image.FromFile($"D:\\Pictures\\yugioh\\{i.name}.png");
                        auctionPB.SizeMode = PictureBoxSizeMode.StretchImage;
                        //This break will stop after getting alll the info above from the FIRST card searched (if its an 'fname' fuzzy search). Comment this out plus all unnecessary unfo to get all similar name cards etc.
                        break;
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(loggedInUser != null)
            {
                Bid newbid = new Bid()
                {
                    TimeStamps = DateTime.Now,
                    BidAmount = Convert.ToDecimal(textBox3.Text),
                    Bidder = loggedInUser,
                    Item = context.Sales.Where(x => x.ForAuction == true & x.Sold == false).FirstOrDefault(),
                };
                context.Bids.Add(newbid);
                context.SaveChanges();
                button3.Enabled = false;
            }
        }

        private void uploadButton_Click(object sender, EventArgs e)
        {
            UploadCard uploadform = new UploadCard();
            uploadform.loggedInUser = loggedInUser;
            uploadform.ShowDialog();
        }
    }
}
