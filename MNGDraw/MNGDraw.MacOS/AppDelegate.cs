using AppKit;
using Foundation;
using Xamarin.Forms;
using Xamarin.Forms.Platform.MacOS;

namespace MNGDraw.MacOS
{
    [Register("AppDelegate")]
    public class AppDelegate : FormsApplicationDelegate
    {
        NSWindow _window;

        public AppDelegate()
        {
            var style = NSWindowStyle.Closable | NSWindowStyle.Resizable | NSWindowStyle.Titled;

            var rect = new CoreGraphics.CGRect(200, 1000, 1024, 768);
            _window = new NSWindow(rect, style, NSBackingStore.Buffered, false);
            _window.Title = "MNGDraw";
            _window.TitleVisibility = NSWindowTitleVisibility.Visible;
        }

        public override NSWindow MainWindow
        {
            get { return _window; }
        }

        public override void DidFinishLaunching(NSNotification notification)
        {
            // Insert code here to initialize your application
            NSApplication.SharedApplication.MainMenu = MakeMainMenu();
            Forms.Init();
            LoadApplication(new App());
            base.DidFinishLaunching(notification);
        }

        public override void WillTerminate(NSNotification notification)
        {
            // Insert code here to tear down your application
        }

        //https://forums.xamarin.com/discussion/162572/how-to-add-quit-menu-item-to-macos-xamarin-forms-application
        private NSMenu MakeMainMenu()
        {
            // top bar app menu
            NSMenu menubar = new NSMenu();
            NSMenuItem appMenuItem = new NSMenuItem();
            menubar.AddItem(appMenuItem);

            NSMenu appMenu = new NSMenu();
            appMenuItem.Submenu = appMenu;

            // add separator
            NSMenuItem separator = NSMenuItem.SeparatorItem;
            appMenu.AddItem(separator);

            // add quit menu item
            string quitTitle = string.Format("Quit {0}", "UgomemoPro");
            var quitMenuItem = new NSMenuItem(quitTitle, "q", delegate
            {
                NSApplication.SharedApplication.Terminate(menubar);
            });
            appMenu.AddItem(quitMenuItem);

            return menubar;
        }
    }
}
