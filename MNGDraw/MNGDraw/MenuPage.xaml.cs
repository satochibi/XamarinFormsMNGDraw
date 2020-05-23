using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MNGDraw
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {

        private Manga aManga;

        public MenuPage()
        {
            InitializeComponent();
        }

        public void SetMangaOfMainPage(Manga aManga)
        {
            this.aManga = aManga;
        }


        private void ReturnButtonClicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void SettingButtonClicked(object sender, EventArgs e)
        {
            var settingPage = new SettingPage();
            Navigation.PushAsync(settingPage);
            settingPage.SetMangaOfMainPage(this.aManga);
        }
    }
}