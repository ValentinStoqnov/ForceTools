using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ForceTools.Models
{
    public class Databases : INotifyPropertyChanged
    {
        private string _dbName;
        private string _dbId;
        private BitmapImage _BitImage;

        public event PropertyChangedEventHandler PropertyChanged;

        public string dbName
        {
            get => _dbName;
            set
            {
                _dbName = value;

            }
        }
        public string dbId
        {
            get => _dbId;
            set
            {
                _dbId = value;

            }

        }

        public BitmapImage BitImage
        {
            get => _BitImage;
            set
            {
                _BitImage = value;
                OnPropertyChanged();
            }

        }

        public Databases(string DBName, string DBId, BitmapImage BI)
        {
            dbName = DBName;
            dbId = DBId;
            BitImage = BI;
        }

        public Databases()
        {
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
