using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MNGDraw
{
    public static class SaveProperties
    {
        private static string PropertyKeyScreentoneScale = "ScreentoneScale";
        private static string PropertyKeyBezierHandleThreshold = "BezierHandleThreshold";
        private static string PropertyKeyPathPreviewAlgorithms = "PathPreviewAlgorithms";


        private static double defaultScreentoneScale = 1;
        private static double defaultBezierHandleThreshold = 0;
        private static int defaultPathPreviewAlgorithms = 0;

        public static void Clear()
        {
            Application.Current.Properties.Clear();
        }

        public static double ScreentoneScale
        {
            get
            {
                if (Application.Current.Properties.ContainsKey(PropertyKeyScreentoneScale))
                {
                    return (double)Application.Current.Properties[PropertyKeyScreentoneScale];
                }
                else
                {
                    return SaveProperties.defaultScreentoneScale;
                }
            }
            set
            {
                Application.Current.Properties[PropertyKeyScreentoneScale] = value;
                Application.Current.SavePropertiesAsync();
            }
        }

        public static double BezierHandleThreshold
        {
            get
            {
                if (Application.Current.Properties.ContainsKey(PropertyKeyBezierHandleThreshold))
                {
                    return (double)Application.Current.Properties[PropertyKeyBezierHandleThreshold];
                }
                else
                {
                    return SaveProperties.defaultBezierHandleThreshold;
                }
            }
            set
            {
                Application.Current.Properties[PropertyKeyBezierHandleThreshold] = value;
                Application.Current.SavePropertiesAsync();
            }
        }

        public static int PathPreviewAlgorithms
        {
            get
            {
                if (Application.Current.Properties.ContainsKey(PropertyKeyPathPreviewAlgorithms))
                {
                    return (int)Application.Current.Properties[PropertyKeyPathPreviewAlgorithms];
                }
                else
                {
                    return SaveProperties.defaultPathPreviewAlgorithms;
                }
            }
            set
            {
                Application.Current.Properties[PropertyKeyPathPreviewAlgorithms] = value;
                Application.Current.SavePropertiesAsync();
            }
        }
    }
}
