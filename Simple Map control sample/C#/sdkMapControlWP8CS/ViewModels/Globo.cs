using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Phone.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;

namespace sdkMapControlWP8CS.ViewModels
{
    class Globo
    {
      private Image theBase;
      private Image theButton;
      private Image theLanguage;
      private Image theImage;
      private TextBlock theTitle;

        public Globo()
      {
          theBase.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Assets/pinOpen.png", UriKind.RelativeOrAbsolute));
      }
        public Image getBase() { return theBase; }
        public Image getButton() { return theButton; }
        public Image getLanguage() { return theLanguage; }
        public Image getImage() { return theImage; }
        public TextBlock getTitle() { return theTitle; }
        
    }
}
