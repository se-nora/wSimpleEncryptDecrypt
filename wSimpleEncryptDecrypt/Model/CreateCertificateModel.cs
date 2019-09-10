using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using wSimpleEncryptDecrypt.Annotations;

namespace wSimpleEncryptDecrypt.Model
{
    public class CreateCertificateModel : INotifyPropertyChanged
    {
        private string _subject;
        private int _keyLength = 2048;
        private TimeSpan _validDuration = TimeSpan.FromDays(365);
        private string _password;
        private int _passwordLength = 32;
        private DateTime? _expirationDate;
        private bool _isBusy;

        public string Subject
        {
            get => _subject;
            set
            {
                if (value == _subject) return;
                _subject = value;
                OnPropertyChanged();
            }
        }

        public int KeyLength
        {
            get => _keyLength;
            set
            {
                if (value == _keyLength) return;
                _keyLength = value;
                OnPropertyChanged();
            }
        }

        public double ValidDurationInDays
        {
            get => ValidDuration.TotalDays;
            set => ValidDuration = TimeSpan.FromDays(value);
        }

        public TimeSpan ValidDuration
        {
            get => _validDuration;
            set
            {
                if (value.Equals(_validDuration)) return;
                _validDuration = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ValidDurationInDays));
            }
        }

        public DateTime? ExpirationDate
        {
            get => _expirationDate;
            set
            {
                if (value.Equals(_expirationDate)) return;
                _expirationDate = value;
                if (value != null)
                {
                    ValidDuration = value.Value - DateTime.Now;
                }
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (value == _password) return;
                _password = value;
                OnPropertyChanged();
            }
        }

        public int PasswordLength
        {
            get => _passwordLength;
            set
            {
                if (value == _passwordLength) return;
                _passwordLength = value;
                OnPropertyChanged();
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (value == _isBusy) return;
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged
    }
}
