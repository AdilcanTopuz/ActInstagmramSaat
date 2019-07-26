using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Logger;
using System.IO;

namespace ActInstagramSaat
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }
        private static IInstaApi api;
        private static UserSessionData user;
        private string zaman = "";
        private Bitmap bmp;
        private async void btnBaslat_Click(object sender, EventArgs e)
        {

            try
            {
                if (string.IsNullOrEmpty(txtKullaniciAdi.Text))
                {
                    MessageBox.Show("Kullanıcı Adı Boş Olamaz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(txtParola.Text))
                {
                    MessageBox.Show("Parola Boş Olamaz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                user = new UserSessionData();
                user.UserName = txtKullaniciAdi.Text;
                user.Password = txtParola.Text;
                api = InstaApiBuilder.CreateBuilder().SetUser(user).UseLogger(new DebugLogger(LogLevel.Exceptions)).Build();

                var logInResult = await api.LoginAsync();
                if (logInResult.Succeeded)
                {
                    timer1.Start();
                }
                else
                {
                    if (logInResult.Value == InstaLoginResult.ChallengeRequired)
                    {
                        var challenge = await api.GetChallengeRequireVerifyMethodAsync();
                        if (challenge.Succeeded)
                        {
                            if (challenge.Value.SubmitPhoneRequired)
                            {
                                //telefon
                            }
                            else
                            {
                                if (challenge.Value.StepData != null)
                                {
                                    if (!string.IsNullOrEmpty(challenge.Value.StepData.PhoneNumber))
                                    {
                                        var pnv = challenge.Value.StepData.PhoneNumber;
                                        MessageBox.Show(pnv);
                                        sendCode(api);
                                    }
                                    if (!string.IsNullOrEmpty(challenge.Value.StepData.Email))
                                    {
                                        var emv = challenge.Value.StepData.Email;
                                        MessageBox.Show(emv);
                                        sendCode(api);
                                    }
                                    
                                }
                            }
                        }
                        else
                            MessageBox.Show(challenge.Info.Message, "ERR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        async void sendCode(IInstaApi api)
        {
            bool isEmail = true;
            try
            {                
                if (isEmail)
                {
                    var email = await api.RequestVerifyCodeToEmailForChallengeRequireAsync();
                    this.Size = new Size(429, 260);
                    if (email.Succeeded)
                    {
                        MessageBox.Show("gönderildi");
                    }
                    else
                        MessageBox.Show(email.Info.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    var phoneNumber = await api.RequestVerifyCodeToSMSForChallengeRequireAsync();
                    if (phoneNumber.Succeeded)
                    {
                        MessageBox.Show("doğrulandı");
                        this.Size = new Size(429, 200);
                    }
                    else
                        MessageBox.Show(phoneNumber.Info.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        public async void Dogrula(IInstaApi api)
        {
            try
            {
                var verifyLogin = await api.VerifyCodeForChallengeRequireAsync(txtDogrulama.Text);
                if (verifyLogin.Succeeded)
                {
                    MessageBox.Show("doğrulandı");
                    this.Size = new Size(429, 200);
                    timer1.Start();
                }
                else
                {
                    MessageBox.Show("Yanlış kod");
                    if (verifyLogin.Value == InstaLoginResult.TwoFactorRequired)
                    {
                        MessageBox.Show("iki faktörlü doğrulama gerekiyor.");
                    }
                    else
                        MessageBox.Show(verifyLogin.Info.Message, "ERR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"D");
            }
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (zaman != DateTime.Now.ToLongTimeString())
                {
                    zaman = DateTime.Now.ToLongTimeString();
                    string dosyaYolu = "resim.png";
                    bmp = new Bitmap(dosyaYolu);
                    Graphics g = Graphics.FromImage(bmp);
                    g.DrawString("@18adilcan", new Font("Century Gothic", Convert.ToInt32(60), FontStyle.Bold), new SolidBrush(Color.White), 220f, 650f);
                    g.DrawString(DateTime.Now.ToLongTimeString(), new Font("Times New Roman", Convert.ToInt32(180), FontStyle.Bold), new SolidBrush(Color.White), 40f, 300f);
                    pbPP.Image = bmp;
                    bmp.Save("resimPP.png");
                    string ppYolu = "resimPP.png";
                    byte[] Bytes = File.ReadAllBytes(ppYolu);
                    await api.AccountProcessor.ChangeProfilePictureAsync(Bytes);
                }
            }
            catch (Exception ex)
            {
                this.Text = "Hata: " + ex.Message;
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://facebook.com/18adilcan");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://instagram.com/18adilcan");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://adilcantopuz.com.tr");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Dogrula(api);
        }
    }
}
