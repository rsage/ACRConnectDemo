using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using Acr.Security.AcrIdTool.Annotations;
using Acr.Security.AcrIdTool.SecureAuth;

namespace Acr.Security.AcrIdTool
{
    public class MainModel: INotifyPropertyChanged
    {
        private const string ProfileServicePath = "/webservice/profilews.svc";
        private string _baseUrl;

        private readonly string[] _realms =
        {
            "/SecureAuth0", "/SecureAuth1", "/SecureAuth2", "/SecureAuth3",
            "/SecureAuth4", "/SecureAuth5", "/SecureAuth6", "/SecureAuth7", 
            "/SecureAuth8", "/SecureAuth9", "/SecureAuth10", "/SecureAuth11", 
            "/SecureAuth12", "/SecureAuth13", "/SecureAuth14"
        };
        private string _selectedRealm;
        private Visibility _showProgress = Visibility.Collapsed;
        private readonly ProfileClient _profileClient;

        public MainModel()
        {
            _profileClient = new ProfileClient();
            var defaultUrl = _profileClient.Endpoint.Address.Uri;
            _baseUrl = defaultUrl.AbsoluteUri.Replace(defaultUrl.PathAndQuery, "");
            _selectedRealm = _realms.FirstOrDefault(defaultUrl.PathAndQuery.StartsWith);
        }

        public string BaseUrl
        {
            get { return _baseUrl; }
            set
            {
                if (value == _baseUrl) return;
                _baseUrl = value;
                OnPropertyChanged();
            }
        }

        public string[] Realms
        {
            get { return _realms; }
        }

        public string SelectedRealm
        {
            get { return _selectedRealm; }
            set
            {
                if (value == _selectedRealm) return;
                _selectedRealm = value;
                OnPropertyChanged();
            }
        }

        public Visibility ShowProgress
        {
            get { return _showProgress; }
            set
            {
                if (value == _showProgress) return;
                _showProgress = value;
                OnPropertyChanged();
            }
        }

        public async Task<bool> TestConnectionAsync()
        {
            ShowProgress = Visibility.Visible;
            try
            {
                _profileClient.Endpoint.Address = new EndpointAddress(_baseUrl + SelectedRealm + ProfileServicePath);
                const string testText = "Connection test";

                var res = await _profileClient.EchoAsync(testText);

                ShowProgress = Visibility.Hidden;
                return res.EndsWith(testText);
            }
            catch
            {
                ShowProgress = Visibility.Hidden;
                return false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}