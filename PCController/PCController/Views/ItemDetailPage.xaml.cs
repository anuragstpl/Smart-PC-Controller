using PCController.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace PCController.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}