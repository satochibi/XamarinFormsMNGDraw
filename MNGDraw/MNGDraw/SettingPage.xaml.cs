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
    public partial class SettingPage : ContentPage
    {

        private Manga aManga;


        public SettingPage()
        {
            InitializeComponent();
        }

        public void SetMangaOfMainPage(Manga aManga)
        {
            this.aManga = aManga;
        }


        private void ScreentoneSettingButtonClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ScreentoneSettingPage());
        }

        private void BezierHandleSettingButtonClicked(object sender, EventArgs e)
        {
            var bezierHandleSettingPage = new BezierHandleSettingPage();
            Navigation.PushAsync(bezierHandleSettingPage);
            bezierHandleSettingPage.SetMangaOfMainPage(this.aManga);
        }

        private void PathPreviewSettingButtonClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new PathPreviewSettingPage());
        }

        private void SaveClearButtonClicked(object sender, EventArgs e)
        {
            SaveProperties.Clear();
            DisplayAlert("セーブクリア", "デフォルトに戻しました", "OK");
        }

        private void ReturnButtonClicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}